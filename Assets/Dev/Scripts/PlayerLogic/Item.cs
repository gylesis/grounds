using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [OrderAfter(typeof(HandsService))]
    public class Item : NetworkContext
    {
        [SerializeField] private HitboxRoot _hitboxRoot;
        [SerializeField] private NetworkRigidbody _rigidbody;
        [SerializeField] private Collider _collider;

        [SerializeField] private string _testName = "Good Item 12312";

        [SerializeField] private HandType _handType;

        public HandType HandType => _handType;
        public string TestName => _testName;

        [Networked] private NetworkBool IsCarrying { get; set; }

        protected override void CorrectState()
        {
            base.CorrectState();

            Debug.Log($"Set item state");
            SetItemState(IsCarrying);
        }

        [Rpc]
        public virtual void RPC_OnPickup(bool isCarrying)
        {
            IsCarrying = isCarrying;
            SetItemState(isCarrying);
        }

        private void SetItemState(bool isCarrying)
        {
            if (isCarrying)
            {
                _rigidbody.Rigidbody.angularVelocity = Vector3.zero;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
            else
            {
            }

            _rigidbody.Rigidbody.isKinematic = isCarrying;
            _rigidbody.enabled = !isCarrying;
            _collider.enabled = !isCarrying;
            _hitboxRoot.enabled = !isCarrying;
        }
        
    }
}