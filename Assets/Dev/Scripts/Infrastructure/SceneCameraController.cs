using UnityEngine;

namespace Dev.Infrastructure
{
    public class SceneCameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;
    }
}