using UnityEngine;

namespace Dev.Scripts.Infrastructure
{
    public class SceneCameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
    }
}