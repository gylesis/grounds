using System;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using DG.Tweening;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class HandAbilities : ItemContainer
    {
        [SerializeField] private PlayerCharacter _player;

        private Camera _camera;
        protected Tween _activeTween;
        
        protected ItemsDataService _itemsDataService;

        public override void Spawned()
        {
            base.Spawned();
            _itemsDataService = DependenciesContainer.Instance.GetDependency<ItemsDataService>();

            _camera = _player.CameraController.CharacterCamera;
        }

        public virtual void PrepareToSwing()
        {
            
        }

        public virtual void Swing()
        {
            _activeTween?.Kill();
            _activeTween = AnimateSwing();
            var itemEnumeration = ContainingItem == null ? ItemEnumeration.EmptyHand : ContainingItem.ItemEnumeration;
            var point = _camera.transform.position + _camera.transform.forward * 4f;

            DamageAreaSpawner.Instance.RPC_SpawnBox(itemEnumeration, point, _player.Health);
        }

        public virtual void Throw()
        {
        }

        public virtual void UseItem()
        {
            if (ContainingItem == null)
            {
                Debug.Log("Нельзя сотворить здесь");
                return;
            }

            ContainingItem.Use();
        }

        public override void RPC_PutItem(Item item)
        {
            base.RPC_PutItem(item);
            item.SetLastOwner(_player);

            string itemName = item.ItemName;
            ItemStaticData itemStaticData = _itemsDataService.GetItemStaticData(itemName);

            item.RPC_SetLocalPos(itemStaticData.PositionInHand);
            item.RPC_SetLocalRotation(itemStaticData.RotationInHand);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_DropItem()
        {
            if (IsFree == true) return;
            
            ContainingItem.NetRigidbody.InterpolationSpace = Spaces.World;
            
            ContainingItem.RPC_ChangeState(false);
            ContainingItem.RPC_SetParent(null);
            RPC_SetEmpty();
        }


        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        protected void RPC_LaunchItem()
        {
            if (IsFree == true) return;

            ContainingItem.RPC_ChangeState(false);
            ContainingItem.RPC_SetParent(null);
            
            bool raycastSuccess =
                Physics.Raycast(
                    _camera.ScreenPointToRay(new Vector3((float) Screen.width / 2, (float) Screen.height / 2)),
                    out var hit);

            Vector3 direction =
                (raycastSuccess ? hit.point : _camera.transform.position + _camera.transform.forward * 100) -
                transform.position;
            ContainingItem.NetRigidbody.Rigidbody.AddForce(direction.normalized * 10, ForceMode.Impulse);
            

            RPC_SetEmpty();
        }
        
        public virtual Tween AnimatePrepare()
        {
            return null;
        }

        public virtual Tween AnimateSwing()
        {
            return null;
        }

        public virtual Tween AnimateThrow()
        {
            return null;
        }
    }
}