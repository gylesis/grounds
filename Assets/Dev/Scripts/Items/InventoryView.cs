using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.LevelLogic;
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
        
        [SerializeField] private InventoryHandView _leftHandView;
        [SerializeField] private InventoryHandView _rightHandView;
            
        [SerializeField] private BoxCollider _spawnBox;
                
        private CraftStation _craftStation;
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<InventoryItemView> _itemViews = new List<InventoryItemView>();
        private List<ItemData> _items = new List<ItemData>();
        private ItemsDataService _itemsDataService;

        public InventoryHandView LeftHandView => _leftHandView;

        public InventoryHandView RightHandView => _rightHandView;

        public Subject<int> ToRemoveItemFromInventory { get; } = new Subject<int>();

        private void Awake()
        {
            Hide();

            _craftTriggerZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnCraftZoneEntered));
            _dropItemZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnDropItemZoneEntered));
            _itemInfoPiedestalZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnItemPiedestalEntered));
            _itemInfoPiedestalZone.TriggerExit.TakeUntilDestroy(this).Subscribe((OnItemPiedestalExit));
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

        public void Show(ItemData[] inventoryItems, ItemData[] handsItems)
        {
            _items = inventoryItems.ToList();
            _items.AddRange(handsItems);
            
            foreach (ItemData itemData in inventoryItems)
            {
                int itemId = itemData.ItemId;
             
                _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

                Vector3 spawnPos = _spawnBox.bounds.RandomPointInBounds();
                InventoryItemView itemView = Instantiate(itemStaticData.InventoryData.Prefab, spawnPos, Quaternion.identity, _itemsParent);
                itemView.Setup(itemId);
                
                _itemViews.Add(itemView);
            }
            
            foreach (ItemData itemData in handsItems)
            {
                int itemId = itemData.ItemId;
             
                _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

                Vector3 spawnPos = new Vector3(999,999,999);
                InventoryItemView itemView = Instantiate(itemStaticData.InventoryData.Prefab, spawnPos, Quaternion.identity, _itemsParent);
                itemView.Setup(itemId);

                if (_leftHandView.IsHandBusy)
                {
                    _rightHandView.PutItem(itemView, true);
                }
                else
                {
                    _leftHandView.PutItem(itemView, true);
                }
                
                _itemViews.Add(itemView);
            }
            
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _inventoryItemsDragHandler.SetActive(true);
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