using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
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

        private void Start()
        {
            _inventoryView = DependenciesContainer.Instance.GetDependency<InventoryView>();
            _inventoryView.ToRemoveItemFromInventory.TakeUntilDestroy(this).Subscribe((OnRemoveItemFromInventoryClient));
        }

        private void OnRemoveItemFromInventoryClient(string itemName)
        {
            RPC_RemoveItemFromInventory(Runner.LocalPlayer, itemName);
        }
    
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RemoveItemFromInventory(PlayerRef itemOwner, NetworkString<_16> itemName)
        {
            InventoryData inventoryData = _playersInventoryDatas.First(x => x.Player == itemOwner);
            ItemData itemData = inventoryData.InventoryItems.First(x => x.ItemName.Value == itemName);
           
            var indexOf = _playersInventoryDatas.IndexOf(inventoryData);

            inventoryData.InventoryItems.Remove(itemData);

            _playersInventoryDatas[indexOf] = inventoryData;
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
            Debug.Log($"Put item to iventory {playerRef}");
            InventoryData data = _playersInventoryDatas.First(x => x.Player == playerRef);

            var indexOf = _playersInventoryDatas.IndexOf(data);
            data.InventoryItems.Add(itemData);

            _playersInventoryDatas[indexOf] = data;
            
            Debug.Log($"Item {itemData.ItemName} put in inventory");
        }
        
        
        public void ShowInventory(PlayerRef playerRef)
        {
            RPC_RequestShowInventory(playerRef);
            Debug.Log($"[Client] Request to show inventory for player {playerRef}");
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestShowInventory(PlayerRef playerRef)
        {
            Debug.Log($"[Server] Requested to show inventory for player {playerRef}");
            InventoryData inventoryData = _playersInventoryDatas.First(x => x.Player == playerRef);
            
            
            
            RPC_ShowInventory(playerRef, inventoryData);
        }
    
        [Rpc]
        private void RPC_ShowInventory([RpcTarget] PlayerRef playerRef, InventoryData inventoryData)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            Debug.Log($"[Client] Show inventory request completed");
            var items = inventoryData.InventoryItems.ToList();
            
            
            
            _inventoryView.Show(items.ToArray());
        }   
        
        public void Hide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _inventoryView.Hide();
        }
    }
}