
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Hand : NetworkContext
    {
        [SerializeField] private HandType handType;
        [SerializeField] private NetworkObject _parent;
        
        public HandType HandType => handType;
        public NetworkObject Parent => _parent;

        [Networked] public Item CarryingItem { get; set; }

        public bool IsFree => CarryingItem == null;

        public void DropItem()
        {
            if (IsFree == true) return;
            
            Item item = CarryingItem;
            item.RPC_SetParent(null);
            item.RPC_SetPos(item.transform.position + Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * 1.5f);
            item.RPC_OnPickup(false);
            RPC_SetItem(null);
        }
        
        public void PutItem(Item item)
        {
            if (IsFree == false);
            
            RPC_SetItem(item);
            CarryingItem.RPC_OnPickup(true);
            item.RPC_SetParent(Parent);
            item.RPC_SetLocalPos(Vector3.zero);
            item.RPC_SetLocalRotation(Vector3.zero);
        }
        
        [Rpc]
        public void RPC_SetItem(Item item)
        {
            CarryingItem = item;
        }
        
    }
    
    
}       