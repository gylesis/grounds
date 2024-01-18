using System;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField] private ItemSizeType _itemSizeType;
        [SerializeField] private ItemEnumeration _itemEnumeration;
        [SerializeField] private ItemNameTag _itemNameTag;
        
        [Networked]
        private NetworkString<_16> itemName { get; set; }
        
        private Subject<Unit> _useAction = new();
        private ItemDynamicData _itemDynamicData = new();
        private IDisposable _disposable;
        private ItemsDataService _itemsDataService;
        [Networked] private NetworkBool IsCarrying { get; set; }

        public ItemSizeType ItemSizeType => _itemSizeType;
        public NetworkRigidbody NetRigidbody => _rigidbody;
        public ItemEnumeration ItemEnumeration => _itemEnumeration;
        public Health Health => _health;
        public ItemDynamicData ItemDynamicData => _itemDynamicData;

       // public string ItemName => _itemNameTag.ItemName;
        public string ItemName => itemName.Value;

        private void Start()
        {
            _itemsDataService = DependenciesContainer.Instance.GetDependency<ItemsDataService>();
            _itemsDataService.RegisterItem(this);
        }

        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                if (itemName == String.Empty)
                {
                    itemName = _itemNameTag.ItemName;
                }
            }
            
        }

        protected override void CorrectState()
        {
            base.CorrectState();

            SetItemState(IsCarrying);
        }

        public void Setup(ItemNameTag itemNameTag)
        {
            _itemNameTag = itemNameTag;
        }
        
        public void Setup(string itemName)
        {
            this.itemName = itemName;
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
            else { }

            _rigidbody.Rigidbody.isKinematic = isCarrying;
            _rigidbody.enabled = !isCarrying;
            _collider.enabled = !isCarrying;
            _hitboxRoot.enabled = !isCarrying;
        }

        [Button]
        private void UpdatePositionDataInHand()
        {
            ItemStaticData itemStaticData = _itemsDataService.GetItemStaticData(ItemName);
            
            itemStaticData.PositionInHand = transform.localPosition;
            itemStaticData.RotationInHand = transform.localRotation.eulerAngles;
        }
    }
}