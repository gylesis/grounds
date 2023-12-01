﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.Infrastructure
{
    public class ConnectionManager : NetworkContext, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner _networkRunner;

        private PopUpService _popUpService;

        public static ConnectionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();
            
            if (networkRunner.IsConnectedToServer)
            {
                _networkRunner.gameObject.SetActive(false);
                return;
            }
            else
            {
                _networkRunner.AddCallbacks(this);

                var startGameArgs = new StartGameArgs();

                startGameArgs.GameMode = GameMode.Shared;
                startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
                startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

                _networkRunner.StartGame(startGameArgs);
            }
        }

        public override void Spawned()
        {
            Runner.AddCallbacks(this);
        }

        public void Disconnect()
        {
            Runner.Shutdown();
            
            _popUpService.HideAllPopUps();

            SceneManager.LoadScene(0);
        }

        public async void
            OnPlayerJoined(NetworkRunner runner,
                PlayerRef player) // for new player after lobby started. invokes if game starts from Lobby
        {
            if (runner.GameMode == GameMode.Shared)
            {
                Debug.Log($"Someone's late connection to the game, spawning {player}");

                await Task.Delay(2000);

            }
        }

        public async void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"On Player Left");

            if (runner.IsSharedModeMasterClient)
            {
                Debug.Log($"Despawning player");
                //_playersSpawner.DespawnPlayer(player, true);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"On Shutdown: {shutdownReason}");
        }

        public async void OnConnectedToServer(NetworkRunner runner) // invokes if game starts from Main scene
        {
            if (runner.GameMode == GameMode.Shared)
            {
                PlayerRef playerRef = runner.LocalPlayer;

                Debug.Log($"Someone connected to the game");
                Debug.Log($"Spawning player... {playerRef}");


            }
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log($"On disconnect from server");
            Application.Quit();
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log($"On Host migration");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public async void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log($"OnSceneLoadDone");

            /*if (runner.IsSharedModeMasterClient)
            {
                LevelService.Instance.LoadLevel(_gameSettings.FirstLevelName.ToString());

                await Task.Delay(3000); // TODO wait until all players load the scene

                foreach (PlayerRef playerRef in PlayerManager.PlayerQueue)
                {
                    // Debug.Log($"Respawning Player {_playersDataService.GetNickname(playerRef)}");
                    Debug.Log($"Spawning Player {playerRef}");

                    _playersSpawner.ChooseCharacterClass(playerRef);
                }

                PlayerManager.PlayerQueue.Clear();
            }*/
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"OnSceneLoadStart");
        }
    }
}