using UnityEngine;

namespace Dev.Scripts.Items
{
    public class DraggableObject : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;

        public void PrepareToDrag()
        {
            Rigidbody.useGravity = false;
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.velocity = Vector3.zero;
            Rigidbody.isKinematic = false;
            // Rigidbody.centerOfMass = _raycastHitPoint;
        }

        public void SetFreezeState(bool toFreeze)
        {
            if (toFreeze)
            {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero;
                Rigidbody.useGravity = false;
                Rigidbody.isKinematic = true;
            }
            else
            {
                Rigidbody.isKinematic = false;
                Rigidbody.useGravity = true;
            }
            
        }
        
        public void MakeFree(Vector3 throwVelocity)
        {
            Rigidbody.isKinematic = false;
           Rigidbody.centerOfMass = Vector3.zero;
           Rigidbody.useGravity = true;
           Rigidbody.velocity += throwVelocity;     
        }
        
    }
}