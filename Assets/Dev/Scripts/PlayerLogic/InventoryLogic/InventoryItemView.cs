using Dev.Scripts.Items;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    [RequireComponent(typeof(DraggableObject))]
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private DraggableObject _draggableObject;

        public DraggableObject DraggableObject => _draggableObject;

        private void Reset()
        {
            _draggableObject = GetComponent<DraggableObject>();
        }

        public int ItemId { get; private set; }

        public void Setup(int itemId)
        {
            ItemId = itemId;
        }
        
    }
}