﻿using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    [CreateAssetMenu(menuName = "StaticData/ItemStaticData", fileName = "ItemStaticData", order = 0)]
    public class ItemStaticData : ScriptableObject
    {
        public string ItemName;
        public string ItemDescription;
        public Sprite UIIcon;
    }
}