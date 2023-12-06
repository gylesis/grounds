using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Levels.Interactions;
using Dev.PlayerLogic;
using Dev.Scripts;
using Fusion;
using Fusion.Sockets;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.Infrastructure
{
    public class FusionLobbyConnector : NetworkContext, INetworkRunnerCallbacks
    {
        [SerializeField] private PlayerTriggerZone _playerTriggerZone;
        [SerializeField] private int _neededPlayersAmountToStartGame = 1;
        
        private NetworkRunner _networkRunner;
        private SceneLoader _sceneLoader;

        private void Awake()
        {
            _sceneLoader = FindObjectOfType<SceneLoader>();
            _playerTriggerZone.PlayerEntered.TakeUntilDestroy(this).Subscribe((OnPlayerEntered));
        }

        public override void Spawned()
        {
            _networkRunner = FindObjectOfType<NetworkRunner>();
            _networkRunner.AddCallbacks(this);
            base.Spawned();
        }

        private void OnPlayerEntered(PlayerCharacter playerCharacter)
        {
            if(Runner.IsServer == false) return;
             
            PlayersGameData.PutPlayerInQueue(playerCharacter.Object.InputAuthority);
            OnReadyPlayersUpdate();
        }
        
        
        private async void OnReadyPlayersUpdate()
        {
            if(Runner.IsServer == false) return;
            
            if (PlayersGameData.CountPlayersInQueue >= _neededPlayersAmountToStartGame)
            {
                Debug.Log($"Enough players are connected, starting game");

                RPC_StartNewGame();
            }
        }

        [Rpc]
        private async void RPC_StartNewGame()
        {
            if (_networkRunner.State == NetworkRunner.States.Running)
            {
                await _networkRunner.Shutdown(false, ShutdownReason.GameClosed);

                await UniTask.DelayFrame(5);
            }
                
            var startGameArgs = new StartGameArgs();
            startGameArgs.GameMode = GameMode.AutoHostOrClient;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.Scene = SceneManager.GetSceneByName("MainScene").buildIndex;

            StartGameResult gameResult = await _networkRunner.StartGame(startGameArgs);

            if (gameResult.Ok)
            {
                _sceneLoader.LoadScene("MainScene");
                Debug.Log($"Game started, loading main level");
            }
            else
            {
                Debug.LogError($"Game not started {gameResult.ErrorMessage}");
            }
        }
        
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log($"Player {player} joined");

            if (runner.IsServer)
            {
                /*PlayersGameData.PutPlayerInQueue(player);

                if (PlayersGameData.CountPlayersInQueue > 0)
                {
                    Debug.Log($"Enough players are connected, starting game");
                    _sceneLoader.LoadScene("MainScene");
                }*/
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnInput(NetworkRunner runner, NetworkInput input) { }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log($"Game shutdown - {shutdownReason}");
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

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log($"Scene load done 1");

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            //Runner.RemoveCallbacks(this);

            Debug.Log($"Scene load started 1");
        }
    }
}