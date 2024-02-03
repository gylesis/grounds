using Dev.Scripts.Infrastructure;
using Fusion;
using Fusion.KCC;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerCharacter : NetworkContext
    {
        [SerializeField] private KCC _kcc;
        [SerializeField] private PlayerView _playerView;
        [FormerlySerializedAs("_basePlayerController")] [SerializeField] private BasePlayerController _playerController;
        [SerializeField] private Interactor _interactor;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private InputService _inputService;
        [SerializeField] private Health _health;
        [SerializeField] private HitboxRoot _hitboxRoot;

        public HitboxRoot HitboxRoot => _hitboxRoot;
        public InputService InputService => _inputService;
        public CameraController CameraController => _cameraController;
        public PlayerView PlayerView => _playerView;
        public BasePlayerController PlayerController => _playerController;
        public Interactor Interactor => _interactor;
        public KCC Kcc => _kcc;
        public PlayerView Animator => _playerView;
        public Health Health => _health;

    }
}