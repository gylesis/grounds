using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.LevelLogic;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.Utils;
using Fusion;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Infrastructure
{
    public class PlayersSpawner : NetworkContext
    {
        [SerializeField] private SpawnPoint[] _spawnPoints;
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;
        private DiContainer _diContainer;
    
        public Subject<PlayerRef> PlayerSpawned { get; } = new Subject<PlayerRef>();
        public Subject<PlayerRef> PlayerDeSpawned { get; } = new Subject<PlayerRef>();
        
        [Networked] private NetworkDictionary<PlayerRef, PlayerCharacter> PlayersList { get; }
        
        public int PlayersCount => PlayersList.Count;

        public List<PlayerCharacter> AllPlayers => PlayersList.Select(x => x.Value).ToList();
        
        [Inject]
        private void Construct(DiContainer diContainer)
        {
            _diContainer = diContainer;
        }
        
        public void SpawnPlayer(PlayerRef playerRef, bool firstSpawn = true)
        {
            /*if (PlayersList.ContainsKey(playerRef))
            {
                Debug.Log($"This player already has character!!");
                return;
            }*/
            
            PlayerCharacter playerCharacterPrefab = _playerCharacterPrefab;


            Vector3 spawnPos = _spawnPoints.GetFreeSpawnPoint().SpawnPos;

            if (Runner.IsSinglePlayer)
            {
                spawnPos = _spawnPoints.First().SpawnPos;
            }
            
            PlayerCharacter playerCharacter = Runner.Spawn(playerCharacterPrefab, spawnPos,
                quaternion.identity, playerRef);

            NetworkObject playerNetObj = playerCharacter.Object;

            PlayersList.Set(playerRef, playerCharacter);

            playerNetObj.RequestStateAuthority();
            playerNetObj.AssignInputAuthority(playerRef);
            Runner.SetPlayerObject(playerRef, playerNetObj);

            var playerName = $"Player №{playerNetObj.InputAuthority.PlayerId}";
            playerCharacter.RPC_SetName(playerName);

            //RespawnPlayer(playerRef);

            PlayersGameData.AddAlivePlayer(playerRef);
            
            RPC_OnPlayerSpawnedInvoke(playerCharacter);
            
            //LoadWeapon(player);
        }

        public void DespawnPlayer(PlayerRef playerRef)
        {   
            PlayerCharacter playerCharacter = GetPlayer(playerRef);

            Runner.Despawn(playerCharacter.Object);

            PlayerDeSpawned.OnNext(playerRef);

            PlayersGameData.RemoveAlivePlayer(playerRef);
            PlayersList.Remove(playerRef);
        }

        private void RPC_AddPlayer(PlayerRef playerRef, PlayerCharacter playerBase)
        {
            PlayersList.Add(playerRef, playerBase);
        }

        /*private void AssignTeam(PlayerRef playerRef)
        {
            bool doPlayerHasTeam = _teamsService.DoPlayerHasTeam(playerRef);

            if (doPlayerHasTeam) return;

            TeamSide teamSide = TeamSide.Blue;

            if (PlayersCount % 2 == 0)
            {
                teamSide = TeamSide.Red;
            }

            _teamsService.AssignForTeam(playerRef, teamSide);
        }*/

        public void SetPlayerActiveState(PlayerRef playerRef, bool isOn)
        {
            PlayerCharacter playerCharacter = GetPlayer(playerRef);

            playerCharacter.gameObject.SetActive(isOn);

        }

        public void RespawnPlayerCharacter(PlayerRef playerRef)
        {
          //  _playersHealthService.RestorePlayerHealth(playerRef);

            //TeamSide playerTeamSide = _teamsService.GetPlayerTeamSide(playerRef);

           // var spawnPoints = LevelService.Instance.CurrentLevel.GetSpawnPointsByTeam(playerTeamSide);

           // SpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            PlayerCharacter playerCharacter = GetPlayer(playerRef);

           // playerCharacter.RPC_SetPos(spawnPoint.transform.position);

            //playerCharacter.PlayerController.AllowToMove = true;
           // playerCharacter.PlayerController.AllowToShoot = true;

            //playerCharacter.HitboxRoot.HitboxRootActive = true;

           // ColorTeamBanner(playerRef);
        }
        
        public PlayerCharacter GetPlayer(PlayerRef playerRef)
        {
            return PlayersList[playerRef];
        }

        public Vector3 GetPlayerPos(PlayerRef playerRef) => GetPlayer(playerRef).transform.position;

        private void RPC_OnPlayerSpawnedInvoke(PlayerCharacter playerCharacter)
        {
            PlayerSpawned.OnNext(playerCharacter.Object.InputAuthority);
        }
    }
}