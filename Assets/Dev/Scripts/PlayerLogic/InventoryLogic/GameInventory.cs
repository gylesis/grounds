using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class GameInventory : NetworkContext
    {
        private List<InventoryData> _playersInventoryDatas = new List<InventoryData>();
        private bool _invOpened = false;
        private InventoryView _inventoryView;
        private PlayersDataService _playersDataService;
        private ItemsDataService _itemsDataService;

        private void Start()
        {
            _inventoryView.ToRemoveItemFromInventory
                .TakeUntilDestroy(this)
                .Subscribe((OnRemoveItemFromInventoryClient));

            _inventoryView.LeftHandView.ItemChanged
                .TakeUntilDestroy(this)
                .Subscribe((context =>
                    OnInventoryHandItemChanged(Runner.LocalPlayer, context, true)));

            _inventoryView.RightHandView.ItemChanged
                .TakeUntilDestroy(this)
                .Subscribe((context =>
                    OnInventoryHandItemChanged(Runner.LocalPlayer, context, false)));
        }

        [Inject]
        private void Construct(InventoryView inventoryView, PlayersDataService playersDataService, ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
            _inventoryView = inventoryView;
            _playersDataService = playersDataService;
        }
        
        private void OnInventoryHandItemChanged(PlayerRef playerRef, InventoryHandView.HandChangedEventContext context,
            bool isLeftHand)
        {
            if (HasStateAuthority == false) return;

            int itemId = context.ItemId;
            
            PlayerCharacter playerCharacter = _playersDataService.GetPlayer(playerRef);
            Hand targetHand;
            
            if (isLeftHand)
            {
                targetHand = playerCharacter.PlayerController.Hands.GetHandByType(HandType.Left);
            }
            else
            {
                targetHand = playerCharacter.PlayerController.Hands.GetHandByType(HandType.Right);
            }

            Debug.Log($"Item {context.ItemId} to remove - {context.ToRemove}, is left hand {isLeftHand}");
                
            if (context.ToRemove)
            {
                var leftHand = playerCharacter.PlayerController.Hands.GetHandByType(HandType.Left);
                var rightHand = playerCharacter.PlayerController.Hands.GetHandByType(HandType.Right);
                
                if (leftHand.IsFree == false)
                {
                    var itemData = new ItemData(itemId);
                    
                    PutItemInInventory(itemData, playerRef);
                    
                    RemoveItemFromHands(playerRef, itemId);
                    _itemsDataService.RPC_RemoveItemFromWorld(leftHand.ContainingItem);
                    leftHand.RPC_DropItem();
                }
                else if (rightHand.IsFree == false)
                {
                    var itemData = new ItemData(itemId);
                    
                    PutItemInInventory(itemData, playerRef);
                    
                    RemoveItemFromHands(playerRef, itemId);
                    _itemsDataService.RPC_RemoveItemFromWorld(rightHand.ContainingItem);
                    rightHand.RPC_DropItem();
                }
            }
            else
            {
                RemoveItemFromInventory(playerRef, itemId);
                
                Item item = _itemsDataService.SpawnItem(itemId, Vector3.zero);
                targetHand.RPC_PutItem(item);
            }
            
        }

        public override async void Spawned()
        {
            base.Spawned();

            await UniTask.Delay(1000);

            if (HasStateAuthority)
            {
                PlayerCharacter playerCharacter = _playersDataService.GetPlayer(Runner.LocalPlayer);

                foreach (Hand hand in playerCharacter.PlayerController.Hands.HandsList)
                {
                    hand.ItemTaken.TakeUntilDestroy(this).Subscribe((s => OnItemTakenToHands(s, Runner.LocalPlayer)));
                    hand.ItemDropped.TakeUntilDestroy(this)
                        .Subscribe((s => OnItemDroppedFromHands(s, Runner.LocalPlayer)));
                }
            }
        }

        private void OnItemDroppedFromHands(ItemData itemData, PlayerRef playerRef)
        {
            RemoveItemFromHands(playerRef, itemData.ItemId);

            //Debug.Log($"Item {itemData.ItemName} dropped from hands");
        }

        private void RemoveItemFromHands(PlayerRef playerRef, int itemId)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            if(inventoryData.HandItems.Any(x => x.ItemId == itemId) == false) return;
            
            ItemData data = inventoryData.HandItems.First(x => x.ItemId == itemId);

            inventoryData.HandItems.Remove(data);
            _playersInventoryDatas[indexOf] = inventoryData;
        }
            
        public void RemoveItemFromInventory(PlayerRef playerRef, int itemId)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            if(inventoryData.InventoryItems.Any(x => x.ItemId == itemId) == false) return;
            
            ItemData data = inventoryData.InventoryItems.First(x => x.ItemId == itemId);

            inventoryData.InventoryItems.Remove(data);
            _playersInventoryDatas[indexOf] = inventoryData;

            Debug.Log($"Removed item {itemId} from Player {playerRef}");
        }   
        
        private void OnItemTakenToHands(ItemData itemData, PlayerRef playerRef)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            //Debug.Log($"Item {itemData.ItemName} added to hands");

            inventoryData.InventoryItems.Remove(itemData);
            inventoryData.HandItems.Add(itemData);
            
            _playersInventoryDatas[indexOf] = inventoryData;
        }

        private void OnRemoveItemFromInventoryClient(int itemId)
        {
            RPC_RemoveItemFromInventory(Runner.LocalPlayer, itemId);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RemoveItemFromInventory(PlayerRef itemOwner, int itemId)
        {
            RemoveItemFromInventory(itemOwner, itemId);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_OnPlayerSpawned(PlayerRef playerRef)
        {
            var inventoryData = new InventoryData(playerRef);

            _playersInventoryDatas.Add(inventoryData);
        }

        public InventoryData GetInventoryData(PlayerRef playerRef)
        {
            return _playersInventoryDatas.First(x => x.Player == playerRef);
        }

        public void PutItemInInventory(ItemData itemData, PlayerRef playerRef)
        {
            InventoryData data = _playersInventoryDatas.First(x => x.Player == playerRef);

            var indexOf = _playersInventoryDatas.IndexOf(data);
            data.InventoryItems.Add(itemData);

            _playersInventoryDatas[indexOf] = data;
            
            Debug.Log($"Item {itemData.ItemId} added to Player {playerRef}");
        }

        public void ShowInventory(PlayerRef playerRef)
        {
            RPC_RequestShowInventory(playerRef);
            // Debug.Log($"[Client] Request to show inventory for player {playerRef}");
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestShowInventory(PlayerRef playerRef)
        {
            //Debug.Log($"[Server] Requested to show inventory for player {playerRef}");
            InventoryData inventoryData = _playersInventoryDatas.First(x => x.Player == playerRef);

            RPC_ShowInventory(playerRef, inventoryData);
        }

        [Rpc]
        private void RPC_ShowInventory([RpcTarget] PlayerRef playerRef, InventoryData inventoryData)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToMove(false);
            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToAim(false);

            var inventoryItems = inventoryData.InventoryItems.ToArray();
            var handsItems = inventoryData.HandItems.ToArray();

            _inventoryView.Show(inventoryItems, handsItems);
        }

        public void Hide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _playersDataService.GetPlayer(Runner.LocalPlayer).PlayerController.SetAllowToMove(true);
            _playersDataService.GetPlayer(Runner.LocalPlayer).PlayerController.SetAllowToAim(true);

            _inventoryView.Hide();
        }
    }
}