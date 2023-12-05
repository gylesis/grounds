using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class ItemContainer : NetworkContext
    {
        [SerializeField] private NetworkObject _itemMountPoint;
        public NetworkObject ItemMountPoint => _itemMountPoint;
        
        [Networked] public Item ContainingItem { get; set; }
        
        public bool IsFree => ContainingItem == null;

        
        public virtual void PutItem(Item item)
        {
            if (IsFree == false) return;
            
            RPC_SetItem(item);
            ContainingItem.RPC_ChangeState(true);
            ContainingItem.RPC_SetParent(_itemMountPoint);
            ContainingItem.RPC_SetLocalPos(Vector3.zero);
            ContainingItem.RPC_SetLocalRotation(Vector3.zero);
        }
        
        public void DropItem()
        {
            if (IsFree == true) return;
            
            ContainingItem.RPC_SetParent(null);
            ContainingItem.RPC_SetPos(ContainingItem.transform.position + Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * 1.5f);
            ContainingItem.RPC_ChangeState(false);
            RPC_SetItem(null);
        }

        public void LaunchItem()
        {
            if (IsFree == true) return;
            
            ContainingItem.RPC_SetParent(null);
            ContainingItem.RPC_ChangeState(false);
            bool raycastSuccess = Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3((float)Screen.width / 2, (float)Screen.height / 2)), out var hit);
            Vector3 direction = (raycastSuccess ? hit.point : Camera.main.transform.position + Camera.main.transform.forward * 100) - transform.position;  
            ContainingItem.NetRigidbody.Rigidbody.AddForce(direction.normalized * 10, ForceMode.Impulse);
            RPC_SetItem(null);
        }
        
        [Rpc]
        private void RPC_SetItem(Item item)
        {
            ContainingItem = item;
        }
    }
}