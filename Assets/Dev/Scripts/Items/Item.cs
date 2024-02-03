using System;
using Dev.Scripts.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Items
{
    [SelectionBase]
    [OrderAfter(typeof(Hands))]
    [RequireComponent(typeof(GameObjectContext))]
    public class Item : NetworkContext
    {
        [SerializeField] private HitboxRoot _hitboxRoot;
        [SerializeField] private NetworkRigidbody _rigidbody;
        [SerializeField] private Collider _collider;
        [SerializeField] private Health _health;
        [SerializeField] private ItemSizeType _itemSizeType;
        [SerializeField] private ItemNameTag _itemNameTag;
        [SerializeField] private GameObjectContext _gameObjectContext;
        [ReadOnly][SerializeField] private int _itemId;
        [SerializeField] private ItemView _itemView;
        
        
        public GameObjectContext GameObjectContext => _gameObjectContext;

        private Subject<Unit> _useAction = new();
        private ItemDynamicData _itemDynamicData = new();
        private IDisposable _disposable;
        private ItemsDataService _itemsDataService;
        [Networked] private NetworkBool IsCarrying { get; set; }

        public int ItemId => _itemNameTag.ItemId;
        public ItemSizeType ItemSizeType => _itemSizeType;
        public NetworkRigidbody NetRigidbody => _rigidbody;
        public Health Health => _health;
        public ItemDynamicData ItemDynamicData => _itemDynamicData;

        [Inject]
        private void Construct(ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
        }

        public void Setup(int itemId)
        {
            _itemId = itemId;
        }

        private void Start()
        {
            if (_itemsDataService == null) // true only if object not spawned in ItemsDataService and already was on scene
            {
                DiContainerSingleton.Instance.Inject(_gameObjectContext);
            }
            
            _itemView.Initialize(this);
        }

        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();
            
            _itemsDataService.RegisterItem(this);
        }

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
            else { }

            _rigidbody.Rigidbody.isKinematic = isCarrying;
            _rigidbody.enabled = !isCarrying;
            _collider.enabled = !isCarrying;
            _hitboxRoot.enabled = !isCarrying;
        }

        [Button]
        private void UpdatePositionDataInHand()
        {
            ItemStaticData itemStaticData = _itemsDataService.GetItemStaticData(ItemId);
            
            itemStaticData.PositionInHand = transform.localPosition;
            itemStaticData.RotationInHand = transform.localRotation.eulerAngles;
            
            _itemsDataService.SaveItemStaticDataContainer();
        }
    }
}