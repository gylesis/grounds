using Dev.Infrastructure;
using Dev.PlayerLogic;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class ItemContainer : NetworkContext
    {
        [SerializeField] private NetworkObject _itemMountPoint;
        private PlayerCharacter _playerCharacter;
        public NetworkObject ItemMountPoint => _itemMountPoint;
        
        [Networked] public Item ContainingItem { get; set; }
        
        public bool IsFree => ContainingItem == null;


        public override async void Spawned()
        {
            base.Spawned();

            _playerCharacter = FindObjectOfType<PlayerCharacter>();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public virtual void RPC_PutItem(Item item)
        {
            Debug.Log("PutItem");
            if (IsFree == false) return;
            
            RPC_SetItem(item);
            ContainingItem.RPC_ChangeState(true);
            ContainingItem.RPC_SetParent(_itemMountPoint);
            ContainingItem.RPC_SetLocalPos(Vector3.zero);
            ContainingItem.RPC_SetLocalRotation(Vector3.zero);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_DropItem()
        {
            Debug.Log("DropItem");
            if (IsFree == true) return;

            ContainingItem.RPC_SetParent(null);
            ContainingItem.NetRigidbody.InterpolationSpace = Spaces.World;
            // ContainingItem.RPC_SetPos(ContainingItem.transform.position + Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up) * 1.5f);
            ContainingItem.RPC_ChangeState(false);
            RPC_SetItem(null);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_LaunchItem()
        {
            Debug.Log("LaunchItem");
            if (IsFree == true) return;
            
            ContainingItem.RPC_SetParent(null);
            ContainingItem.RPC_ChangeState(false);
            
            Camera camera = _playerCharacter.CameraController.CharacterCamera;  
            
            bool raycastSuccess = Physics.Raycast(camera.ScreenPointToRay(new Vector3((float)Screen.width / 2, (float)Screen.height / 2)), out var hit);
            
            Vector3 direction = (raycastSuccess ? hit.point : camera.transform.position + camera.transform.forward * 100) - transform.position;  
            ContainingItem.NetRigidbody.Rigidbody.AddForce(direction.normalized * 10, ForceMode.Impulse);
            RPC_SetItem(null);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_SetItem(Item item)
        {
            ContainingItem = item;
        }
    }
}