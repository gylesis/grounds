using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct HandItemData : INetworkStruct
    {
        [Networked]
        public HandType HandType { get; private set; }

        [Networked]
        public ItemData ItemData { get; set; }

        public int ItemId => ItemData.ItemId;
        
        public HandItemData(ItemData itemData, HandType handType)
        {
            HandType = handType;
            ItemData = itemData;
        }
        
    }
}