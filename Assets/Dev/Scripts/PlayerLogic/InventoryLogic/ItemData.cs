using System;
using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct ItemData : INetworkStruct
    {
        public ItemData(NetworkString<_16> itemName)
        {
            ItemName = itemName;
            UniquNumber = Guid.NewGuid();
        }
    
        [Networked]
        public NetworkString<_16> ItemName { get; set; }

        public Guid UniquNumber; // dont touch. its magic.

    }
}