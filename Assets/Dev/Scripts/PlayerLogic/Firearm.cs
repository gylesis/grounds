using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Firearm : ItemContainer
    {
        [SerializeField] private Item _correspondingItem;
        [SerializeField] private List<ItemEnumeration> _allowedAmmunition;
        [SerializeField] private int _magazineSize;
        [SerializeField] private float _itemAcceleration = 25;

        private int _magazineAmmoAmount;
        private Camera PlayerCamera => _correspondingItem.ItemDynamicData.PlayerCharacter.CameraController.CharacterCamera;

        public override void Spawned()
        {
            base.Spawned();
            _correspondingItem.UpdateUseAction(RPC_Shoot);
        }

        public bool ReloadWith(Item item)
        {
            if (item == null)
            {
                Debug.Log("Не суй руку в дуло");
                return false;
            }

            if (CheckAmmunitionCompatibility(item))
            {
                Debug.Log("Снаряд не подходит");
                return false;
            }

            if (_magazineAmmoAmount >= _magazineSize)
            {
                Debug.Log("Больше не лезет");
                return false;
            }
            
            _magazineAmmoAmount += 1;
            RPC_PutItem(item);
            return true;
        }

        public bool CheckAmmunitionCompatibility(Item item)
        {
            return _allowedAmmunition.Any(enumeration => item.ItemEnumeration == enumeration);
        }
        
        [Rpc]
        public void RPC_Shoot()
        {
            if (IsFree == true) return;

            _magazineAmmoAmount -= 1;
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