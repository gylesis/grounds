﻿using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Fusion.KCC;
using UnityEngine;

namespace Dev.PlayerLogic
{
    public class PlayerCharacter : NetworkContext
    {
        [SerializeField] private KCC _kcc;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private Interactor _interactor;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private InputService _inputService;
        [SerializeField] private Health _health;
        

        public InputService InputService => _inputService;

        public CameraController CameraController => _cameraController;
        public PlayerView PlayerView => _playerView;
        public PlayerController PlayerController => _playerController;
        public Interactor Interactor => _interactor;
        public KCC Kcc => _kcc;
        public PlayerView Animator => _playerView;
        public Health Health => _health;
    }
}