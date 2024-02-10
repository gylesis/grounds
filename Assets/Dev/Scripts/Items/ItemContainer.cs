using Dev.Scripts.Infrastructure;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class ItemContainer : NetworkContext
    {
        [SerializeField] private NetworkObject _itemMountPoint;
        public NetworkObject ItemMountPoint => _itemMountPoint;

        [ReadOnly] [Networked] public Item ContainingItem { get; set; }

        public bool IsFree => ContainingItem == null;
        
        public Subject<ItemData> ItemTaken { get; } = new Subject<ItemData>();
        public Subject<ItemData> ItemDropped { get; } = new Subject<ItemData>();
    
    
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public virtual void RPC_PutItem(Item item)
        {
            if (IsFree == false) return;
            
            Debug.Log($"Item {item.ItemId} placed to item container {name}");

            SetItem(item);
            ContainingItem.RPC_ChangeState(true);
            ContainingItem.RPC_SetParent(_itemMountPoint);
            ContainingItem.RPC_SetLocalPos(Vector3.zero);
            ContainingItem.RPC_SetLocalRotation(Vector3.zero);
            
            var itemData = new ItemData(item.ItemId);
            
            ItemTaken.OnNext(itemData);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SetEmpty()
        {
            var itemData = new ItemData(ContainingItem.ItemId);

            ItemDropped.OnNext(itemData);
            
            SetItem(null);
        }
        
        protected void SetItem(Item item)
        {
            ContainingItem = item;
        }
    }
    
}