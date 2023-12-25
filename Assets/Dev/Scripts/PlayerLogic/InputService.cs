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
        private bool _throwState;
        private bool _dropItemState;

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
            bool swingState = _playerInputs.Player.SwingItem.IsPressed();
            bool pickUpState = _playerInputs.Player.PickItem.IsPressed();
            bool alternateHand = _playerInputs.Player.AlternateHand.IsPressed();
            bool toggleInventory = _playerInputs.Player.ToggleInventory.IsPressed();
            bool putItemInInventory = _playerInputs.Player.PutItemInInventory.IsPressed();
            bool useItem = _playerInputs.Player.UseItem.IsPressed();
            bool reload = _playerInputs.Player.ReloadWeapon.IsPressed();

            _playerInputs.Player.ThrowItem.performed += _ => _throwState = true;
            _playerInputs.Player.ThrowItem.canceled += _ => _throwState = false;

            _playerInputs.Player.DropItem.performed += _ => _dropItemState = true;
            _playerInputs.Player.DropItem.canceled += _ => _dropItemState = false;

            Vector2 inputVector = _playerInputs.Player.Move.ReadValue<Vector2>();
            Vector2 lookVector = _playerInputs.Player.Look.ReadValue<Vector2>();

            Vector2 look = new Vector2(lookVector.y, lookVector.x);
            Vector2 keyBoardInput = new Vector2(inputVector.x,inputVector.y);

            PlayerInput playerInput = new PlayerInput();

            playerInput.Sprint = sprintState;
            playerInput.MoveDirection = keyBoardInput;
            playerInput.LookDirection = look;
            playerInput.Buttons.Set(Buttons.Jump, jumpState);
            playerInput.Buttons.Set(Buttons.Swing, swingState);
            playerInput.Buttons.Set(Buttons.Throw, _throwState);
            playerInput.Buttons.Set(Buttons.PickItem, pickUpState);
            playerInput.Buttons.Set(Buttons.AlternateHand, alternateHand);
            playerInput.Buttons.Set(Buttons.DropItem, _dropItemState);
            playerInput.Buttons.Set(Buttons.ToggleInventory, toggleInventory);
            playerInput.Buttons.Set(Buttons.PutItemToInventory, putItemInInventory);
            playerInput.Buttons.Set(Buttons.UseItem, useItem);
            playerInput.Buttons.Set(Buttons.ReloadWeapon, reload);

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