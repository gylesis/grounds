using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Fusion;
using Fusion.KCC;
using UnityEngine;
using Zenject;

namespace Dev.PlayerLogic
{
    public abstract class BasePlayerController : NetworkContext
    {
        [Networked] protected NetworkButtons _buttonsPrevious { get; set; }

        [SerializeField] protected PlayerView _playerView;
        [SerializeField] protected KCC _kcc;
        [SerializeField] protected Hands _hands;
        [SerializeField] protected Interactor _interactor;

        [SerializeField] protected Transform _cameraTransform;

        [SerializeField] protected float _jumpModifier = 2;
        [SerializeField] protected float _sensivity = 2;

        [SerializeField] protected float _sprintAcceleration = 4.5f;

        [Networked] protected NetworkBool AllowToMove { get; set; } = true;
        [Networked] protected NetworkBool AllowToAim{ get; set; } = true;
        
        protected PlayerCharacter _playerCharacter;

        public Hands Hands => _hands;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        [Inject]
        private void Construct(PlayerCharacter playerCharacter)
        {
            _playerCharacter = playerCharacter;
        }

        private void Start()
        {
            var gameObjectContext = GetComponent<GameObjectContext>();
            DiContainerSingleton.Instance.Inject(gameObjectContext);
        }
        
        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();
            
            if (HasInputAuthority == false)
            {
                _playerCharacter.CameraController.CharacterCamera.gameObject.SetActive(false);
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