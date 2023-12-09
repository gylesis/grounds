using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [OrderAfter(typeof(Hands))]
    public class Item : NetworkContext
    {
        [SerializeField] private HitboxRoot _hitboxRoot;
        [SerializeField] private NetworkRigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private string _testName = "Good Item 12312";
        [SerializeField] private ItemSizeType _itemSizeType;

        [Networked] private NetworkBool IsCarrying { get; set; }
        
        public ItemSizeType ItemSizeType => _itemSizeType;
        public string TestName => _testName;
        public NetworkRigidbody NetRigidbody => _rigidbody;

        protected override void CorrectState()
        {
            base.CorrectState();

            SetItemState(IsCarrying);
        }

        [Rpc]
        public virtual void RPC_ChangeState(bool isCarrying)
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