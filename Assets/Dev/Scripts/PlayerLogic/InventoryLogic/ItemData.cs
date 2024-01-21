using System;
using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct ItemData : INetworkStruct
    {
        public ItemData(int itemId)
        {
            ItemId = itemId;
            UniquNumber = Guid.NewGuid();
        }
        
        [Networked]
        public int ItemId { get; private set; }
        
        public Guid UniquNumber; // dont touch. its magic.

    }
}