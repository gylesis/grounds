using System;
using System.Collections.Generic;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public struct QuickMenuSetupContext
    {
        public Action<int> TabChosen;
        public int TabsCount => Items.Count;
        public List<ItemData> Items;
    }
}