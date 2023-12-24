using System;
using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using Fusion.KCC;
using UnityEngine;
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

        [Networked] public NetworkBool ForbidToMove { get; set; }

        private bool _invOpened = false;


        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Start()
        {
            _gameInventory = DependenciesContainer.Instance.GetDependency<GameInventory>();
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

                if (ForbidToMove) return;
                
                _kcc.AddLookRotation(input.LookDirection * _sensivity * Runner.DeltaTime);

                Vector3 moveDirection = _kcc.transform.forward * input.MoveDirection.y +
                                        _kcc.transform.right * input.MoveDirection.x;
                moveDirection.Normalize();
                _kcc.SetInputDirection(moveDirection);

                _playerView.RPC_OnInput(input.MoveDirection);

                if (wasPressed.IsSet(Buttons.Jump))
                {
                    _kcc.Jump(Vector3.up * _jumpModifier);
                }

                if (input.Sprint)
                {
                    if (_kcc.FixedData.IsGrounded)
                        _kcc.AddExternalVelocity(moveDirection * _sprintAcceleration);
                }

                var toOpenInventory = wasPressed.IsSet(Buttons.ToggleInventory);

                if (toOpenInventory)
                {
                    Debug.Log($"Show inv {_invOpened}");

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

                if(_invOpened) return;

                _hands.OnInput(input, wasPressed, wasReleased);
                _interactor.ItemHandle(input, wasPressed, wasReleased);
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