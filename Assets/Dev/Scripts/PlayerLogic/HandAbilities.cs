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
        private PlayerCharacter _player;

        private Camera _camera;
        protected Tween _activeTween;

        protected ItemsDataService _itemsDataService;
        private DamageAreaSpawner _damageAreaSpawner;
        private PlayersDataService _playersDataService;

        protected void Construct(ItemsDataService itemsDataService, PlayersDataService playersDataService,
            DamageAreaSpawner damageAreaSpawner)
        {
            _damageAreaSpawner = damageAreaSpawner;
            _playersDataService = playersDataService;
            _itemsDataService = itemsDataService;
        }

        protected override void OnDependenciesResolve()
        {
            if (Object.HasStateAuthority)
            {
                _player = _playersDataService.GetPlayer(Object.InputAuthority);
                _camera = _player.CameraController.CharacterCamera;
            }
        }

        protected virtual void Start()
        {
        }

        public virtual void PrepareToSwing()
        {
        }

        public virtual void Swing()
        {
            _activeTween?.Kill();
            _activeTween = AnimateSwing();
            
            RPC_Swing();
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

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public override void RPC_PutItem(Item item)
        {
            base.RPC_PutItem(item);
            item.SetLastOwner(_player);

            int itemId = item.ItemId;
            ItemStaticData itemStaticData = _itemsDataService.GetItemStaticData(itemId);

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

            var throwForce = direction.normalized * 10;
            var playerVelocity = _player.Kcc.Data.RealVelocity * 0.5f;
            Debug.Log(playerVelocity);
            ContainingItem.NetRigidbody.Rigidbody.AddForce(throwForce + playerVelocity, ForceMode.Impulse);


            RPC_SetEmpty();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_Swing()
        {
            var itemId = ContainingItem == null ? -1 : ContainingItem.ItemId;
            var point = _camera.transform.position + _camera.transform.forward * 4f;
            
            _damageAreaSpawner.RPC_SpawnBox(itemId, point, _player.Health);
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