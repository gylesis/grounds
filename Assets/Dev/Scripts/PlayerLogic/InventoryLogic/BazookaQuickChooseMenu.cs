using System;
using System.Collections.Generic;
using Dev.UI;
using Dev.UI.PopUpsAndMenus;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class BazookaQuickChooseMenu : PopUp
    {
        [SerializeField] private Transform _quickTabsParent;
        [SerializeField] private QuickTab _quickTabPrefab;

        [SerializeField] private Transform _chooseTransform;
        
        
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<QuickTab> _quickTabs = new();
        private Action<int> _onTabChosen;

        private QuickTab _currentTab;
        
        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }
        
        public void Setup(QuickMenuSetupContext setupContext)
        {
            _onTabChosen = setupContext.TabChosen;

            for (var index = 0; index < setupContext.Items.Count; index++)
            {
                var itemData = setupContext.Items[index];
                var hasData =
                    _itemStaticDataContainer.TryGetItemStaticDataByName(itemData.ItemName, out var itemStaticData);

                if (hasData == false) continue;

                QuickTab quickTab = Instantiate(_quickTabPrefab, _quickTabsParent);

                quickTab.Setup(index, itemStaticData.ItemIcon);
                quickTab.Button.Clicked.TakeUntilDestroy(this).Subscribe((OnQuickTabChosen));
                
                _quickTabs.Add(quickTab);
            }
            
            Show();
        }

        private void Update()
        {
            if(IsActive == false) return;

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                
            }
            
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                
            }
           
        }

        private void Move(bool isRight)
        {
            
        }
        
        private void OnQuickTabChosen(EventContext<QuickTab> context)
        {
            _onTabChosen?.Invoke(context.Value.TabId);
        }
    }
}