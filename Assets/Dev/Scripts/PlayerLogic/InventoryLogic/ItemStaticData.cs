using Dev.Scripts.Items;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    [CreateAssetMenu(menuName = "StaticData/ItemStaticData", fileName = "ItemStaticData", order = 0)]
    public class ItemStaticData : ScriptableObject
    {
        [SerializeField] private ItemNameTag _itemNameTag;

        public Sprite ItemIcon;
        
        public ItemNameTag ItemNameTag => _itemNameTag;

        public string ItemName => _itemNameTag.ItemName;
        
        public string ItemDescription;

        public ItemWorldData WorldData;
        public ItemInventoryStaticData InventoryData;
        
        public Vector3 PositionInHand;
        public Vector3 RotationInHand;
    }
}