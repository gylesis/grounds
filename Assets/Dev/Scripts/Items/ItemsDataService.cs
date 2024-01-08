using System;
using System.Linq;
using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Dev.Scripts.Items
{
    public class ItemsDataService : NetworkContext
    {
        [SerializeField] private ItemSpawnPlace[] _itemSpawnPlaces;
        
        private ItemStaticDataContainer _itemStaticDataContainer;

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }

#if UNITY_EDITOR
        [Button]
        private void FindItemSpawnPlaces()
        {
            _itemSpawnPlaces = FindObjectsOfType<ItemSpawnPlace>();

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
#endif

        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
            {
                SpawnItemsOnSpawnPlaces();
            }
        }

        private void SpawnItemsOnSpawnPlaces()
        {
            foreach (ItemSpawnPlace itemSpawnPlace in _itemSpawnPlaces)
            {
                string itemNameToSpawn = itemSpawnPlace.GetRandomItemNameToSpawn();
                SpawnItem(itemNameToSpawn, itemSpawnPlace.transform.position);
            }
        }
        
        public void RegisterItem(Item item)
        {
            if (item.ItemName == String.Empty)
            {
                _itemStaticDataContainer.Find(item);
            }
        }

        public Item SpawnItem(string itemName, Vector3 pos)
        {
            bool hasData = _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);

            if (hasData == false)
            {
                Debug.Log($"Couldn't spawn item {itemName}, no data about it");
                return null;
            }

            Item item = Runner.Spawn(itemStaticData.WorldData.Prefab, pos, Quaternion.identity);

            item.Setup(itemStaticData.ItemNameTag.ItemName);

            return item;
        }
        
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_ReturnItemToWorldFromInventory(string itemName)
        {   
            PlayerRef playerRef = Runner.LocalPlayer;

            GameObject playerGo = Runner.GetPlayerObject(playerRef).gameObject;
            Vector3 pos = playerGo.transform.position + playerGo.transform.forward * 1.5f + Vector3.up * 1.5f;
    
            SpawnItem(itemName, pos);
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
        
    }
    
}