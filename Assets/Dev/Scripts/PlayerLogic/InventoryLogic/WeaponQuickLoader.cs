using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class WeaponQuickLoader : NetworkContext
    {
        [SerializeField] private float _quickTabsShowCooldown = 0.5f;

        private PopUpService _popUpService;
        private GameInventory _gameInventory;

        private bool _isShown;

        private TickTimer _showTimer;   
        private PlayersDataService _playersDataService;
        private ItemStaticDataContainer _itemStaticDataContainer;

        private TickTimer _showQuickTabsTimer;

        private Dictionary<PlayerRef, List<int>> _playersLastQuickTabItems = new Dictionary<PlayerRef, List<int>>();
        private ItemsDataService _itemsDataService;

        [Networked] private NetworkBool WasArmed { get; set; } // TODO мб нужно дополнительно соблюдать это. например при переключении предмета ставить в
        
        [Inject]
        private void Construct(ItemStaticDataContainer itemStaticDataContainer, PlayersDataService playersDataService, GameInventory gameInventory, PopUpService popUpService, ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
            _itemStaticDataContainer = itemStaticDataContainer;
            _popUpService = popUpService;
            _gameInventory = gameInventory;
            _playersDataService = playersDataService;
        }

        protected override void OnDependenciesResolve()
        {
            _popUpService.TryGetPopUp<BazookaQuickChooseMenu>(out var bazookaQuickChooseMenu);
            bazookaQuickChooseMenu.ItemChosen.Subscribe((itemId => OnQuickTabChosen(itemId, Runner.LocalPlayer))); 
        }

        public override void Render()
        {
            if(HasInputAuthority == false) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                _showQuickTabsTimer = TickTimer.CreateFromSeconds(Runner, _quickTabsShowCooldown);
            }
            
            if (Input.GetKey(KeyCode.R))
            {
                if (_showQuickTabsTimer.Expired(Runner))
                {
                    if (_isShown == false)
                    {
                        RPC_ShowQuickMenuRequest(Object.InputAuthority);
                    }
                }
            }
            else
            {
                if (_isShown)
                {
                    Hide();
                }
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                _showQuickTabsTimer = TickTimer.None;
            }
            
        }

        [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
        private void RPC_ShowQuickMenuRequest(PlayerRef playerRef)
        {
            Debug.Log($"Show menu");
            var inventoryData = _gameInventory.GetInventoryData(playerRef);

            PlayerController playerController = _playersDataService.GetPlayer(playerRef).PlayerController;
            
            if (AllowToShowQuickTabs(playerController.Hands) == false) return;

            var itemDatas = new List<ItemData>();

            foreach (ItemData itemData in inventoryData.InventoryItems)
            {
                bool isItemThisTypeof = _itemStaticDataContainer.IsItemOfThisType(itemData.ItemId, ItemType.LoadableInItemLauncher);

                if (isItemThisTypeof)
                {
                    itemDatas.Add(itemData);
                }
            }

            if (_playersLastQuickTabItems.ContainsKey(playerRef) == false)
            {
                _playersLastQuickTabItems.Add(playerRef, new List<int>());
            }

            _playersLastQuickTabItems[playerRef] = itemDatas.Select(x => x.ItemId).ToList();
            
            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToAim(false);

            RPC_ShowQuickMenu(playerRef, itemDatas.ToArray());
        }

        private bool AllowToShowQuickTabs(Hands hands)
        {
            if (hands.IsAnyHandBusy() == false)
            {
                return false;
            }
            
            bool hasItem = _itemStaticDataContainer.TryGetItemByType(out var itemStaticData, ItemType.Firearm);

            if (hasItem)
            {
                var hasThisItemInHands = hands.HasThisItemInHands(itemStaticData.ItemId);

                return hasThisItemInHands;
            }

            return false;
        }

        [Rpc]
        private void RPC_ShowQuickMenu([RpcTarget] PlayerRef playerRef, ItemData[] items)
        {   
            _popUpService.TryGetPopUp<BazookaQuickChooseMenu>(out var quickChooseMenu);
            
            var quickMenuSetupContext = new QuickMenuSetupContext();
            quickMenuSetupContext.Items = items.ToList();

            quickChooseMenu.Setup(quickMenuSetupContext);

            _isShown = true;
        }

        [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
        private void RPC_HideQuickMenu(PlayerRef playerRef)
        {
            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToAim(true);
        }

        private void OnQuickTabChosen(int itemId, PlayerRef playerRef)
        {
            Debug.Log($"On quick tab chosen");
            
            if(HasStateAuthority == false) return;

            if (_playersLastQuickTabItems.ContainsKey(playerRef) == false)
            {
                Debug.Log($"No info about last items");
                return;
            }

            List<int> itemsList = _playersLastQuickTabItems[playerRef];

            bool playerHadThisItem = itemsList.Contains(itemId);

            Debug.Log($"Quick tab validation {playerHadThisItem}");

            if (playerHadThisItem)
            {
                _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

                var player = _playersDataService.GetPlayer(playerRef);
                player.PlayerController.Hands.GetHandWithThisItemType(ItemType.Firearm).GetComponent<Hand>().TryGetFirearm(out var firearm);

                var ableToReload = firearm.AbleToReload(itemId);

                Debug.Log($"Firearm ready to reload {ableToReload}");
                
                if (ableToReload)
                {
                    Item item = _itemsDataService.SpawnItem(itemId, Vector3.zero);

                    var reloadWith = firearm.ReloadWith(item);

                    Debug.Log($"Firearm reloaded {reloadWith}");

                    _gameInventory.RemoveItemFromInventory(playerRef, itemId);
                }
            }
            
            Hide();
        }

        private void Hide()
        {
            Debug.Log($"Hide quick menu");
            _popUpService.HidePopUp<BazookaQuickChooseMenu>();
            _isShown = false;
            
            RPC_HideQuickMenu(Object.InputAuthority);
        }
    }
    
}