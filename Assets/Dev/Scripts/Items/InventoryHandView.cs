using Dev.Scripts.LevelLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Items
{
    public class InventoryHandView : MonoBehaviour
    {   
        [SerializeField] private Transform _hand;
        [SerializeField] private TriggerZone _triggerZone;
        [SerializeField] private bool _isLeftHand;
            
        private InventoryItemsDragHandler _inventoryItemsDragHandler;
        private InventoryItemView _holdingItemView;
        private InventoryItemView _potentialItemView;

        public InventoryItemView HoldingItemView => _holdingItemView;
        public InventoryItemView PotentialItemView => _potentialItemView;

        public bool IsLeftHand => _isLeftHand;
        public bool IsHandBusy => _holdingItemView != null;

        public Subject<HandChangedEventContext> ItemChanged { get; } = new Subject<HandChangedEventContext>();

        public struct HandChangedEventContext
        {
            public int ItemId;
            public bool ToRemoveFromHand;
        }

        private void Awake()
        {
            _triggerZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnItemViewEntered));
            _triggerZone.TriggerExit.TakeUntilDestroy(this).Subscribe(((OnItemViewExit)));
        }

        [Inject]
        private void Init(InventoryItemsDragHandler inventoryItemsDragHandler)
        {
            _inventoryItemsDragHandler = inventoryItemsDragHandler;
        }

        private void OnItemViewEntered(Collider collider)
        {
            if (_inventoryItemsDragHandler.IsDragging == false) return;
            
            if (collider.TryGetComponent<InventoryItemView>(out var itemView) == false) return;
                
            Debug.Log($"Potential item view entered {itemView.name}");
            _potentialItemView = itemView;
        }

        private void OnItemViewExit(Collider collider)
        {
            if (_inventoryItemsDragHandler.IsDragging == false) return;

            if (collider.TryGetComponent<InventoryItemView>(out var itemView) == false) return;

            if(_potentialItemView == null) return;
            
            if (itemView.gameObject.GetInstanceID() != _potentialItemView.gameObject.GetInstanceID()) return;
            
            Debug.Log($"Potential item view exit {itemView.name}");

            _potentialItemView = null;
        }

        public void PutItem(InventoryItemView itemView, bool isInitialization = false)
        {
            Debug.Log($"Put item {itemView.ItemId} to {name} inventory hand");

            _holdingItemView = itemView;
            itemView.transform.position = _hand.transform.position;
            itemView.DraggableObject.SetFreezeState(true);

            _holdingItemView.AssignHand(this);
            
            if(isInitialization) return;
            
            var handChangedEventContext = new HandChangedEventContext();
            handChangedEventContext.ItemId = itemView.ItemId;
            handChangedEventContext.ToRemoveFromHand = false;
            
            ItemChanged.OnNext(handChangedEventContext);
        }

        public void FreeHand()
        {
            var itemName = _holdingItemView.ItemId;

            Debug.Log($"Free inventory hand {gameObject.name}");
           
            _holdingItemView.DraggableObject.SetFreezeState(false);

            _holdingItemView.AssignHand(null);
            _holdingItemView = null;
            
            var handChangedEventContext = new HandChangedEventContext();
            handChangedEventContext.ItemId = itemName;
            handChangedEventContext.ToRemoveFromHand = true;
            
            ItemChanged.OnNext(handChangedEventContext);
        }
        
        public void PullItemToHand(InventoryItemView itemView)
        {
            itemView.transform.position = _hand.transform.position;
            itemView.DraggableObject.SetFreezeState(true);
        }
        
    }
}