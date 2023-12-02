using Dev.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class CameraController : NetworkContext
    {
        [SerializeField] private Camera _characterCamera;

        public Camera CharacterCamera => _characterCamera;
    }
}