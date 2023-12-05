using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dev.Levels.Interactions;
using Dev.PlayerLogic;
using Fusion;
using Fusion.Sockets;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.Infrastructure
{
    [RequireComponent(typeof(NetworkRunner))]
    public class FusionLobbyConnector : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private PlayerTriggerZone _playerTriggerZone;
        
        private NetworkRunner _networkRunner;
        private SceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = FindObjectOfType<SceneLoader>();
            _playerTriggerZone.PlayerEntered.TakeUntilDestroy(this).Subscribe((OnPlayerEntered));

            Debug.Log($"Subscription");
            
            ConnectToLobby();
        }

        private async void ConnectToLobby()
        {
            _networkRunner = GetComponent<NetworkRunner>();
            _networkRunner.ProvideInput = true;
            _networkRunner.AddCallbacks(this);

            var joinSessionLobby = _networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

            await joinSessionLobby;

            StartGameResult result = joinSessionLobby.Result;

            if (result.Ok)
            {
                Debug.Log($"Joined lobby");
            }
            else
            {
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
        }

        private async void OnPlayerEntered(PlayerCharacter playerCharacter)
        {
            Debug.Log($"Player entered");
            
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.AutoHostOrClient;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGameResult startGameResult = await _networkRunner.StartGame(startGameArgs);

            if (startGameResult.Ok)
            {
                Debug.Log($"Game started");
            }
            else
            {
                Debug.Log($"{startGameResult.ErrorMessage}");
            }
            
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log($"Player {player} joined");
                
                if (runner.ActivePlayers.Count() > 0)
                {
                    Debug.Log($"Enough players are connected, starting game");
                    _sceneLoader.LoadScene("MainScene");
                }
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }

        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner) { }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token) { }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log($"Scene load done 1");

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"Scene load started 1");
        }
    }
}