using Fusion;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct InventoryData : INetworkStruct
    {
        [Networked]
        public PlayerRef Player { get; set; }

        [Networked, Capacity(20)] public NetworkLinkedList<ItemData> InventoryItems => default;
        [Networked, Capacity(2)] public NetworkLinkedList<HandItemData> HandItems => default;
        
        public InventoryData(PlayerRef playerRef)
        {
            Player = playerRef;
        }   
        
    }
}