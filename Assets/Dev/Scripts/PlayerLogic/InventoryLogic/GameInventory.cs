using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.Scripts.Items;
using Fusion;
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
            data.Items.Add(itemData);

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
            RPC_ShowInventory(playerRef, _playersInventoryDatas.First(x => x.Player == playerRef));
        }
    
        [Rpc]
        private void RPC_ShowInventory([RpcTarget] PlayerRef playerRef, InventoryData inventoryData)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            Debug.Log($"[Client] Show inventory request completed");
            var itemDatas = inventoryData.Items.ToList();
            
            _inventoryView.Show();
        }   
        
        public void Hide()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _inventoryView.Hide();
        }
    }
}