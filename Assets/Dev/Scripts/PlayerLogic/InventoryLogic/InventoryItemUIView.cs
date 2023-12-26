using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;

        public string ItemName { get; private set; }

        public void Setup(string itemName)
        {
            ItemName = itemName;
        }
        
    }
}