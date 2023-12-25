using System;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    [SelectionBase]
    [OrderAfter(typeof(Hands))]
    public class Item : NetworkContext
    {
        [SerializeField] private HitboxRoot _hitboxRoot;
        [SerializeField] private NetworkRigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private Health _health;
        [SerializeField] private string _testName = "Good Item 12312";
        [SerializeField] private ItemSizeType _itemSizeType;
        [SerializeField] private ItemEnumeration _itemEnumeration;
        [SerializeField] private ItemStaticData _itemStaticData;
        
        [Networked] private NetworkBool IsCarrying { get; set; }

        private Subject<Unit> _useAction = new();
        private ItemDynamicData _itemDynamicData = new();
        private IDisposable _disposable;

        public ItemSizeType ItemSizeType => _itemSizeType;
        public string TestName => _testName;
        public NetworkRigidbody NetRigidbody => _rigidbody;
        public ItemEnumeration ItemEnumeration => _itemEnumeration;
        public Health Health => _health;
        public ItemStaticData ItemStaticData => _itemStaticData;
        public ItemDynamicData ItemDynamicData => _itemDynamicData;

        
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

        public void Use()
        {
            _useAction.OnNext(Unit.Default);
        }

        public void UpdateUseAction(Action action)
        {
            _disposable?.Dispose();
            _disposable = _useAction.Subscribe(unit => action.Invoke());
        }

        public void SetLastOwner(PlayerCharacter owner)
        {
            _itemDynamicData.UpdateOwner(owner);
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

        [Button]
        private void UpdatePositionDataInHand()
        {
            _itemStaticData.PositionInHand = transform.localPosition;
            _itemStaticData.RotationInHand = transform.localRotation.eulerAngles;
        }
    }
    
    
}