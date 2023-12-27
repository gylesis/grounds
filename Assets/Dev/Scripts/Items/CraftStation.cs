using System.Collections.Generic;
using Dev.Infrastructure;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class CraftStation : NetworkContext
    {
        private List<string> _currentItemsToCraft = new List<string>(4);
        private List<InventoryItemView> _itemViews = new List<InventoryItemView>(4);

        public List<InventoryItemView> ItemViews => _itemViews;

        public Subject<bool> Crafted { get; } = new Subject<bool>();

        public void AddToCraftingTable(InventoryItemView itemView)
        {
            string itemName = itemView.ItemName;

            var addToCraftingTable = AddToCraftingTable(itemName);

            if (addToCraftingTable)
            {
                _itemViews.Add(itemView);
            }
        }
        
        public bool AddToCraftingTable(string item)
        {
            if (_currentItemsToCraft.Contains(item))
            {
                Debug.Log($"Item {item} already added to craft recipe!");
                return false;
            }
            Debug.Log($"Item {item} added to craft recipe");
           
            _currentItemsToCraft.Add(item);
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