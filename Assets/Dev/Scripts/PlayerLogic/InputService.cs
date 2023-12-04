using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace Dev.Infrastructure
{
    public class InputService : NetworkContext, INetworkRunnerCallbacks
    {
        private KeyCode[] _keyCodes =
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };

        private PlayerInputs _playerInputs;

        public PlayerInputs PlayerInputs => _playerInputs;

        private void Awake()
        {
            _playerInputs = new PlayerInputs();
            _playerInputs.Enable();
        }

        public override void Spawned()
        {
            Runner.AddCallbacks(this);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            bool jumpState = _playerInputs.Player.Jump.IsPressed();
            bool sprintState = _playerInputs.Player.Sprint.IsPressed();

            Vector2 inputVector = _playerInputs.Player.Move.ReadValue<Vector2>();
            Vector2 lookVector = _playerInputs.Player.Look.ReadValue<Vector2>();

            Vector2 look = new Vector2(lookVector.y, lookVector.x);
            Vector2 keyBoardInput = new Vector2(inputVector.x,inputVector.y);

            PlayerInput playerInput = new PlayerInput();

            playerInput.Sprint = sprintState;
            playerInput.MoveDirection = keyBoardInput;
            playerInput.LookDirection = look;
            playerInput.Buttons.Set(Buttons.Jump, jumpState);
            

            /*for (int i = 0; i < _keyCodes.Length; i++)
            {
                if (Input.GetKeyDown(_keyCodes[i]))
                {
                    int numberPressed = i + 1;
                    playerInput.WeaponNum = numberPressed;
                }
            }
            */

            input.Set(playerInput);
        }

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

        public void OnSceneLoadDone(NetworkRunner runner) { }

        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
}