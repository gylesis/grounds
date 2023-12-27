using System.Linq;
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

        public void Find(Item itemInstance)
        {
            int hashCode = itemInstance.GetHashCode();

            Debug.Log($"Target item hash {hashCode}");
                
            foreach (ItemStaticData data in _items)
            {
                Debug.Log($"Data {data.ItemName} hash {data.WorldData.Prefab.GetHashCode()}");
            }
        }
        
    }   
}