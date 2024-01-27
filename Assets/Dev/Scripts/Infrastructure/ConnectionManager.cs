using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Scripts.UI.PopUpsAndMenus;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Dev.Scripts.Infrastructure
{
    public class ConnectionManager : NetworkContext, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner _networkRunner;

        private PopUpService _popUpService;
        private PlayersSpawner _playersSpawner;
        private SceneCameraController _sceneCameraController;

        public static ConnectionManager Instance { get; private set; }

        public Action<StartGameResult> LastGameStartResult;

        public override void Spawned()
        {
            Debug.Log($"Connection manager spawned");
            base.Spawned();
            
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
            if (Runner)
            {
                Debug.Log($"Auto connect returned");
                Runner.AddCallbacks(this);
                return;
            }

            Debug.Log($"Auto connect procceed");
            NetworkRunner networkRunner = FindObjectOfType<NetworkRunner>();

            if (networkRunner == null)
            {
                _networkRunner.gameObject.SetActive(true);
                networkRunner = _networkRunner;
            }

            if (networkRunner.State == NetworkRunner.States.Running)
            {
                networkRunner.AddCallbacks(this);
                _networkRunner.gameObject.SetActive(false);
            }
            else
            {
                _networkRunner.gameObject.SetActive(true);
                _networkRunner.AddCallbacks(this);
            }
        }

        [Inject]
        private void Construct(PlayersSpawner playersSpawner, SceneCameraController sceneCameraController)
        {
            _sceneCameraController = sceneCameraController;
            _playersSpawner = playersSpawner;
        }

        private async UniTask<StartGameResult> StartGame(StartGameArgs startGameArgs)
        {
            _networkRunner.gameObject.SetActive(true);
            var startGameResult = await _networkRunner.StartGame(startGameArgs);

            if (startGameResult.Ok == false)
            {
                Debug.LogError($"{startGameResult.ErrorMessage}");
            }
            LastGameStartResult?.Invoke(startGameResult);
            
            return startGameResult;
        }

        public void StartSinglePlayer()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Single;
            startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;
            
            StartGame(startGameArgs);
        }

        public void StartHost()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Host;
            startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGame(startGameArgs);
        }

        public void StartServer()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Server;
            startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGame(startGameArgs);
        }

        public void StartClient()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Client;
            startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGame(startGameArgs);
        }

        public async void QuitToLobby()
        {
            NetworkRunner networkRunner = Runner;

            if (Runner.State == NetworkRunner.States.Running)
            {
                await Runner.Shutdown(false, ShutdownReason.GameClosed);
                await UniTask.DelayFrame(5);
            }
                
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.AutoHostOrClient;
            startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGameResult gameResult = await networkRunner.StartGame(startGameArgs);

            if (gameResult.Ok)
            {
                networkRunner.RemoveCallbacks(this);
                FindObjectOfType<SceneLoader>().LoadScene("LobbyScene");
                Debug.Log($"Loading lobby");
            }
            else
            {
                Debug.LogError($"Game not started {gameResult.ErrorMessage}. Probably server isn't started");
            }
        }
        
        
        // for new player after lobby started. invokes if game starts from Lobby
        public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log($"Player {player} joined, spawning");
                await UniTask.DelayFrame(6);
                _playersSpawner.SpawnPlayer(player);
            }
        }

        public async void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log($"Player {player} left, despawning");
                _playersSpawner.DespawnPlayer(player);
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

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

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"On Shutdown: {shutdownReason}");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public async void OnSceneLoadDone(NetworkRunner runner)
        {
            _sceneCameraController.SetActiveState(false);

            Debug.Log($"OnSceneLoadDone");

            if (runner.IsServer)
            {
                Debug.Log($"Server ready to accept new players!");
                Debug.Log($"__________________________________________________");
            }
           
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"OnSceneLoadStart");
        }
    }
}