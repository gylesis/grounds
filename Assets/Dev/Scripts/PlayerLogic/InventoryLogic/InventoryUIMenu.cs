using System.Collections.Generic;
using Dev.UI.PopUpsAndMenus;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class InventoryUIMenu : PopUp
    {
        [SerializeField] private InventoryItemView _inventoryItemViewPrefab;
        [SerializeField] private BoxCollider _inventorySpawnBox;
        [SerializeField] private Transform _itemsParent;
        [SerializeField] private Vector3 _itemSpawnPos;
        
        private List<InventoryItemView> _itemUIViews = new List<InventoryItemView>();
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<ItemData> _itemDatas = new List<ItemData>();

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }

        public void UpdateItemsData(List<ItemData> items)
        {
            _itemUIViews.ForEach(x => Destroy(x.gameObject));
            _itemUIViews.Clear();

            foreach (ItemData itemData in items)
            {
                InventoryItemView itemView = Instantiate(_inventoryItemViewPrefab, _itemSpawnPos, Quaternion.identity,_itemsParent );

                itemView.Setup(itemData.ItemName.Value);

                _itemUIViews.Add(itemView);
            }

            _itemDatas = items;
        }

    }
    
}