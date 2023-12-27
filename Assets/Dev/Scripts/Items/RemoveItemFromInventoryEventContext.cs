using Fusion;

namespace Dev.Scripts.Items
{
    public struct RemoveItemFromInventoryEventContext
    {
        public PlayerRef ItemOwner;
        public string ItemName;
    }
}