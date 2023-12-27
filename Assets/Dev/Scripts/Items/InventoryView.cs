﻿using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.Levels.Interactions;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.Utils;
using DG.Tweening;
using Fusion;
using Fusion.KCC;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;  
        [SerializeField] private DragHandler _dragHandler;
        [SerializeField] private Transform _itemsParent;
        [SerializeField] private TriggerZone _craftTriggerZone;
        [SerializeField] private TriggerZone _dropItemZone;

        [SerializeField] private BoxCollider _spawnBox;
                
        private CraftStation _craftStation;
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<InventoryItemView> _itemViews = new List<InventoryItemView>();
        private ItemData[] _items;

        public Subject<string> ToRemoveItemFromInventory { get; } = new Subject<string>();

        private void Awake()
        {
            _craftTriggerZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnCraftZoneEntered));
            _dropItemZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnDropItemZoneEntered));
        }

      
        private void Start()
        {
            _craftStation = DependenciesContainer.Instance.GetDependency<CraftStation>();
            _itemStaticDataContainer = DependenciesContainer.Instance.GetDependency<ItemStaticDataContainer>();
            _craftStation.Crafted.TakeUntilDestroy(this).Subscribe((OnItemCrafted));
        }

        private void OnItemCrafted(bool isSuccess)
        {
            if (isSuccess)
            {
                foreach (InventoryItemView itemView in _craftStation.ItemViews)
                {
                    _itemViews.Remove(itemView);
                    Destroy(itemView.gameObject);
                }
                
                _craftStation.ItemViews.Clear();
            }
            else
            {
                foreach (InventoryItemView itemView in _craftStation.ItemViews)
                {
                    SendItemBack(itemView);
                }
            }
            
            //_craftStation.ItemViews.ForEach(x => x.transform.p);
        }

        public void Show(ItemData[] items)
        {
            _items = items;
            foreach (ItemData itemData in items)
            {
                string itemName = itemData.ItemName.Value;
             
                _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);

                Vector3 spawnPos = _spawnBox.bounds.RandomPointInBounds();
                InventoryItemView itemView = Instantiate(itemStaticData.InventoryData.Prefab, spawnPos, Quaternion.identity, _itemsParent);
                itemView.Setup(itemName);
                
                _itemViews.Add(itemView);
            }
            
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _dragHandler.SetActive(true);
        }

        public void SendItemBack(InventoryItemView itemView)
        {
            Vector3 pos = _spawnBox.bounds.RandomPointInBounds();

            itemView.DraggableObject.Rigidbody.position = pos;
        }
        
        public void Hide()  
        {
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _dragHandler.SetActive(false);
            
            _itemViews.ForEach(x => Destroy(x.gameObject));
            _itemViews.Clear();
            _items.Clear();
        }

        private void OnDropItemZoneEntered(Collider collider)
        {
            if (collider.TryGetComponent<InventoryItemView>(out var itemView))
            {
                string itemName = itemView.ItemName;
                
                ReturnWorldItemAlive(itemName);
                RemoveItemFromInventory(itemName);
            }    
        }

        private void RemoveItemFromInventory(string itemName)
        {
            InventoryItemView itemView = _itemViews.First(x => x.ItemName == itemName);
            Destroy(itemView.gameObject);
            _itemViews.Remove(itemView);

            ToRemoveItemFromInventory.OnNext(itemName);
        }

        private void ReturnWorldItemAlive(string itemName)
        {
            ItemData itemData = _items.First(x => x.ItemName.Value == itemName);

            NetworkId id = itemData.WorldItemId;

            var items = FindObjectsOfType<Item>(true);

            Item item = items.First(x => x.Object.Id == id);
                
            item.RPC_SetActive(true);
            item.RPC_ChangeState(false);
        }
        
        
        private void OnCraftZoneEntered(Collider collider)
        {
            if (collider.TryGetComponent<InventoryItemView>(out var itemView))
            {
                _craftStation.AddToCraftingTable(itemView.ItemName);
            }
        }
    }
}