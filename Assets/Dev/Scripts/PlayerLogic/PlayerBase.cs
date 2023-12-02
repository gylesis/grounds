using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Fusion;
using UnityEngine;

namespace Dev.PlayerLogic
{
    public class PlayerBase : NetworkContext
    {
        [HideInInspector] [Networked] public PlayerCharacter PlayerCharacterInstance { get; set; }

        [SerializeField] private CameraController _cameraController;

        public CameraController CameraController => _cameraController;
    }
}