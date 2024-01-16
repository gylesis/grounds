﻿using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class QuickTab : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private QuickTabReactiveButton _button;

        public QuickTabReactiveButton Button => _button;

        public int TabId { get; private set; }
        public void Setup(int tabId, Sprite itemSprite)
        {
            TabId = tabId;
            _itemIcon.sprite = itemSprite;
            _button.Setup(this);
        }

    }
}