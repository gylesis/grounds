using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct InventoryData : INetworkStruct
    {
        [Networked]
        public PlayerRef Player { get; set; }

        [Networked, Capacity(20)] public NetworkLinkedList<ItemData> Items => default;

        public InventoryData(PlayerRef playerRef)
        {
            Player = playerRef;
        }
        
    }
}