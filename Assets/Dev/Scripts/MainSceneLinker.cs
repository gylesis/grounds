using System;
using System.Collections.Generic;
using Dev.Infrastructure;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Dev.Scripts
{
    public class MainSceneLinker : NetworkContext, INetworkRunnerCallbacks
    {
        private NetworkRunner _networkRunner;

        private void Awake()
        {
            _networkRunner = FindObjectOfType<NetworkRunner>();
            _networkRunner.AddCallbacks(this);
        }


        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

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
            Debug.Log($"Scene load done 2");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"Scene load started 2");

        }
    }
}