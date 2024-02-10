using System.Collections.Generic;
using System.Linq;
using Dev.Scripts.Infrastructure;
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
        
        private InventoryView _inventoryView;
        private PlayersDataService _playersDataService;
        private ItemsDataService _itemsDataService;

        public Subject<bool> InventoryOpened { get; } = new Subject<bool>();

        [Inject]
        private void Construct(InventoryView inventoryView, PlayersDataService playersDataService, ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
            _inventoryView = inventoryView;
            _playersDataService = playersDataService;
        }

        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();
            
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
            
            if (Runner.IsServer)
            {
                _playersDataService.PlayerSpawned.TakeUntilDestroy(this).Subscribe((OnPlayerSpawned));
                _playersDataService.PlayerDeSpawned.TakeUntilDestroy(this).Subscribe((OnPlayerDeSpawned));
            }
        }

        private void OnPlayerSpawned(PlayerRef playerRef)   
        {
            PlayerCharacter playerCharacter = _playersDataService.GetPlayer(playerRef);
            playerCharacter.PlayerController.Hands.PutItemInInventory += OnPutItemInInventory;

            void OnPutItemInInventory(ItemData itemData, PlayerRef playerRef)
            {
                PutItemInInventory(itemData, playerRef);
            }
                
            var inventoryData = new InventoryData(playerRef);
            _playersInventoryDatas.Add(inventoryData);

            foreach (Hand hand in playerCharacter.PlayerController.Hands.HandsList)
            {
                hand.ItemTaken.TakeUntilDestroy(this).Subscribe((s => OnItemTakenToHands(s, playerRef, hand.HandType)));
                
                hand.ItemDropped.TakeUntilDestroy(this)
                    .Subscribe((s => OnItemDroppedFromHands(s, playerRef, hand.HandType)));
            }
        }

        private void OnPlayerDeSpawned(PlayerRef playerRef)
        {
            if (_playersInventoryDatas.Any(x => x.Player == playerRef))
            {
                InventoryData inventoryData = _playersInventoryDatas.FirstOrDefault(x => x.Player == playerRef);
                _playersInventoryDatas.Remove(inventoryData);
            }
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

            Debug.Log($"Item {context.ItemId} to remove - {context.ToRemoveFromHand}, is left hand {isLeftHand}");
                
            if (context.ToRemoveFromHand)
            {
                if (targetHand.IsFree == false)
                {
                    var itemData = new ItemData(itemId);
                    
                    PutItemInInventory(itemData, playerRef);

                    HandType handType = isLeftHand ? HandType.Left : HandType.Right;
                    RemoveItemFromHands(playerRef, itemId, handType);
                    
                    _itemsDataService.RPC_RemoveItemFromWorld(targetHand.ContainingItem);
                    targetHand.RPC_DropItem();
                } 
               
            }
            else
            {
                RemoveItemFromInventory(playerRef, itemId);
                
                Item item = _itemsDataService.SpawnItem(itemId, Vector3.zero);
                targetHand.RPC_PutItem(item);
            }
            
        }

        private void OnItemDroppedFromHands(ItemData itemData, PlayerRef playerRef, HandType handType)
        {
            RemoveItemFromHands(playerRef, itemData.ItemId, handType);

            //Debug.Log($"Item {itemData.ItemName} dropped from hands");
        }

        private void RemoveItemFromHands(PlayerRef playerRef, int itemId, HandType handType)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            if(inventoryData.HandItems.Any(x => x.ItemId == itemId && x.HandType == handType) == false) return;
            
            HandItemData data = inventoryData.HandItems.First(x => x.ItemId == itemId && x.HandType == handType);
    
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

            Debug.Log($"<color=red>Removed</color> item {itemId} from Player {playerRef}'s inventory");
        }   
        
        private void OnItemTakenToHands(ItemData itemData, PlayerRef playerRef, HandType handType)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            Debug.Log($"Item {itemData.ItemId} added to hands");

            inventoryData.InventoryItems.Remove(itemData);

            var handItemData = new HandItemData(itemData, handType);
            
            inventoryData.HandItems.Add(handItemData);
            
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
            Debug.Log($"Item {itemData.ItemId} <color=green>added</color> to Player {playerRef}'s inventory");
        }

        public void ShowInventory(PlayerRef playerRef)
        {
            RPC_RequestShowInventory(playerRef);
            // Debug.Log($"[Client] Request to show inventory for player {playerRef}");
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestShowInventory(PlayerRef playerRef)
        {
            if(playerRef == PlayerRef.None) return;

            Debug.Log($"[Server] Requested to show inventory for player {playerRef}");
            InventoryData inventoryData = _playersInventoryDatas.First(x => x.Player == playerRef);

            RPC_ShowInventory(playerRef, inventoryData);
        }

        [Rpc]
        private void RPC_ShowInventory([RpcTarget] PlayerRef playerRef, InventoryData inventoryData)
        {
           // Debug.Log($"RPC SHOW INVENTORY FOR player {playerRef}");
           CursorController.SetActiveState(true);

            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToMove(false);
            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToAim(false);

            var inventoryItems = inventoryData.InventoryItems.ToArray();
            var handsItems = inventoryData.HandItems.ToArray();

            _inventoryView.Show(inventoryItems, handsItems);
        }

        public void HideInventory(PlayerRef playerRef)
        {
            RPC_HideInventoryRequest(playerRef);
            
            CursorController.SetActiveState(false);

            _inventoryView.Hide();
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_HideInventoryRequest(PlayerRef playerRef)
        {
            if(playerRef == PlayerRef.None) return;

           // Debug.Log($"Hide inventory request from {playerRef}");
            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToMove(true);
            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToAim(true);
        }

    }
}