using System;
using Dev.Infrastructure;
using Dev.UI.PopUpsAndMenus;
using Fusion.KCC;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dev.PlayerLogic
{
    public class PlayerController : NetworkContext
    {
        [SerializeField] private KCC _kcc;

        [SerializeField] private Transform _cameraTransform;
            
        [SerializeField] private float _jumpModifier = 2;
        [SerializeField] private float _sensivity = 2;
        
        private PopUpService _popUpService;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void Spawned()
        {
            if (HasInputAuthority == false)
            {
                GetComponentInChildren<Camera>().gameObject.SetActive(false);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput<PlayerInput>(out var input))
            {
                _kcc.AddLookRotation(input.LookDirection * _sensivity * Runner.DeltaTime);

                Vector3 moveDirection = _kcc.transform.forward * input.MoveDirection.y + _kcc.transform.right * input.MoveDirection.x;  
                moveDirection.Normalize();
                _kcc.SetInputDirection(moveDirection);

                if (input.Jump)
                {
                    _kcc.Jump(Vector3.up * _jumpModifier);
                }
            }

        }

        public override void Render()
        {
            Quaternion quaternion = _cameraTransform.transform.rotation;
            Vector3 eulerAngles = quaternion.eulerAngles;   
            _cameraTransform.transform.rotation = Quaternion.Euler(-_kcc.FixedData.LookPitch,eulerAngles.y,eulerAngles.z);
        }
    }
}