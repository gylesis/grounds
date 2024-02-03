using Dev.Scripts.Infrastructure;
using Fusion.KCC;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerCharacter : NetworkContext
    {
        [SerializeField] private KCC _kcc;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private BasePlayerController _basePlayerController;
        [SerializeField] private Interactor _interactor;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private InputService _inputService;
        [SerializeField] private Health _health;

        public InputService InputService => _inputService;
        public CameraController CameraController => _cameraController;
        public PlayerView PlayerView => _playerView;
        public BasePlayerController BasePlayerController => _basePlayerController;
        public Interactor Interactor => _interactor;
        public KCC Kcc => _kcc;
        public PlayerView Animator => _playerView;
        public Health Health => _health;

    }
}