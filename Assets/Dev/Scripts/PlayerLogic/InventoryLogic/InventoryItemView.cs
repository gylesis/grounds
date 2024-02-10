using Dev.Scripts.Items;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    [RequireComponent(typeof(DraggableObject))]
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private DraggableObject _draggableObject;
        
        private InventoryHandView _parentHandView;

        public DraggableObject DraggableObject => _draggableObject;
        public bool HasParent => _parentHandView != null;

        public InventoryHandView ParentHandView => _parentHandView;
        private void Reset()
        {
            _draggableObject = GetComponent<DraggableObject>();
        }

        public int ItemId { get; private set; }

        public void Setup(int itemId)
        {
            ItemId = itemId;
        }

        public void AssignHand(InventoryHandView handView)
        {
            _parentHandView = handView;
        }

        public bool IsInThatHand(InventoryHandView handView)
        {
            if (_parentHandView == null)
            {
                Debug.Log($"No parent Hand for this Inventory Item {ItemId}", gameObject);
                return false;
            }
            
            return handView.gameObject.GetInstanceID() == _parentHandView.gameObject.GetInstanceID();
        }
        
    }
}