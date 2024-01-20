using System.Collections.Generic;
using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using Unity.Collections;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Items
{
    public class ItemsDataService : NetworkContext
    {
        [SerializeField, ReadOnly] private List<ItemSpawnPlace> _itemSpawnPlaces;
        
        private ItemStaticDataContainer _itemStaticDataContainer;
        private DiContainer _diContainer;

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer, DiContainer diContainer)
        {
            _diContainer = diContainer;
            _itemStaticDataContainer = itemStaticDataContainer;
        }

        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                SpawnItemsOnSpawnPlaces();
            }
        }

        public void AddItemSpawnPlace(ItemSpawnPlace itemSpawnPlace)
        {
            _itemSpawnPlaces.Add(itemSpawnPlace);
        }
        
        public string GetItemNameById(int itemId)
        {
            bool hasData = _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var staticData);

            if (hasData)
            {
                return staticData.ItemName;
            }

            return "NONE";
        }
        
        private void SpawnItemsOnSpawnPlaces()
        {
            foreach (ItemSpawnPlace itemSpawnPlace in _itemSpawnPlaces)
            {
                int itemIdToSpawn = itemSpawnPlace.GetRandomItemNameToSpawn();
                SpawnItem(itemIdToSpawn, itemSpawnPlace.transform.position);
            }
        }
        
        public void RegisterItem(Item item)
        {
            /*if (item.ItemName == String.Empty)
            {
                _itemStaticDataContainer.Find(item);
            }*/
        }

        public Item SpawnItem(int itemId, Vector3 pos)
        {
            bool hasData = _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

            if (hasData == false)
            {
                Debug.Log($"Couldn't spawn item {itemId}, no data about it");
                return null;
            }

            Item item = Runner.Spawn(itemStaticData.WorldData.Prefab, pos, Quaternion.identity);

            _diContainer.Inject(item);
            item.Setup(itemStaticData.ItemId);

            return item;
        }
        
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ReturnItemToWorldFromInventory(int itemId)
        {   
            PlayerRef playerRef = Runner.LocalPlayer;

            GameObject playerGo = Runner.GetPlayerObject(playerRef).gameObject;
            Vector3 pos = playerGo.transform.position + playerGo.transform.forward * 1.5f + Vector3.up * 1.5f;
    
            SpawnItem(itemId, pos);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_RemoveItemFromWorld(Item item)
        {
            item.RPC_SetActive(false);
        }
        
        public ItemStaticData GetItemStaticData(string itemName)
        {
            _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);
            return itemStaticData;
        }
        
        public ItemStaticData GetItemStaticData(int itemId)
        {
            _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);
            return itemStaticData;
        }
        
    }
    
}