using Dev.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class CameraController : NetworkContext
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
    }
}