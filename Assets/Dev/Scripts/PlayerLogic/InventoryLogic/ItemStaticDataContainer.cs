using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    [CreateAssetMenu(menuName = "StaticData/ItemStaticDataContainer", fileName = "ItemStaticDataContainer", order = 0)]
    public class ItemStaticDataContainer : ScriptableObject
    {
        [SerializeField] private ItemStaticData[] _items;

        public bool TryGetItemStaticDataByName(string itemName, out ItemStaticData itemStaticData)
        {
            itemStaticData = _items.FirstOrDefault(x => x.ItemName == itemName);

            return itemStaticData != null;
        }

        public bool IsItemThisTypeof(string itemName, params ItemType[] itemTypes)
        {
            ItemStaticData staticData = _items.FirstOrDefault(x => x.ItemName == itemName);

            if (staticData != null)
            {
                foreach (ItemType itemType in itemTypes)
                {
                    var contains = staticData.ItemTypes.Contains(itemType);

                    if (contains == false) return false;
                }

                return true;
            }
            
            return false;
        }
        
        public bool TryGetItemTypes(string itemName, out ItemType[] itemTypes)
        {
            ItemStaticData staticData = _items.FirstOrDefault(x => x.ItemName == itemName);

            itemTypes = null;
            
            if (staticData != null)
            {
                itemTypes = new List<ItemType>(staticData.ItemTypes).ToArray();
            }

            return staticData != null;
        }
        
        public void Find(Item itemInstance)
        {
            int hashCode = itemInstance.GetHashCode();

            Debug.Log($"Target item hash {hashCode}");
                
            foreach (ItemStaticData data in _items)
            {
                Debug.Log($"Data {data.ItemName} hash {data.WorldData.Prefab.GetHashCode()}");
            }
        }

        [Button(ButtonSizes.Gigantic,Name = "Find items data and Fill")]
        private void RefreshList()
        {
            string path = "Assets/Dev/SO/Items/ItemsData/";

            var itemStaticDatas = AssetDatabase.FindAssets("t:ItemStaticData", new []{ path});

            var staticDatas = new List<ItemStaticData>(itemStaticDatas.Length);

            foreach (var assetPath in itemStaticDatas)
            {
                ItemStaticData itemStaticData = AssetDatabase.LoadAssetAtPath<ItemStaticData>(AssetDatabase.GUIDToAssetPath(assetPath));
                staticDatas.Add(itemStaticData);
            }

            _items = staticDatas.ToArray();
            
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            
        }
        
    }   
}