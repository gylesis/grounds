using System.Collections.Generic;
using Dev.Scripts.Items;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    [CreateAssetMenu(menuName = "StaticData/ItemStaticData", fileName = "ItemStaticData", order = 0)]
    public class ItemStaticData : ScriptableObject
    {
        [SerializeField] private ItemNameTag _itemNameTag;
        
        public ItemNameTag ItemNameTag => _itemNameTag;

        public string ItemName => _itemNameTag.ItemName;
        public int ItemId => _itemNameTag.ItemId;
        
        public List<ItemType> ItemTypes => _itemTypes;
        
        public Sprite ItemIcon;
        public string ItemDescription;

        [SerializeField] private List<ItemType> _itemTypes;

        public ItemWorldData WorldData;
        public ItemInventoryStaticData InventoryData;
        
        public Vector3 PositionInHand;
        public Vector3 RotationInHand;
    }
}
