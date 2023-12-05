using System;
using System.Collections.Generic;
using Dev.Infrastructure;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Dev.Scripts
{
    public class Test : NetworkContext, INetworkRunnerCallbacks
    {
        private NetworkRunner _networkRunner;
        private SceneLoader _sceneLoader;

        private bool _isloading;
        
        private void Awake()
        {
            _networkRunner = gameObject.AddComponent<NetworkRunner>();
            _sceneLoader = gameObject.AddComponent<SceneLoader>();
        }

        private void OnGUI()
        {
            if(_isloading) return;
            
            if (GUI.Button(new Rect(100, 100, 100, 100), "Join 1"))
            {
                StartGame("Session 1");
            } 
            
            if (GUI.Button(new Rect(300, 100, 100, 100), "Join 2"))
            {
                StartGame("Session 2");
            } 
            
        }

        private async void StartGame(string sessionName)
        {
            _isloading = true;
            
            if (_networkRunner.IsShutdown == false)
            {
                await _networkRunner.Shutdown(false, ShutdownReason.GameClosed);
            }
            
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.AutoHostOrClient;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.SessionName = sessionName;

            StartGameResult result = await _networkRunner.StartGame(startGameArgs);

            _isloading = false;

            if (result.Ok)
            {
                Debug.Log($"Connected to session {sessionName}");
            }
            else
            {
                Debug.Log($"Error: {result.ErrorMessage}");
            }
            
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"Server shutdown {shutdownReason}");
        }

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

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}