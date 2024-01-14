﻿using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dev.Scripts.Items
{
    [SelectionBase]
    public class ItemSpawnPlace : MonoBehaviour
    {
        [SerializeField] private ItemStaticDataContainer _itemStaticDataContainer;
        [SerializeField] private ItemNameTag[] _itemsToSpawn;

        private ItemsDataService _itemsDataService;
       
        private void Awake()
        {
            _itemsDataService = DependenciesContainer.Instance.GetDependency<ItemsDataService>();
        }

        public string GetRandomItemNameToSpawn()
        {
            return _itemsToSpawn[Random.Range(0, _itemsToSpawn.Length)].ItemName;
        }
        
        #if UNITY_EDITOR
       
        private void OnDrawGizmos()
        {
            for (var index = 0; index < _itemsToSpawn.Length; index++)
            {
                ItemNameTag nameTag = _itemsToSpawn[index];
                string itemName = nameTag.ItemName;

                var hasData = _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var staticData);

                if (hasData)
                {
                    var guiStyle = new GUIStyle();
                    guiStyle.fontSize = 15;
                    guiStyle.padding = new RectOffset(5, 5, 5, 5);
                    guiStyle.richText = true;
                    guiStyle.fontStyle = FontStyle.Bold;

                    Vector3 pos = transform.position + Vector3.up * (1.2f * index + 1.5f);

                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(pos, 0.4f);
                    
                    Handles.Label(pos, $"{itemName}", guiStyle);
                }
            }
        }
        #endif
        
    }
}