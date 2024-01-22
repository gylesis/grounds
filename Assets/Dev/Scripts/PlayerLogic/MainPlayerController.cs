using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.PlayerLogic
{
    public class MainPlayerController : BasePlayerController
    {
        private GameInventory _gameInventory;
        
        [Networked] private NetworkBool IsInventoryOpened { get; set; }

        [Inject]
        private void Construct(GameInventory gameInventory, PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
            _gameInventory = gameInventory;
        }

        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();

            _gameInventory.InventoryOpened.TakeUntilDestroy(this).Subscribe((OnInventoryOpened));
        }
    
        private void OnInventoryOpened(bool isOpened)
        {
            Debug.Log($"inventory is opened {isOpened}");
            IsInventoryOpened = isOpened;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput<PlayerInput>(out var input))
            {
                var wasPressed = input.Buttons.GetPressed(_buttonsPrevious);
                var wasReleased = input.Buttons.GetReleased(_buttonsPrevious);

                _buttonsPrevious = input.Buttons;

                InventoryHandle(wasPressed);

                if (AllowToMove == false) return;
                
                MoveHandle(input);
                JumpHandle(wasPressed);
                
                if (AllowToAim == false) return;
                
                LookRotationHandle(input);

                if(IsInventoryOpened) return;

                _hands.OnInput(input, wasPressed, wasReleased);
                _interactor.ItemHandle(input, wasPressed, wasReleased);
            }
        }

        public void SetAllowToAim(bool allow)
        {
            AllowToAim = allow;

            if (allow == false)
            {
                _kcc.AddLookRotation(Vector2.zero);
            }
        }
        
        public void SetAllowToMove(bool allow)
        {
            AllowToMove = allow;

            if (allow == false)
            {
                _kcc.SetInputDirection(Vector3.zero);
                _kcc.SetDynamicVelocity(Vector3.zero);
                _kcc.SetExternalVelocity(Vector3.zero);
                _kcc.SetKinematicVelocity(Vector3.zero);
            }
        }

        private void JumpHandle(NetworkButtons wasPressed)
        {
            if (wasPressed.IsSet(Buttons.Jump))
            {
                _kcc.Jump(Vector3.up * _jumpModifier);
            }
        }

        private void LookRotationHandle(PlayerInput input)
        {
            _kcc.AddLookRotation(input.LookDirection * _sensivity * Runner.DeltaTime);
        }

        private void MoveHandle(PlayerInput input)
        {
            Vector3 moveDirection = _kcc.transform.forward * input.MoveDirection.y +
                                    _kcc.transform.right * input.MoveDirection.x;
            moveDirection.Normalize();
            _kcc.SetInputDirection(moveDirection);

            if (input.Sprint)
            {
                if (_kcc.FixedData.IsGrounded)
                    _kcc.AddExternalVelocity(moveDirection * _sprintAcceleration);
            }

            _playerView.RPC_OnInput(input.MoveDirection);
        }

        private void InventoryHandle(NetworkButtons wasPressed)
        {
            var toOpenInventory = wasPressed.IsSet(Buttons.ToggleInventory);

            if (toOpenInventory)
            {
                if (IsInventoryOpened == false)
                {
                    _gameInventory.ShowInventory(Runner.LocalPlayer);
                }
                else
                {
                    _gameInventory.HideInventory(Runner.LocalPlayer);
                }

                IsInventoryOpened = !IsInventoryOpened;
            }
        }

        public override void Render()
        {
            Quaternion quaternion = _cameraTransform.transform.rotation;
            Vector3 eulerAngles = quaternion.eulerAngles;
            _cameraTransform.transform.rotation =
                Quaternion.Euler(-_kcc.FixedData.LookPitch, eulerAngles.y, eulerAngles.z);
        }
    }
}