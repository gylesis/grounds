﻿using TMPro;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class InventoryItemUIView : DraggableUIElement
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TMP_Text _itemName;

        public RectTransform RectTransform => _rectTransform;
        public string ItemName { get; private set; }

        public void Setup(string itemName)
        {
            ItemName = itemName;
            _itemName.text = $"{itemName}";
        }
    }
}