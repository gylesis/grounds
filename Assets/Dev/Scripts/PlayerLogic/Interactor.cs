using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class Interactor : NetworkContext
    {
        [SerializeField] private Hands _hands;

        [SerializeField] private float _maxDistance = 5f;
        [SerializeField] private float _radius = 0.2f;

        [SerializeField] private LayerMask _itemLayer;

        private InteractorView _interactorView;
        private bool _hadItemInPrevFrame;
        private GameDataService _gameDataService;
        private PlayerCharacter _player;

        private Item TargetItem { get; set; }

        private void Start()
        {
            _interactorView = DependenciesContainer.Instance.GetDependency<InteractorView>();
            //  _gameDataService = DependenciesContainer.Instance.GetDependency<GameDataService>();
        }

        public override async void Spawned()
        {
            base.Spawned();

            //_player = _gameDataService.GetPlayer(Object.InputAuthority);

            await UniTask.DelayFrame(5);

            _player = Runner.GetPlayerObject(Object.InputAuthority).GetComponent<PlayerCharacter>(); // TEMP
        }

        public override void Render()
        {
            if (HasInputAuthority == false) return;

            ItemHandle();

            if (Time.frameCount % 3 != 0) return;

            if (_player == null) return; // TEMP

            var center = new Vector2(Screen.width / 2, Screen.height / 2);

            Ray ray = _player.CameraController.CharacterCamera.ScreenPointToRay(center);

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

        private void ItemHandle()
        {
            if (_player.InputService.PlayerInputs.Player.DropItem.WasPressedThisFrame())
            {
                if (_player.InputService.PlayerInputs.Player.RightHand.IsPressed())
                {
                    _hands.GetHandByType(HandType.Right).DropItem();
                }
                else if (_player.InputService.PlayerInputs.Player.LeftHand.IsPressed())
                {
                    _hands.GetHandByType(HandType.Left).DropItem();
                }
                else
                {
                    _hands.GetOccupiedHand()?.DropItem();
                }
            }

            if (TargetItem == null) return;


            if (TargetItem.HandType == HandType.Center &&
                _player.InputService.PlayerInputs.Player.AnyHand.WasPressedThisFrame())
            {
                _hands.PutItem(TargetItem);
            }
            else if (_player.InputService.PlayerInputs.Player.LeftHand.WasPressedThisFrame())
            {
                _hands.GetHandByType(HandType.Left).PutItem(TargetItem);
            }
            else if (_player.InputService.PlayerInputs.Player.RightHand.WasPressedThisFrame())
            {
                _hands.GetHandByType(HandType.Right).PutItem(TargetItem);
            }

        }
    }
}