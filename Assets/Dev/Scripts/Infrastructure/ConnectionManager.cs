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

        private PopUpService _popUpService;
        private PlayersSpawner _playersSpawner;
        private SceneCameraController _sceneCameraController;

        public static ConnectionManager Instance { get; private set; }

        public Action<StartGameResult> LastGameStartResult;

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
            if (Runner)
            {
                Destroy(_networkRunner.gameObject);
                return;
            }
            
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
        private void Init(PlayersSpawner playersSpawner, SceneCameraController sceneCameraController)
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

        public void StartClient()
        {
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.Client;
            startGameArgs.SceneManager = FindObjectOfType<SceneLoader>();
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGame(startGameArgs);
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

        public async void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log($"Starting host migration");
            
            // Step 2.1
            // Shutdown the current Runner, this will not be used anymore. Perform any prior setup and tear down of the old Runner

            // The new "ShutdownReason.HostMigration" can be used here to inform why it's being shut down in the "OnShutdown" callback
            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

            // Step 2.2
            // Create a new Runner.
            var newRunner = new GameObject("NetworkRunner", typeof(NetworkRunner)).GetComponent<NetworkRunner>();

            // setup the new runner...

            // Start the new Runner using the "HostMigrationToken" and pass a callback ref in "HostMigrationResume".
            StartGameResult result = await newRunner.StartGame(new StartGameArgs()
            {
                // SessionName = SessionName,              // ignored, peer never disconnects from the Photon Cloud
                // GameMode = gameMode,                    // ignored, Game Mode comes with the HostMigrationToken
                HostMigrationToken = hostMigrationToken, // contains all necessary info to restart the Runner
                HostMigrationResume = HostMigrationResume, // this will be invoked to resume the simulation
                // other args
            });



            // Check StartGameResult as usual
            if (result.Ok == false)
            {
                Debug.LogWarning(result.ShutdownReason);
            }
            else
            {
                Debug.Log("Done");
            }
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"On Shutdown: {shutdownReason}");

            // Can check if the Runner is being shutdown because of the Host Migration
            if (shutdownReason == ShutdownReason.HostMigration)
            {
                // ...
            }
            else
            {
                // Or a normal Shutdown
            }
        }

        void HostMigrationResume(NetworkRunner runner)
        {
            // Get a temporary reference for each NO from the old Host
            foreach (var resumeNO in runner.GetResumeSnapshotNetworkObjects())

                if (
                    // Extract any NetworkBehavior used to represent the position/rotation of the NetworkObject
                    // this can be either a NetworkTransform or a NetworkRigidBody, for example
                    resumeNO.TryGetBehaviour<NetworkPositionRotation>(out var posRot))
                {
                    runner.Spawn(resumeNO, position: posRot.ReadPosition(), rotation: posRot.ReadRotation(),
                        onBeforeSpawned: (runner, newNO) =>
                        {
                            // One key aspects of the Host Migration is to have a simple way of restoring the old NetworkObjects state
                            // If all state of the old NetworkObject is all what is necessary, just call the NetworkObject.CopyStateFrom
                            newNO.CopyStateFrom(resumeNO);

                            // and/or

                            // If only partial State is necessary, it is possible to copy it only from specific NetworkBehaviours
                            if (resumeNO.TryGetBehaviour<NetworkBehaviour>(out var myCustomNetworkBehaviour))
                            {
                                newNO.GetComponent<NetworkBehaviour>().CopyStateFrom(myCustomNetworkBehaviour);
                            }
                        });
                }
        }


        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public async void OnSceneLoadDone(NetworkRunner runner)
        {
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