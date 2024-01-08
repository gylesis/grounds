using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using Fusion;
using UniRx;
using UnityEngine;

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
            _inventoryView = DependenciesContainer.Instance.GetDependency<InventoryView>();
            _playersDataService = DependenciesContainer.Instance.GetDependency<PlayersDataService>();
            _itemsDataService = DependenciesContainer.Instance.GetDependency<ItemsDataService>();

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

        private void OnInventoryHandItemChanged(PlayerRef playerRef, InventoryHandView.HandChangedEventContext context,
            bool isLeftHand)
        {
            if (HasStateAuthority == false) return;

            string itemName = context.ItemName;
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

            Debug.Log($"Item {context.ItemName} to remove - {context.ToRemove}, is left hand {isLeftHand}");
                
            if (context.ToRemove)
            {
                var leftHand = playerCharacter.PlayerController.Hands.GetHandByType(HandType.Left);
                var rightHand = playerCharacter.PlayerController.Hands.GetHandByType(HandType.Right);
                
                if (leftHand.IsFree == false)
                {
                    var itemData = new ItemData();
                    itemData.ItemName = itemName;
                    
                    PutItemInInventory(itemData, playerRef);
                    
                    RemoveItemFromHands(playerRef, itemName);
                    _itemsDataService.RPC_RemoveItemFromWorld(leftHand.ContainingItem);
                    leftHand.RPC_DropItem();
                }
                else if (rightHand.IsFree == false)
                {
                    var itemData = new ItemData();
                    itemData.ItemName = itemName;
                    
                    PutItemInInventory(itemData, playerRef);
                    
                    RemoveItemFromHands(playerRef, itemName);
                    _itemsDataService.RPC_RemoveItemFromWorld(rightHand.ContainingItem);
                    rightHand.RPC_DropItem();
                }
            }
            else
            {
                RemoveItemFromInventory(playerRef, itemName);
                
                Item item = _itemsDataService.SpawnItem(itemName, Vector3.zero);
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
            RemoveItemFromHands(playerRef, itemData.ItemName.Value);

            //Debug.Log($"Item {itemData.ItemName} dropped from hands");
        }

        private void RemoveItemFromHands(PlayerRef playerRef, string itemName)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            if(inventoryData.HandItems.Any(x => x.ItemName == itemName) == false) return;
            
            ItemData data = inventoryData.HandItems.First(x => x.ItemName == itemName);

            inventoryData.HandItems.Remove(data);
            _playersInventoryDatas[indexOf] = inventoryData;
        }
            
         private void RemoveItemFromInventory(PlayerRef playerRef, string itemName)
        {
            InventoryData inventoryData = GetInventoryData(playerRef);
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            if(inventoryData.InventoryItems.Any(x => x.ItemName == itemName) == false) return;
            
            ItemData data = inventoryData.InventoryItems.First(x => x.ItemName == itemName);

            inventoryData.InventoryItems.Remove(data);
            _playersInventoryDatas[indexOf] = inventoryData;

            Debug.Log($"Remove item {itemName} from Player {playerRef}");
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

        private void OnRemoveItemFromInventoryClient(string itemName)
        {
            RPC_RemoveItemFromInventory(Runner.LocalPlayer, itemName);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RemoveItemFromInventory(PlayerRef itemOwner, NetworkString<_16> itemName)
        {
            RemoveItemFromInventory(itemOwner, itemName.Value);
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
            
            Debug.Log($"Item {itemData.ItemName} added to Player {playerRef}");
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

            _playersDataService.GetPlayer(playerRef).PlayerController.ForbidToMove = true;

            var inventoryItems = inventoryData.InventoryItems.ToArray();
            var handsItems = inventoryData.HandItems.ToArray();

            _inventoryView.Show(inventoryItems, handsItems);
        }

        public void Hide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _playersDataService.GetPlayer(Runner.LocalPlayer).PlayerController.ForbidToMove = false;

            _inventoryView.Hide();
        }
    }
}