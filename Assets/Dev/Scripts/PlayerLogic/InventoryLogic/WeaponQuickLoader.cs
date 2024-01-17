using System.Linq;
using Dev.Infrastructure;
using Dev.UI.PopUpsAndMenus;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class WeaponQuickLoader : NetworkContext
    {
        private float _timer;

        private PopUpService _popUpService;
        private GameInventory _gameInventory;

        [Networked] private NetworkBool IsShown { get; set; }

        private TickTimer _showTimer;
        private PlayersDataService _playersDataService;

        private void Start()
        {
            _popUpService = DependenciesContainer.Instance.GetDependency<PopUpService>();
            _gameInventory = DependenciesContainer.Instance.GetDependency<GameInventory>();
            _playersDataService = DependenciesContainer.Instance.GetDependency<PlayersDataService>();
        }

        public override void Render()
        {
            if(HasInputAuthority == false) return;

            if (Input.GetKey(KeyCode.R))
            {
                if (IsShown == false)
                {
                    RPC_ShowQuickMenuRequest(Object.InputAuthority);
                }
            }
            else
            {
                if (IsShown)
                {
                    Hide();
                }
            }
            
        }

        [Rpc]
        private void RPC_ShowQuickMenuRequest(PlayerRef playerRef)
        {
            var inventoryData = _gameInventory.GetInventoryData(playerRef);
            RPC_ShowQuickMenu(playerRef, inventoryData.InventoryItems.ToArray());
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