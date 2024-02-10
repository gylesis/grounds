using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.LevelLogic;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.Scripts.Utils;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Dev.Scripts.Items
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private InventoryItemInfoView _inventoryItemInfoView;
        [SerializeField] private CanvasGroup _curtain;  
        [FormerlySerializedAs("_dragHandler")] [SerializeField] private InventoryItemsDragHandler _inventoryItemsDragHandler;
        [SerializeField] private Transform _itemsParent;
        
        [SerializeField] private TriggerZone _craftTriggerZone;
        [SerializeField] private TriggerZone _dropItemZone;
        [SerializeField] private TriggerZone _itemInfoPiedestalZone;

        [SerializeField] private InventoryHandView[] _inventoryHandViews;
            
        [SerializeField] private BoxCollider _spawnBox;
                
        private CraftStation _craftStation;
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<InventoryItemView> _itemViews = new List<InventoryItemView>();
        private List<ItemData> _items = new List<ItemData>();
        private ItemsDataService _itemsDataService;

        public InventoryHandView LeftHandView => _inventoryHandViews.First(x => x.IsLeftHand);
        public InventoryHandView RightHandView => _inventoryHandViews.First(x => x.IsLeftHand == false);

        public Subject<int> ToRemoveItemFromInventory { get; } = new Subject<int>();

        private void Awake()
        {
            Hide();

            _craftTriggerZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnCraftZoneEntered));
            _dropItemZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnDropItemZoneEntered));
            _itemInfoPiedestalZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnItemPiedestalEntered));
            _itemInfoPiedestalZone.TriggerExit.TakeUntilDestroy(this).Subscribe((OnItemPiedestalExit));
            
            _inventoryItemsDragHandler.DraggableObjectUp.TakeUntilDestroy(_inventoryItemsDragHandler)
                .Subscribe((OnItemDragUp));
        }

        private void Start()
        {
            _craftStation.Crafted.TakeUntilDestroy(this).Subscribe((OnItemCrafted));
        }

        [Inject]
        private void Construct(ItemStaticDataContainer itemStaticDataContainer, CraftStation craftStation, ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
            _itemStaticDataContainer = itemStaticDataContainer;
            _craftStation = craftStation;
        }

        public void Show(ItemData[] inventoryItems, HandItemData[] handsItems)
        {
            _items = inventoryItems.ToList();
            _items.AddRange(handsItems.Select(x => x.ItemData));
            
            foreach (ItemData itemData in inventoryItems)
            {
                int itemId = itemData.ItemId;
             
                _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

                Vector3 spawnPos = _spawnBox.bounds.RandomPointInBounds();
                InventoryItemView itemView = Instantiate(itemStaticData.InventoryData.Prefab, spawnPos, Quaternion.identity, _itemsParent);
                itemView.Setup(itemId);
                
                _itemViews.Add(itemView);
            }   
            
            foreach (HandItemData itemData in handsItems)
            {
                int itemId = itemData.ItemId;
             
                _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

                Vector3 spawnPos = new Vector3(999,999,999);
                InventoryItemView itemView = Instantiate(itemStaticData.InventoryData.Prefab, spawnPos, Quaternion.identity, _itemsParent);
                itemView.Setup(itemId);

                if (itemData.HandType == HandType.Left)
                {
                    LeftHandView.PutItem(itemView, true);
                }
                else
                {
                    RightHandView.PutItem(itemView, true);
                }
                
                _itemViews.Add(itemView);
            }
            
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _inventoryItemsDragHandler.SetActive(true);
        }

        private void OnItemDragUp(DraggableObject draggableObject)
        {
            if(draggableObject.TryGetComponent<InventoryItemView>(out var itemView) == false) return; 
            
            InventoryHandView GetHandView()
            {
                foreach (InventoryHandView inventoryHandView in _inventoryHandViews)
                {
                    if(inventoryHandView.PotentialItemView == null) continue;
                
                    if (inventoryHandView.PotentialItemView.gameObject.GetInstanceID() ==
                        draggableObject.gameObject.GetInstanceID())
                    {
                        return inventoryHandView;
                    }
                }

                return null;
            }

            InventoryHandView handView = GetHandView();
            
            if (handView == null) // means: dropped to the floor
            {
                if (itemView.HasParent)
                {
                    itemView.ParentHandView.FreeHand();
                }
                
                return;
            }   
            
            InventoryHandView otherHandView = _inventoryHandViews.First(x => x.IsLeftHand != handView.IsLeftHand);
            
            if (handView.IsHandBusy)
            {
                bool sameItems = handView.HoldingItemView.ItemId == itemView.ItemId;
                
                if (sameItems)
                {
                    if (itemView.HasParent)
                    {
                        itemView.ParentHandView.PullItemToHand(itemView);
                    }
                    else
                    {
                        SendItemBack(itemView);
                    }

                    Debug.Log($"Items are same, no swap applied");
                    return;
                }

                InventoryItemView currentHandItem = handView.HoldingItemView;

                if (otherHandView.IsHandBusy)
                {
                    handView.FreeHand();
                    otherHandView.FreeHand();

                    otherHandView.PutItem(currentHandItem);
                    handView.PutItem(itemView);
                }
                else
                {
                    handView.FreeHand();

                    handView.PutItem(itemView);

                    SendItemBack(currentHandItem);
                }
            }
            else
            {
                if (handView.PotentialItemView == null) return;

                if (otherHandView.PotentialItemView != null)
                {
                    if (otherHandView.PotentialItemView.gameObject.GetInstanceID() ==
                        itemView.gameObject.GetInstanceID())
                    {
                        otherHandView.FreeHand();
                    }
                }
                
                handView.PutItem(handView.PotentialItemView);
            }
            
            
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

        public void Hide()  
        {
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _inventoryItemsDragHandler.SetActive(false);
            
            _itemViews.ForEach(x => Destroy(x.gameObject));
            _itemViews.Clear();
            _items.Clear();
            
            _inventoryItemInfoView.Hide();
        }

        public void SendItemBack(InventoryItemView itemView)
        {
            Vector3 pos = _spawnBox.bounds.RandomPointInBounds();

            itemView.DraggableObject.Rigidbody.position = pos;
        }

        private void OnDropItemZoneEntered(Collider collider)
        {
            if (collider.TryGetComponent<InventoryItemView>(out var itemView))
            {
                int itemName = itemView.ItemId;
                
                ReturnWorldItemAlive(itemName);
                RemoveItemFromInventory(itemName);
            }    
        }

        private void RemoveItemFromInventory(int itemId)
        {
            InventoryItemView itemView = _itemViews.First(x => x.ItemId == itemId);
            Destroy(itemView.gameObject);
            _itemViews.Remove(itemView);

            ToRemoveItemFromInventory.OnNext(itemId);
        }

        private void ReturnWorldItemAlive(int itemId)
        {
            ItemData itemData = _items.First(x => x.ItemId == itemId);

            _itemsDataService.RPC_ReturnItemToWorldFromInventory(itemId);
        }
        
        private void OnCraftZoneEntered(Collider collider)
        {
            if (collider.TryGetComponent<InventoryItemView>(out var itemView))
            {
                _craftStation.AddToCraftingTable(itemView.ItemId);
            }
        }

        private void OnItemPiedestalEntered(Collider collider)
        {
            if (collider.TryGetComponent<InventoryItemView>(out var itemView))
            {
                _itemStaticDataContainer.TryGetItemStaticDataById(itemView.ItemId, out var itemStaticData);
                
                string itemInfo = $"{itemView.ItemId}\n{itemStaticData.ItemDescription}";
                _inventoryItemInfoView.Show(itemInfo);
            }
        }

        private void OnItemPiedestalExit(Collider collider)
        {
            if (collider.TryGetComponent<InventoryItemView>(out var itemView))
            {
                _inventoryItemInfoView.Hide();
            }
        }
        
    }
}