using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dev.Scripts.Items
{
    [CreateAssetMenu(menuName = "StaticData/Items/ItemNameTag", fileName = "ItemNameTag", order = 0)]
    public class ItemNameTag : ScriptableObject
    {
        [SerializeField] private string _itemName;
        [ReadOnly][SerializeField] private int _itemId = -1;

        public int ItemId => _itemId;

        public string ItemName => _itemName;

        private void Awake()
        {
            TryCreateItemId();
        }

        private void TryCreateItemId()
        {
            if (_itemId == -1)
            {
                int hashCode = Guid.NewGuid().GetHashCode();
                _itemId = hashCode;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }
    }
}