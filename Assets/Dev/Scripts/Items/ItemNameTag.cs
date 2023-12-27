using UnityEngine;

namespace Dev.Scripts.Items
{
    [CreateAssetMenu(menuName = "StaticData/Items/ItemNameTag", fileName = "ItemNameTag", order = 0)]
    public class ItemNameTag : ScriptableObject
    {
        [SerializeField] private string _itemName;

        public string ItemName => _itemName;
    }
}