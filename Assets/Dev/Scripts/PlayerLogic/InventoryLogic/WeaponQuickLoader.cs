using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class WeaponQuickLoader : NetworkContext
    {
        [SerializeField] private float _quickTabsShowCooldown = 1f;

        private PopUpService _popUpService;
        private GameInventory _gameInventory;

        [Networked] private NetworkBool IsShown { get; set; }

        private TickTimer _showTimer;
        private PlayersDataService _playersDataService;
        private ItemStaticDataContainer _itemStaticDataContainer;

        private TickTimer _showQuickTabsTimer;
        
        [Inject]
        private void Construct(ItemStaticDataContainer itemStaticDataContainer, PlayersDataService playersDataService, GameInventory gameInventory, PopUpService popUpService)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
            _popUpService = popUpService;
            _gameInventory = gameInventory;
            _playersDataService = playersDataService;
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
                    if (IsShown == false)
                    {
                        RPC_ShowQuickMenuRequest(Object.InputAuthority);
                    }
                }
            }
            else
            {
                if (IsShown)
                {
                    Hide();
                }
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                _showQuickTabsTimer = TickTimer.None;
            }
            
        }

        [Rpc]
        private void RPC_ShowQuickMenuRequest(PlayerRef playerRef)
        {
            var inventoryData = _gameInventory.GetInventoryData(playerRef);

            var itemDatas = new List<ItemData>();

            foreach (ItemData itemData in inventoryData.InventoryItems)
            {
                bool isItemThisTypeof = _itemStaticDataContainer.IsItemThisTypeof(itemData.ItemName, ItemType.LoadableInItemLauncher);

                if (isItemThisTypeof)
                {
                    itemDatas.Add(itemData);
                }
            }
            
            RPC_ShowQuickMenu(playerRef, itemDatas.ToArray());
        }

        [Rpc]
        private void RPC_ShowQuickMenu([RpcTarget] PlayerRef playerRef, ItemData[] items)
        {   
            _popUpService.TryGetPopUp<BazookaQuickChooseMenu>(out var quickChooseMenu);
            
            var quickMenuSetupContext = new QuickMenuSetupContext();
            quickMenuSetupContext.Items = items.ToList();

            quickChooseMenu.Setup(quickMenuSetupContext);

            _playersDataService.GetPlayer(playerRef).PlayerController.SetAllowToAim(false);

            IsShown = true;
        }

        private void Hide()
        {
            _popUpService.HidePopUp<BazookaQuickChooseMenu>();
            IsShown = false;
            
            _playersDataService.GetPlayer(Object.InputAuthority).PlayerController.SetAllowToAim(true);
        }
    }
}