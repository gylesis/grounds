using System;
using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using Fusion.KCC;
using UnityEngine;
using Zenject;
using Cursor = UnityEngine.Cursor;

namespace Dev.PlayerLogic
{
    public class PlayerController : NetworkContext
    {
        [Networked] private NetworkButtons _buttonsPrevious { get; set; }

        [SerializeField] private PlayerView _playerView;
        [SerializeField] private KCC _kcc;
        [SerializeField] private Hands _hands;
        [SerializeField] private Interactor _interactor;

        [SerializeField] private Transform _cameraTransform;

        [SerializeField] private float _jumpModifier = 2;
        [SerializeField] private float _sensivity = 2;

        [SerializeField] private float _sprintAcceleration = 4.5f;

        private PopUpService _popUpService;
        private GameInventory _gameInventory;

        [Networked] private NetworkBool AllowToMove { get; set; } = true;
        [Networked] private NetworkBool AllowToAim{ get; set; } = true;

        private bool _invOpened = false;

        public Hands Hands => _hands;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        [Inject]
        private void Construct(GameInventory gameInventory)
        {
            _gameInventory = gameInventory;
        }


        public override void Spawned()
        {
            if (HasInputAuthority == false)
            {
                GetComponentInChildren<Camera>(true).gameObject.SetActive(false);
            }
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

                if(_invOpened) return;

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
                if (_invOpened)
                {
                    _gameInventory.Hide();
                }
                else
                {
                    _gameInventory.ShowInventory(Object.InputAuthority);
                }

                _invOpened = !_invOpened;
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