using Cysharp.Threading.Tasks;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using UnityEngine;
using Zenject;

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
            //  _gameDataService = DependenciesContainer.Instance.GetDependency<GameDataService>();
        }

        [Inject]
        private void Construct(InteractorView interactorView)
        {
            _interactorView = interactorView;
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
                        _hadItemInPrevFrame = true;

                        _interactorView.ShowItem(item, _player);

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
                else
                {
                    _hadItemInPrevFrame = false;
                }
            }
            else
            {
                _hadItemInPrevFrame = false;
            }
            
            if (_hadItemInPrevFrame == false)
            {
                TargetItem = null;
                _interactorView.Hide();
            }
        }

        public void ItemHandle(PlayerInput input, NetworkButtons wasPressed,NetworkButtons wasReleased)
        {
            if (!_player) return;

            if (TargetItem == null) return;
            if (_hands.IsFree is not true) return;
            
            if (TargetItem.ItemSizeType == ItemSizeType.TwoHanded
                && wasPressed.IsSet(Buttons.PickItem))
            {
                _hands.RPC_PutItem(TargetItem);
            }
            else if (wasPressed.IsSet(Buttons.PickItem)
                     && !input.Buttons.IsSet(Buttons.AlternateHand)
                     && _hands.GetHandByType(HandType.Right).IsFree)
            {
                _hands.GetHandByType(HandType.Right).RPC_PutItem(TargetItem);
            }
            else if (wasPressed.IsSet(Buttons.PickItem))
            {
                _hands.GetHandByType(HandType.Left).RPC_PutItem(TargetItem);
            }
        }
    }
}