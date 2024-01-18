using Dev.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class QuickTab : UIElementBase
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private QuickTabReactiveButton _button;
        [SerializeField] private RectTransform _rectTransform;

        public RectTransform RectTransform => _rectTransform;

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