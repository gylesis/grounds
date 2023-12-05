using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Scripts;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Dev.Infrastructure
{
    public class ConnectionManager : NetworkContext, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner _networkRunner;
        [SerializeField] private Transform _testSpawner;
        
        private PopUpService _popUpService;
        private PlayersSpawner _playersSpawner;
        private SceneCameraController _sceneCameraController;

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

            TryAutoConnect();
        }

        private async void TryAutoConnect()
        {
            _networkRunner.gameObject.SetActive(false);
            
            NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();

            if (networkRunner == null)
            {
                _networkRunner.gameObject.SetActive(true);
                networkRunner = _networkRunner;
            }
            
            if (networkRunner.State == NetworkRunner.States.Running)
            {
                _testSpawner.gameObject.SetActive(false);
                networkRunner.AddCallbacks(this);
                _networkRunner.gameObject.SetActive(false);
                return;
            }
            else
            {
                _networkRunner.gameObject.SetActive(true);
                _testSpawner.gameObject.SetActive(true);
                _networkRunner.AddCallbacks(this);

                return;
                
                var startGameArgs = new StartGameArgs();

                startGameArgs.GameMode = GameMode.AutoHostOrClient;
                startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
                startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;
                startGameArgs.SessionName = "Test1";

                StartGameResult startGameResult = await _networkRunner.StartGame(startGameArgs);

                if (startGameResult.Ok)
                {
                    Debug.Log($"Started game with mode: {_networkRunner.GameMode}");
                }
                else
                {
                    Debug.LogError($"{startGameResult.ErrorMessage}");
                }
            }
        }

        [Inject]
        private void Init(PlayersSpawner playersSpawner, SceneCameraController sceneCameraController)
        {
            _sceneCameraController = sceneCameraController;
            _playersSpawner = playersSpawner;
        }
        
        // for new player after lobby started. invokes if game starts from Lobby
        public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
        {
            if (runner.IsServer)
            {
                Debug.Log($"Player joined, Spawning");
                _playersSpawner.SpawnPlayer(player);
            }
        }

        public async void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"On Player Left");

            if (runner.IsServer)
            {
                _playersSpawner.DespawnPlayer(player);
                Debug.Log($"Despawning player");
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"On Shutdown: {shutdownReason}");
        }

        public async void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public async void OnSceneLoadDone(NetworkRunner runner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            
            _sceneCameraController.Camera.gameObject.SetActive(false);
            
            Debug.Log($"OnSceneLoadDone");

            if (runner.IsServer)
            {
                foreach (PlayerRef playerRef in PlayersGameData.PlayersQueue)
                {
                    _playersSpawner.SpawnPlayer(playerRef);
                }

                PlayersGameData.PlayersQueue.Clear();
            }
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"OnSceneLoadStart");
        }

        public void Disconnect()
        {
            Runner.Shutdown();

            _popUpService.HideAllPopUps();

            SceneManager.LoadScene(0);
        }
    }
}