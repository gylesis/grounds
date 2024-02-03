using System.Collections.Generic;
using Dev.Scripts.Infrastructure;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class CraftStation : NetworkContext
    {
        private List<int> _currentItemsToCraft = new List<int>(4);
        private List<InventoryItemView> _itemViews = new List<InventoryItemView>(4);

        public List<InventoryItemView> ItemViews => _itemViews;

        public Subject<bool> Crafted { get; } = new Subject<bool>();

        public void AddToCraftingTable(InventoryItemView itemView)
        {
            int itemId = itemView.ItemId;

            var addToCraftingTable = AddToCraftingTable(itemId);
    
            if (addToCraftingTable)
            {
                _itemViews.Add(itemView);
            }
        }   
        
        public bool AddToCraftingTable(int itemId)
        {
            if (_currentItemsToCraft.Contains(itemId))
            {
                Debug.Log($"Item {itemId} already added to craft recipe!");
                return false;
            }
            Debug.Log($"Item {itemId} added to craft recipe");
           
            _currentItemsToCraft.Add(itemId);
            return true;
        }

        public void Craft()
        {
            if (_currentItemsToCraft.Count == 0)
            {
                Debug.Log($"Nothing to craft, add items to Craft Station");
                
                Crafted.OnNext(false);
                return;
            }
            // recipe check

            Debug.Log($"Crafted something");
            
            Crafted.OnNext(true);
            
            _currentItemsToCraft.Clear();
        }
        
    }
}