﻿using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct ItemData : INetworkStruct
    {
        public ItemData(NetworkString<_16> itemName, NetworkId worldItemId)
        {
            WorldItemId = worldItemId;
            ItemName = itemName;
        }

        [Networked]
        public NetworkString<_16> ItemName { get; set; }
        
        public NetworkId WorldItemId { get; set; }
        
    }
}