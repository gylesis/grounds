using Dev.Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class QuickTab : UIElementBase
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform;

        public int ItemId { get; private set; }

        public void Setup(Sprite itemSprite, int itemId)
        {
            ItemId = itemId;
            _itemIcon.sprite = itemSprite;
        }

    }
}