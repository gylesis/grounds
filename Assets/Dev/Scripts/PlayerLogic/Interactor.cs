using System;
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Interactor : NetworkContext
    {
        [SerializeField] private HandsService _handsService;
        [SerializeField] private Camera _camera;
        [SerializeField] private InputService _inputService;
        [SerializeField] private PlayerView _playerView;
        
        [SerializeField] private float _maxDistance = 5f;
        [SerializeField] private float _radius = 0.2f;

        [SerializeField] private LayerMask _itemLayer;

        private InteractorView _interactorView;
        private bool _hadItemInPrevFrame;

        private Item TargetItem { get; set; }

        private void Start()
        {
            _interactorView = DependenciesContainer.Instance.GetDependency<InteractorView>();
        }

        public override void Render()
        {
            if(HasInputAuthority == false) return;
            
            if (Time.frameCount % 3 != 0) return;

            var center = new Vector2(Screen.width / 2, Screen.height / 2);

            Ray ray = _camera.ScreenPointToRay(center);

            var sphereCast = Physics.SphereCast(transform.position, _radius, ray.direction, out var hit, _maxDistance,
                _itemLayer);

            if (sphereCast)
            {
                var isItem = hit.transform.gameObject.TryGetComponent<Item>(out var item);

                if (isItem)
                {
                    if (TargetItem == null)
                    {
                        _interactorView.ShowItem(item);

                        TargetItem = item;
                    }
                    else
                    {
                        if (TargetItem.GetInstanceID() == item.GetInstanceID())
                        {
                            _hadItemInPrevFrame = true;
                        }
                    }
                }
            }
            else
            {
                if (_hadItemInPrevFrame)
                {
                    TargetItem = null;
                    _hadItemInPrevFrame = false;
                    _interactorView.Hide();
                }
            }
        }

        private void Update()
        {
            bool hasItemInLeftHand = _handsService.HasItemInHand(HandType.Left);
            var hasItemInRightHand = _handsService.HasItemInHand(HandType.Right);
            var hasItemInCenterHand = _handsService.HasItemInHand(HandType.Center);
            
            /*_playerView.OnItemPickup(HandType.Left, hasItemInLeftHand);
            _playerView.OnItemPickup(HandType.Right, hasItemInRightHand);
            _playerView.OnItemPickup(HandType.Center, hasItemInCenterHand);*/
            
            if (hasItemInLeftHand) // left hand
            {
                if (_inputService.PlayerInputs.Player.DropItemLeft.WasPressedThisFrame())
                {
                    _handsService.DropItemFromHand(HandType.Left);
                }
            }

            if (hasItemInRightHand) // right hand
            {
                if (_inputService.PlayerInputs.Player.DropItemRight.WasPressedThisFrame())
                {
                    _handsService.DropItemFromHand(HandType.Right);
                }
            }

            if (hasItemInCenterHand) // center hand
            {
                if (_inputService.PlayerInputs.Player.DropItemRight.WasPressedThisFrame() ||
                    _inputService.PlayerInputs.Player.DropItemLeft.WasPressedThisFrame())
                {
                    _handsService.DropItemFromHand(HandType.Center);
                }
            }
            
            if (TargetItem == null) return;

            if (_inputService.PlayerInputs.Player.Interaction.WasPressedThisFrame())
            {
                if (_handsService.AbleToPutItem(TargetItem.HandType))
                {
                    _handsService.PutItemInHand(TargetItem.HandType, TargetItem);
                }
            }

           
        }
    }
}