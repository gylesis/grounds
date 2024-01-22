using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using Fusion.KCC;
using UnityEngine;
using Zenject;

namespace Dev.PlayerLogic
{
    public class LobbyPlayerController : BasePlayerController
    {   
        public override void FixedUpdateNetwork()
        {
            if (GetInput<PlayerInput>(out var input))
            {
                var wasPressed = input.Buttons.GetPressed(_buttonsPrevious);
                var wasReleased = input.Buttons.GetReleased(_buttonsPrevious);

                _buttonsPrevious = input.Buttons;

                if (AllowToMove == false) return;
                
                MoveHandle(input);
                JumpHandle(wasPressed);
                
                if (AllowToAim == false) return;
                
                LookRotationHandle(input);

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

        public override void Render()
        {
            Quaternion quaternion = _cameraTransform.transform.rotation;
            Vector3 eulerAngles = quaternion.eulerAngles;
            _cameraTransform.transform.rotation =
                Quaternion.Euler(-_kcc.FixedData.LookPitch, eulerAngles.y, eulerAngles.z);
        }
    }
}