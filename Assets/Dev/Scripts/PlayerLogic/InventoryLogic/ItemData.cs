using System;
using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct ItemData : INetworkStruct
    {
        public ItemData(NetworkString<_16> itemNameNet)
        {
            ItemNameNet = itemNameNet;
            UniquNumber = Guid.NewGuid();
        }
    
        [Networked]
        public NetworkString<_16> ItemNameNet { get; set; }

        public string ItemName => ItemNameNet.Value;
        
        public Guid UniquNumber; // dont touch. its magic.

    }
}