using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class ItemUISlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _itemName;
        [SerializeField] private Image _itemImg;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Transform _itemUiViewParent;

        public Transform ItemUiViewParent => _itemUiViewParent;

        private ItemUISlotData _slotData;
        private InventoryItemView _itemView;

        public InventoryItemView ItemView => _itemView;


        public ItemUISlotData SlotData => _slotData;

        public string ItemName => _slotData.ItemName;
        public string ItemDescription => _slotData.ItemDescription;
        public RectTransform RectTransform => _rectTransform;

        public bool IsFree { get; private set; } = true;
        
        public void AssignItem(ItemUISlotData slotData, InventoryItemView itemView)
        {
            _itemView = itemView;
            _slotData = slotData;
            _itemImg.sprite = slotData.Sprite;
            _itemName.text = $"{slotData.ItemName}";

            IsFree = false;
        }

        public void FreeSlot()
        {
            _itemView = null;
            IsFree = true;
            _slotData.ItemName = String.Empty;
            _itemName.text = "-";
        }
        
    }
}