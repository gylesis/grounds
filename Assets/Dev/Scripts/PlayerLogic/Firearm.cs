using System.Collections.Generic;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class Firearm : ItemContainer
    {
        [SerializeField] private Item _correspondingItem;
        [SerializeField] private List<ItemType> _allowedAmmunition;
        [SerializeField] private int _magazineSize;
        [SerializeField] private float _itemAcceleration = 25;

        [Networked] private int MagazineAmmoAmount { get; set; }
        
        private ItemsDataService _itemsDataService;
        private Camera PlayerCamera => _correspondingItem.ItemDynamicData.PlayerCharacter.CameraController.CharacterCamera;

        [Inject]
        private void Construct(ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
        }
        
        public override void Spawned()
        {
            base.Spawned();
            _correspondingItem.UpdateUseAction(RPC_Shoot);
        }

        public bool AbleToReload(int itemId)
        {
            if (MagazineAmmoAmount >= _magazineSize)
            {
                Debug.Log("Больше не лезет");
                return false;
            }   

            if (CheckAmmunitionCompatibility(itemId))
            {
                Debug.Log("Снаряд не подходит");
                return false;
            }
            
            return true;
        }

        public bool ReloadWith(Item item)
        {
            if (item == null)
            {
                Debug.Log("Не суй руку в дуло");
                return false;
            }
            
            if (AbleToReload(item.ItemId) == false)
            {
                return false;
            }
            
            MagazineAmmoAmount += 1;
            RPC_PutItem(item);

            Debug.Log($"Item {item} loaded into weapon");
            return true;
        }

        public bool CheckAmmunitionCompatibility(int itemId)
        {
            ItemStaticData itemStaticData = _itemsDataService.GetItemStaticData(itemId);

            foreach (var itemType in _allowedAmmunition)
            {
                if (itemStaticData.ItemTypes.Contains(itemType)) return true;
            }

            return false;
        }
        
        [Rpc]
        public void RPC_Shoot()
        {
            if (IsFree == true) return;

            MagazineAmmoAmount -= 1;
            ContainingItem.RPC_SetParent(null);
            ContainingItem.RPC_ChangeState(false);

            bool raycastSuccess =
                Physics.Raycast(
                    PlayerCamera.ScreenPointToRay(new Vector3((float) Screen.width / 2, (float) Screen.height / 2)),
                    out var hit);

            Vector3 direction =
                (raycastSuccess ? hit.point : PlayerCamera.transform.position + PlayerCamera.transform.forward * 100) -
                ContainingItem.transform.position;
            
            Debug.DrawRay(ContainingItem.transform.position, direction, Color.blue, 2);
            ContainingItem.NetRigidbody.Rigidbody.AddForce(direction.normalized * _itemAcceleration, ForceMode.Impulse);
            SetItem(null);
        }
    }
}