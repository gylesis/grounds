using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class GameInventory : NetworkContext
    {
        private PopUpService _popUpService;

        private List<InventoryData> _playersInventoryDatas = new List<InventoryData>();
        private bool _invOpened = false;

        private void Start()
        {
            _popUpService = DependenciesContainer.Instance.GetDependency<PopUpService>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_OnPlayerSpawned(PlayerRef playerRef)
        {
            var inventoryData = new InventoryData(playerRef);

            Debug.Log($"Added player inv data {playerRef}");
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
            Debug.Log($"[Client] Show inventory request completed");
            var itemDatas = inventoryData.Items.ToList();
            
            _popUpService.TryGetPopUp<InventoryUIMenu>(out var inventoryUIMenu);

            inventoryUIMenu.UpdateItemsData(itemDatas);
            
            _popUpService.ShowPopUp<InventoryUIMenu>();
        }   
        
        public void Hide()
        {
            _popUpService.HidePopUp<InventoryUIMenu>();
        }
    }
}