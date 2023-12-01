using System;
using System.Collections.Generic;
using System.Linq;
using Dev.PlayerLogic;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Zenject;

namespace Dev.Infrastructure
{
    public class PlayersSpawner : NetworkContext
    {
        [SerializeField] private InputService _inputServicePrefab;
        [SerializeField] private PlayerBase _playerBasePrefab;

        [SerializeField] private Transform _spawnPoint;
        
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;
        public Subject<PlayerSpawnEventContext> PlayerSpawned { get; } = new Subject<PlayerSpawnEventContext>();
        public Subject<PlayerSpawnEventContext> CharacterSpawned { get; } = new Subject<PlayerSpawnEventContext>();
        public Subject<PlayerRef> CharacterDeSpawned { get; } = new Subject<PlayerRef>();
        public Subject<PlayerRef> PlayerDeSpawned { get; } = new Subject<PlayerRef>();
        public Dictionary<PlayerRef, List<NetworkObject>> PlayerServices => _playerServices;
        [Networked] private NetworkDictionary<PlayerRef, PlayerBase> PlayersBase { get; }

        public List<PlayerCharacter> Players => PlayersBase.Select(x => x.Value.PlayerCharacterInstance).ToList();

        public int PlayersCount => PlayersBase.Count;

        private Dictionary<PlayerRef, List<NetworkObject>> _playerServices =
            new Dictionary<PlayerRef, List<NetworkObject>>();

        private TeamsService _teamsService;
        private PopUpService _popUpService;

        [Inject]
        private void Init(TeamsService teamsService, PopUpService popUpService)
        {
            _popUpService = popUpService;
            _teamsService = teamsService;
        }

        public PlayerCharacter SpawnPlayer(PlayerRef playerRef,  NetworkRunner networkRunner, bool firstSpawn = true)
        {
            if (firstSpawn)
            {
                AssignTeam(playerRef);

                PlayerBase playerBase = networkRunner.Spawn(_playerBasePrefab, null, null, playerRef);
                playerBase.Object.AssignInputAuthority(playerRef);

                RPC_AddPlayer(playerRef, playerBase);
            }
            else
            {
                DespawnPlayer(playerRef, false);
            }

            PlayerCharacter playerCharacterPrefab = _playerCharacterPrefab;

            Vector3 spawnPos = _spawnPoint.position;

            PlayerCharacter playerCharacter = networkRunner.Spawn(playerCharacterPrefab, spawnPos,
                quaternion.identity, playerRef);

            NetworkObject playerNetObj = playerCharacter.Object;

            PlayersBase[playerRef].PlayerCharacterInstance = playerCharacter;

            playerNetObj.RequestStateAuthority();
            playerNetObj.AssignInputAuthority(playerRef);
            networkRunner.SetPlayerObject(playerRef, playerNetObj);

            if (firstSpawn)
            {
                _playerServices.Add(playerRef, new List<NetworkObject>());

                SetInputService(playerRef, networkRunner);
                SetCamera(playerRef, playerCharacter, networkRunner);
            }
            else
            {
            }

            var playerName = $"Player №{playerNetObj.InputAuthority.PlayerId}";
            playerCharacter.RPC_SetName(playerName);

            //RespawnPlayer(playerRef);

            RPC_OnPlayerSpawnedInvoke(playerCharacter);

            //LoadWeapon(player);

            return playerCharacter;
        }

        public void DespawnPlayer(PlayerRef playerRef, bool isLeftFromSession)
        {
            PlayerCharacter playerCharacter = GetPlayer(playerRef);

            Runner.Despawn(playerCharacter.Object);

            CharacterDeSpawned.OnNext(playerRef);

            //GetPlayerCameraController(playerRef).SetFollowState(false);

            if (isLeftFromSession)
            {
                // remove player's NO

                _teamsService.RemoveFromTeam(playerRef);

                PlayersBase.Remove(playerRef);
            }

        }

        private void RPC_AddPlayer(PlayerRef playerRef, PlayerBase playerBase)
        {
            PlayersBase.Add(playerRef, playerBase);
        }

        private void AssignTeam(PlayerRef playerRef)
        {
            bool doPlayerHasTeam = _teamsService.DoPlayerHasTeam(playerRef);

            if (doPlayerHasTeam) return;

            TeamSide teamSide = TeamSide.Blue;

            if (PlayersCount % 2 == 0)
            {
                teamSide = TeamSide.Red;
            }

            _teamsService.AssignForTeam(playerRef, teamSide);
        }

        private void SetCamera(PlayerRef playerRef, PlayerCharacter playerCharacter, NetworkRunner networkRunner)
        {
            /*CameraController cameraController = networkRunner.Spawn(_cameraControllerPrefab,
                playerCharacter.transform.position,
                Quaternion.identity,
                playerRef);

            cameraController.SetFollowState(true);
            cameraController.Object.RequestStateAuthority();
            _playerServices[playerRef].Add(cameraController.Object);*/
        }

        private void SetInputService(PlayerRef playerRef, NetworkRunner networkRunner)
        {
            InputService inputService =
                networkRunner.Spawn(_inputServicePrefab, Vector3.zero, Quaternion.identity, playerRef);

            inputService.Object.RequestStateAuthority();

            _playerServices[playerRef].Add(inputService.Object);
        }

        public void SetPlayerActiveState(PlayerRef playerRef, bool isOn)
        {
            PlayerCharacter playerCharacter = GetPlayer(playerRef);

            playerCharacter.gameObject.SetActive(isOn);

        }

        public void RespawnPlayerCharacter(PlayerRef playerRef)
        {
          //  _playersHealthService.RestorePlayerHealth(playerRef);

            TeamSide playerTeamSide = _teamsService.GetPlayerTeamSide(playerRef);

           // var spawnPoints = LevelService.Instance.CurrentLevel.GetSpawnPointsByTeam(playerTeamSide);

           // SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            PlayerCharacter playerCharacter = GetPlayer(playerRef);

           // playerCharacter.RPC_SetPos(spawnPoint.transform.position);

            //playerCharacter.PlayerController.AllowToMove = true;
           // playerCharacter.PlayerController.AllowToShoot = true;

            //playerCharacter.HitboxRoot.HitboxRootActive = true;

           // ColorTeamBanner(playerRef);
        }

        private void LoadWeapon(PlayerCharacter playerCharacter)
        {
          //  var weaponSetupContext = new WeaponSetupContext(WeaponType.Rifle);
          //  playerCharacter.WeaponController.Init(weaponSetupContext);
        }

        public PlayerCharacter GetPlayer(PlayerRef playerRef)
        {
            return PlayersBase[playerRef].PlayerCharacterInstance;
        }

        /*public CameraController GetPlayerCameraController(PlayerRef playerRef)
        {
            List<NetworkObject> playerService = _playerServices[playerRef];

            CameraController cameraController = playerService.First(x => x.GetComponent<CameraController>() != null)
                .GetComponent<CameraController>();

            return cameraController;
        }*/

        public Vector3 GetPlayerPos(PlayerRef playerRef) => GetPlayer(playerRef).transform.position;


        [Rpc]
        private void RPC_OnPlayerSpawnedInvoke(PlayerCharacter playerCharacter)
        {
            var spawnEventContext = new PlayerSpawnEventContext();
            spawnEventContext.PlayerRef = playerCharacter.Object.InputAuthority;
            spawnEventContext.Transform = playerCharacter.transform;

            // Debug.Log($"[RPC] Player spawned");
            PlayerSpawned.OnNext(spawnEventContext);
        }
    }
}