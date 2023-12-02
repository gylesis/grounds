using System;
using System.Collections.Generic;
using System.Linq;
using Dev.PlayerLogic;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Zenject;

namespace Dev.Infrastructure
{
    public class PlayersSpawner : NetworkContext
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private PlayerCharacter _playerCharacterPrefab;

        [SerializeField] private PlayerSpawnerPrototype _playerSpawnerPrototype;
        public Subject<PlayerRef> PlayerSpawned { get; } = new Subject<PlayerRef>();
        public Subject<PlayerRef> PlayerDeSpawned { get; } = new Subject<PlayerRef>();
        
        [Networked] private NetworkDictionary<PlayerRef, PlayerCharacter> PlayersList { get; }
        
        public int PlayersCount => PlayersList.Count;


        public void DepositPlayer(NetworkObject playerNetObj)
        {
            
        }
        
        public PlayerCharacter SpawnPlayer(PlayerRef playerRef,  NetworkRunner networkRunner, bool firstSpawn = true)
        {
            /*if (firstSpawn)
            {
                //AssignTeam(playerRef);

                PlayerCharacter player = networkRunner.Spawn(_playerCharacterPrefab, null, null, playerRef);
                player.Object.AssignInputAuthority(playerRef);

                RPC_AddPlayer(playerRef, player);
            }
            else
            {
                DespawnPlayer(playerRef, false);
            }*/

            PlayerCharacter playerCharacterPrefab = _playerCharacterPrefab;

            Vector3 spawnPos = _spawnPoint.position;

            PlayerCharacter playerCharacter = networkRunner.Spawn(playerCharacterPrefab, spawnPos,
                quaternion.identity, playerRef);

            NetworkObject playerNetObj = playerCharacter.Object;

            PlayersList.Set(playerRef, playerCharacter);

            playerNetObj.RequestStateAuthority();
            playerNetObj.AssignInputAuthority(playerRef);
            networkRunner.SetPlayerObject(playerRef, playerNetObj);

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

            PlayerDeSpawned.OnNext(playerRef);

            //GetPlayerCameraController(playerRef).SetFollowState(false);

            if (isLeftFromSession)
            {
                // remove player's NO

                //_teamsService.RemoveFromTeam(playerRef);

                PlayersList.Remove(playerRef);
            }

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

        [Rpc]
        private void RPC_OnPlayerSpawnedInvoke(PlayerCharacter playerCharacter)
        {
            // Debug.Log($"[RPC] Player spawned");
            PlayerSpawned.OnNext(playerCharacter.Object.InputAuthority);
        }
    }
}