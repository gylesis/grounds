using Dev.Scripts.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class CameraController : NetworkContext
    {
        [SerializeField] private Camera _characterCamera;

        public Camera CharacterCamera => _characterCamera;


        public void SetActiveState(bool isActive)
        {
            _characterCamera.gameObject.SetActive(isActive);
        }
        
    }
}   