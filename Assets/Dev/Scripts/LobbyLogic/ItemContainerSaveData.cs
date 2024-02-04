using System;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UnityEngine;

namespace Dev.Scripts.LobbyLogic
{
    [Serializable]
    public class ItemContainerSaveData
    {
        public Vector3 Pos;
        public ItemData[] Items;
    }
}