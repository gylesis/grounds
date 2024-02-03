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

        private InventoryItemsDragHandler _inventoryItemsDragHandler;
        private InventoryItemView _holdingItemView;
        private InventoryItemView _potentialItemView;

        public bool IsHandBusy => _holdingItemView != null;

        public Subject<HandChangedEventContext> ItemChanged { get; } = new Subject<HandChangedEventContext>();

        public struct HandChangedEventContext
        {
            public int ItemId;
            public bool ToRemove;
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

        private void Start()
        {
            _inventoryItemsDragHandler.DraggableObjectUp.TakeUntilDestroy(_inventoryItemsDragHandler)
                .Subscribe((OnItemDragUp));
            
            _inventoryItemsDragHandler.DraggableObjectDown.TakeUntilDestroy(_inventoryItemsDragHandler)
                .Subscribe((OnItemDragDown));
        }

        private void OnItemDragDown(DraggableObject draggableObject)
        {
            return;
            if(IsHandBusy == false) return;

            if(_potentialItemView == null) return;
        }

        private void OnItemDragUp(DraggableObject draggableObject)
        {
            //Debug.Log($"iS hand busy {IsHandBusy}");
            
            if (IsHandBusy)
            {
                if (_holdingItemView.gameObject.GetInstanceID() != draggableObject.gameObject.GetInstanceID()) return;
                
                FreeHand();
            }
            else
            {
                if (_potentialItemView == null) return;
                
                PutItem(_potentialItemView);
            }
        }

        private void OnItemViewEntered(Collider collider)
        {
            if (_inventoryItemsDragHandler.IsDragging == false) return;
            
            if (collider.TryGetComponent<InventoryItemView>(out var itemView) == false) return;
                
           // Debug.Log($"Potential item view entered {itemView.ItemName}");
            _potentialItemView = itemView;
        }

        private void OnItemViewExit(Collider collider)
        {
            if (_inventoryItemsDragHandler.IsDragging == false) return;

            if (collider.TryGetComponent<InventoryItemView>(out var itemView) == false) return;

            if (itemView != _potentialItemView) return;
            
           // Debug.Log($"Potential item view exit {itemView.ItemName}");

            _potentialItemView = null;
        }

        public void PutItem(InventoryItemView itemView, bool isInitialization = false)
        {
            Debug.Log($"Put item {itemView.ItemId} to {name} hand");

            _holdingItemView = itemView;
            itemView.transform.position = _hand.transform.position;
            itemView.DraggableObject.SetFreezeState(true);

            if(isInitialization) return;
            
            var handChangedEventContext = new HandChangedEventContext();
            handChangedEventContext.ItemId = itemView.ItemId;
            handChangedEventContext.ToRemove = false;
            
            ItemChanged.OnNext(handChangedEventContext);
        }

        public void FreeHand()
        {
            var itemName = _holdingItemView.ItemId;

            _holdingItemView.DraggableObject.SetFreezeState(false);
            _holdingItemView = null;
            
            var handChangedEventContext = new HandChangedEventContext();
            handChangedEventContext.ItemId = itemName;
            handChangedEventContext.ToRemove = true;
            
            ItemChanged.OnNext(handChangedEventContext);
        }
        
        public void PullItemToHand(InventoryItemView itemView)
        {
        }
        
    }
}