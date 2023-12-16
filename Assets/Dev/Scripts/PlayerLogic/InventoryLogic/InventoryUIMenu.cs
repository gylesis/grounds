using System.Collections.Generic;
using Dev.UI.PopUpsAndMenus;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class InventoryUIMenu : PopUp
    {
        [SerializeField] private InventoryItemUIView _inventoryItemUIViewPrefab;
        [SerializeField] private Transform _itemsUIParent;

        private List<InventoryItemUIView> _itemUIViews = new List<InventoryItemUIView>();

        public void UpdateItemsData(List<ItemData> items)
        {
            _itemUIViews.ForEach(x => Destroy(x.gameObject));
            _itemUIViews.Clear();
            
            foreach (ItemData itemData in items)
            {
                InventoryItemUIView itemUIView = Instantiate(_inventoryItemUIViewPrefab, _itemsUIParent);

                itemUIView.Setup(itemData.ItemName.Value);
                _itemUIViews.Add(itemUIView);
            }
            
        }
    }
}