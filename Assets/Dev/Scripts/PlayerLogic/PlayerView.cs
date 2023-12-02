using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayerView : NetworkContext
    {
        [SerializeField] private NetworkTransform _armsTransform;
        [SerializeField] private Transform _bodyTransform;

        [SerializeField] private Transform _cameraTransform;
        
        public override void Spawned()
        {
            if (HasInputAuthority)
            {
                _armsTransform.InterpolationSpace = Spaces.World;
                _armsTransform.Transform.parent = _cameraTransform;
                _bodyTransform.gameObject.SetActive(false);                
            }
            else
            {
                _bodyTransform.gameObject.SetActive(true);
                _armsTransform.gameObject.SetActive(true);
            }
        }

        public override void Render()
        {
            if (HasInputAuthority)
            {
                _armsTransform.Transform.SetPositionAndRotation(_cameraTransform.position, _cameraTransform.rotation);
            }
        }
    }
}