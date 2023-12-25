using System.Collections.Generic;
using System.Linq;
using Dev.UI.PopUpsAndMenus;
using Sirenix.Utilities;
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

        private ItemUISlot[] CenterSlots => _slots.Where(x =>
                x.GetInstanceID() != _leftHandSlot.GetInstanceID() ||
                x.GetInstanceID() != _rightHandSlot.GetInstanceID())
            .ToArray();

        private List<ItemData> _itemDatas = new List<ItemData>();

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }

        public void UpdateItemsData(List<ItemData> items)
        {
            _itemUIViews.ForEach(x => Destroy(x.gameObject));
            _itemUIViews.Clear();
            _slots.ForEach(x => x.FreeSlot());

            foreach (ItemData itemData in items)
            {
                ItemUISlot itemUISlot = GetFreeSlot(itemData);

                _targetSlot = itemUISlot; // TEMP

                Transform parent = itemUISlot.ItemUiViewParent;
                InventoryItemUIView itemUIView = Instantiate(_inventoryItemUIViewPrefab, parent);

                itemUIView.PointerDown.TakeUntilDestroy(this)
                    .Subscribe((eventData => OnItemDown(eventData, itemUIView)));
                itemUIView.Drag.TakeUntilDestroy(this).Subscribe((eventData => OnItemDrag(eventData, itemUIView)));
                itemUIView.PointerUp.TakeUntilDestroy(this).Subscribe((eventData => OnItemUp(eventData, itemUIView)));

                itemUIView.Setup(itemData.ItemName.Value);

                PutItemInSlot(itemUIView, itemUISlot);

                _itemUIViews.Add(itemUIView);
            }

            _itemDatas = items;
        }

        private ItemUISlot GetFreeSlot(ItemData itemData)
        {
            ItemUISlot freeSlot = CenterSlots.FirstOrDefault(x => x.IsFree);

            if (freeSlot == null)
            {
                ItemUISlot slot = CenterSlots.FirstOrDefault();

                return slot;
            }

            return freeSlot;
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

            foreach (ItemUISlot itemUISlot in _slots)
            {
                var containsScreenPoint =
                    RectTransformUtility.RectangleContainsScreenPoint(itemUISlot.RectTransform, eventData.position);

                if (containsScreenPoint)
                {
                    if (itemUISlot == _rightHandSlot) // правая рука
                    { }
                    else if (itemUISlot == _leftHandSlot) // левая рука
                    { }
                    else // обычный слот в инвентаре
                    { }


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

        private bool IsSlotFree(ItemUISlot slot)
        {
            return slot.IsFree;
        }

        private void ReturnItemToTargetSlot(InventoryItemUIView itemUIView)
        {
            itemUIView.transform.parent = _targetSlot.ItemUiViewParent;
            itemUIView.RectTransform.localPosition = Vector3.zero;
        }

        private void SwapSlots(ItemUISlot firstSlot, ItemUISlot secondSlot)
        {
            if (firstSlot.IsFree || secondSlot.IsFree) return;

            ItemUISlotData firsSlotData = firstSlot.SlotData;
            InventoryItemUIView firsItem = firstSlot.ItemUIView;

            ItemUISlotData secondSlotData = secondSlot.SlotData;
            InventoryItemUIView secondItem = secondSlot.ItemUIView;

            
            firstSlot.AssignItem(secondSlotData, secondItem);
            secondSlot.AssignItem(firsSlotData, firsItem);

            firsItem.transform.parent = secondSlot.ItemUiViewParent;
            secondItem.transform.parent = firstSlot.ItemUiViewParent;
        }

        private bool PutItemInSlot(InventoryItemUIView itemUIView, ItemUISlot slot)
        {
            string itemName = itemUIView.ItemName;

            bool isSlotBusy = IsSlotFree(slot) == false;

            if (isSlotBusy)
            {
                Debug.Log("Slot not free, return item back");
                SwapSlots(itemUIView.Slot, slot);
                return false;
            }

            bool hasItemStaticData =
                _itemStaticDataContainer.TryGetItemStaticDataByName(itemName, out var itemStaticData);

            if (hasItemStaticData)
            {
                var itemUISlotData = new ItemUISlotData();

                itemUISlotData.ItemName = itemStaticData.ItemName;
                itemUISlotData.ItemDescription = itemStaticData.ItemDescription;
                itemUISlotData.Sprite = itemStaticData.UIIcon;

                slot.AssignItem(itemUISlotData, itemUIView);
                itemUIView.AssignAssociatedSlot(slot);

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