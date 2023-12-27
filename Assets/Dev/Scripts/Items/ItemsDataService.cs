using System;
using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Items
{
    public class ItemsDataService : NetworkContext
    {
        private ItemStaticDataContainer _itemStaticDataContainer;

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }

        public void RegisterItem(Item item)
        {
            if (item.ItemName == String.Empty)
            {
                _itemStaticDataContainer.Find(item);
            }
        }

        public void SpawnItem(string itemName, Vector3 pos)
        {
            bool hasData = _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);

            if (hasData == false)
            {
                Debug.Log($"Couldn't spawn item {itemName}, no data about it");
                return;
            }

            Item item = Runner.Spawn(itemStaticData.WorldData.Prefab, pos, Quaternion.identity);

            item.Setup(itemName);
        }

        public ItemStaticData GetItemStaticData(string itemName)
        {
            _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);
            return itemStaticData;
        }
        
    }
}