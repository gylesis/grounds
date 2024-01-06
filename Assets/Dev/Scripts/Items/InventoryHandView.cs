using Dev.Levels.Interactions;
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
            Debug.Log($"iS hand busy {IsHandBusy}");
            
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
                
            Debug.Log($"Potential item view entered {itemView.ItemName}");
            _potentialItemView = itemView;
        }

        private void OnItemViewExit(Collider collider)
        {
            if (_inventoryItemsDragHandler.IsDragging == false) return;

            if (collider.TryGetComponent<InventoryItemView>(out var itemView) == false) return;

            if (itemView != _potentialItemView) return;
            
            Debug.Log($"Potential item view exit {itemView.ItemName}");

            _potentialItemView = null;
        }

        public void PutItem(InventoryItemView itemView)
        {
            Debug.Log($"Put item {itemView.ItemName} to {name} hand");

            _holdingItemView = itemView;
            itemView.transform.position = _hand.transform.position;
            itemView.DraggableObject.SetFreezeState(true);
        }

        public void FreeHand()
        {
            Debug.Log($"{name} free hand");
            _holdingItemView.DraggableObject.SetFreezeState(false);
            _holdingItemView = null;
        }
        
        
        public void PullItemToHand(InventoryItemView itemView)
        {
        }
        
    }
}