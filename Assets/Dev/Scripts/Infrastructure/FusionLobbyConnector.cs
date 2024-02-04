using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Scripts.LevelLogic;
using Dev.Scripts.PlayerLogic;
using Fusion;
using Fusion.Sockets;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Dev.Scripts.Infrastructure
{
    public class FusionLobbyConnector : NetworkContext, INetworkRunnerCallbacks
    {
        [SerializeField] private PlayerTriggerZone _playerTriggerZone;
        [SerializeField] private int _neededPlayersAmountToStartGame = 1;
        
        private SceneLoader _sceneLoader;
        private PlayersSpawner _playersSpawner;
        private SceneCameraController _sceneCameraController;

        private void Awake()
        {
            _playerTriggerZone.PlayerEntered.TakeUntilDestroy(this).Subscribe((OnPlayerEntered));
        }

        [Inject]
        private void Construct(SceneLoader sceneLoader, PlayersSpawner playersSpawner, SceneCameraController sceneCameraController)
        {
            _sceneCameraController = sceneCameraController;
            _playersSpawner = playersSpawner;
            _sceneLoader = sceneLoader;
        }

        public override void Spawned()
        {
            base.Spawned();
            Runner.AddCallbacks(this);
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
            NetworkRunner networkRunner = Runner;

            Curtains.Instance.SetText("Starting game");
            Curtains.Instance.Show();
            
            if (Runner.State == NetworkRunner.States.Running)
            {
                await Runner.Shutdown(false, ShutdownReason.GameClosed);
                await UniTask.DelayFrame(5);
            }
                
            var startGameArgs = new StartGameArgs();
            
            startGameArgs.GameMode = GameMode.AutoHostOrClient;
            startGameArgs.SceneManager = _sceneLoader;
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;

            StartGameResult gameResult = await networkRunner.StartGame(startGameArgs);

            if (gameResult.Ok)
            {
                Curtains.Instance.SetText("Game found, joining map");
                
                networkRunner.RemoveCallbacks(this);
                _sceneLoader.LoadScene("MainScene");
                Debug.Log($"Game started, loading main level");
            }
            else
            {
                Debug.LogError($"Game not started {gameResult.ErrorMessage}. Probably server isn't started");
            }
        }
        
        
        public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log($"Player joined, Spawning");
                await UniTask.DelayFrame(6);
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
            _sceneCameraController.SetActiveState(false);
            Debug.Log($"Scene load done 1");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log($"Scene load started 1");
        }
    }
}