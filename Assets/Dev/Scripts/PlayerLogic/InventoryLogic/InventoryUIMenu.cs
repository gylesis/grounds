using System.Collections.Generic;
using System.Linq;
using Dev.UI.PopUpsAndMenus;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class InventoryUIMenu : PopUp
    {
        [SerializeField] private InventoryItemUIView _inventoryItemUIViewPrefab;
        [SerializeField] private Transform _firstRenderParent;
    
        [SerializeField] private ItemUISlot _leftHandSlot;
        [SerializeField] private ItemUISlot _rightHandSlot;

        [SerializeField] private ItemUISlot[] _slots;
        
        private List<InventoryItemUIView> _itemUIViews = new List<InventoryItemUIView>();
        private ItemStaticDataContainer _itemStaticDataContainer;
        private ItemUISlot _targetSlot;

        private ItemUISlot[] CenterSlots => _slots.Where(x => x != _leftHandSlot || x != _rightHandSlot).ToArray();

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }
        
        public void UpdateItemsData(List<ItemData> items)
        {
            _itemUIViews.ForEach(x => Destroy(x.gameObject));
            _itemUIViews.Clear();
            
            foreach (ItemData itemData in items)
            {
                ItemUISlot itemUISlot = CenterSlots.First(x => x.IsFree);
                
                Transform parent = itemUISlot.ItemUiViewParent;              
                InventoryItemUIView itemUIView = Instantiate(_inventoryItemUIViewPrefab, parent);

                itemUIView.PointerDown.TakeUntilDestroy(this).Subscribe((eventData => OnItemDown(eventData, itemUIView)));
                itemUIView.Drag.TakeUntilDestroy(this).Subscribe((eventData => OnItemDrag(eventData, itemUIView)));
                itemUIView.PointerUp.TakeUntilDestroy(this).Subscribe((eventData => OnItemUp(eventData, itemUIView)));
                
                itemUIView.Setup(itemData.ItemName.Value);

                PutItemInSlot(itemUIView, itemUISlot);
                
                _itemUIViews.Add(itemUIView);
            }
            
        }

        private void OnItemDown(PointerEventData eventData, InventoryItemUIView itemUIView)
        {
            _targetSlot = GetSlot(itemUIView.ItemName);
            itemUIView.transform.parent = _firstRenderParent;
        }

        private void OnItemDrag(PointerEventData eventData, InventoryItemUIView itemUIView)
        {
            itemUIView.RectTransform.position = eventData.position;
        }

        private void OnItemUp(PointerEventData eventData, InventoryItemUIView itemUIView)
        {
            string itemName = itemUIView.ItemName;

            bool successTransferedItemToSlot = false;   
            
            itemUIView.RectTransform.position = eventData.position;
            
            foreach (ItemUISlot itemUISlot in _slots)
            {
                var containsScreenPoint = RectTransformUtility.RectangleContainsScreenPoint(itemUISlot.RectTransform, eventData.position);

                if (containsScreenPoint)
                {
                    if (itemUISlot == _rightHandSlot) // правая рука
                    {
                        
                    }
                    else if (itemUISlot == _leftHandSlot) // левая рука
                    {
                        
                    }
                    else // обычный слот в инвентаре
                    {
                        
                    }

                    if (PutItemInSlot(itemUIView, itemUISlot))
                    {
                        successTransferedItemToSlot = true;
                        GetSlot(itemName).FreeSlot();
                    }
                    
                    break;
                }
                
            }


            if (successTransferedItemToSlot == false)
            {
                ReturnItemToTargetSlot(itemUIView);
            }
            else
            {
                _targetSlot = null;
            }

        }

        private ItemUISlot GetSlot(string itemName)
        {
            return _slots.FirstOrDefault(x => x.ItemName == itemName);
        }

        public bool IsSlotFree(ItemUISlot slot)
        {
            return slot.IsFree;
        }

        private void ReturnItemToTargetSlot(InventoryItemUIView itemUIView)
        {
            itemUIView.transform.parent = _targetSlot.ItemUiViewParent;
            itemUIView.RectTransform.localPosition = Vector3.zero;
        }

        private bool PutItemInSlot(InventoryItemUIView itemUIView, ItemUISlot slot)
        {
            string itemName = itemUIView.ItemName;

            bool isSlotBusy = IsSlotFree(slot) == false;
            
            if (isSlotBusy)
            {   
                Debug.Log("Slot not free, return item back");
                ReturnItemToTargetSlot(itemUIView);
                return false;
            }

            bool hasItemStaticData = _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);

            if (hasItemStaticData)
            {
                var itemUISlotData = new ItemUISlotData();

                itemUISlotData.ItemName = itemStaticData.ItemName;
                itemUISlotData.ItemDescription = itemStaticData.ItemDescription;
                itemUISlotData.Sprite = itemStaticData.UIIcon;
                
                slot.AssignItem(itemUISlotData);

                itemUIView.transform.parent = slot.ItemUiViewParent;
                itemUIView.RectTransform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.Log($"No item {itemName} in item container");
            }

            return hasItemStaticData;
        }

    }
}