using TMPro;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class InventoryItemUIView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _itemName;

        public void Setup(string itemName)
        {
            _itemName.text = $"{itemName}";
        }
        
    }
}