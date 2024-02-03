using UnityEngine;

namespace Dev.Scripts.Infrastructure
{
    public class SceneCameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        public Camera Camera => _camera;

        public bool IsActive { get; private set; }
        
        public void SetActiveState(bool isActive)
        {
            IsActive = isActive;
            _camera.gameObject.SetActive(isActive);
        }
    }
}