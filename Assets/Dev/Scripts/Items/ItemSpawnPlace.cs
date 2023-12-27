using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dev.Scripts.Items
{
    public class ItemSpawnPlace : MonoBehaviour
    {
        [SerializeField] private ItemNameTag[] _itemsToSpawn;
        
        private ItemsDataService _itemsDataService;
        [SerializeField] private ItemStaticDataContainer _itemStaticDataContainer;

        private void Awake()
        {
            _itemsDataService = DependenciesContainer.Instance.GetDependency<ItemsDataService>();
          //  _itemStaticDataContainer = DependenciesContainer.Instance.GetDependency<ItemStaticDataContainer>();
        }

        #if UNITY_EDITOR
        /*private void OnValidate()
        {
            if(_itemStaticDataContainer == null)
                _itemStaticDataContainer = Resources.Load<ItemStaticDataContainer>("Assets/Dev/SO/Items/ItemStaticDataContainer.asset");
        }*/

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

                    Handles.Label(transform.position + Vector3.up * (1.5f * index + 1.5f), $"{itemName}", guiStyle);
                }
            }
        }
        #endif
        
    }
}