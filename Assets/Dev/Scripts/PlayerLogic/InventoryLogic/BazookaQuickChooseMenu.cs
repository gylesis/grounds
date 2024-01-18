using System;
using System.Collections.Generic;
using System.Linq;
using Dev.Infrastructure;
using Dev.UI;
using Dev.UI.PopUpsAndMenus;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class BazookaQuickChooseMenu : PopUp
    {
        [SerializeField] private Transform _quickTabsParent;
        [SerializeField] private QuickTab _quickTabPrefab;
        [SerializeField] private HorizontalLayoutGroup _tabHorizontalLayout;
        [SerializeField] private RectTransform _tabsCenterTransform;
        
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<QuickTab> _quickTabs = new();
        private Action<int> _onTabChosen;

        private int TabsCount => _quickTabs.Count;
        
        private QuickTab _currentTab;
        private int _currentTabIndex = 0;

        private float MaxWidth => (TabsCount - 1) * WidthStep;
        private float WidthStep => (_tabHorizontalLayout.spacing + _quickTabPrefab.RectTransform.rect.width);
        
        private float _swapTimer;
        
        private void Start()    
        {
            _quickTabs = _quickTabsParent.GetComponentsInChildren<QuickTab>().ToList();
            
            if(TabsCount == 0) return;
            
            _currentTab = _quickTabs[_currentTabIndex];
        }

        [Inject]
        private void Init(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }
        
        public void Setup(QuickMenuSetupContext setupContext)
        {
            foreach (var quickTab in _quickTabs)
            {
                Destroy(quickTab.gameObject);
            }
            _quickTabs.Clear();
            
            
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

            if (Input.GetMouseButtonDown(0))
            {
                ApplyCurrentTabItem();
            }
            
            if (AllowToSwapTab() == false) return;

            if (Mouse.current.delta.x.value > 2)
            {
                Move(true);
            }
            else if (Mouse.current.delta.x.value < -2)
            {
                Move(false);
            }
        }

        private void ApplyCurrentTabItem()
        {
            SelectTab(_currentTabIndex);
            
            
        }

        private void SelectTab(int tabId)
        {
            for (var index = 0; index < _quickTabs.Count; index++)
            {
                var quickTab = _quickTabs[index];
                quickTab.SetSelectionState(index == tabId);
            }
        }

        private bool AllowToSwapTab()
        {
            bool allowToSwapTab;
            
            if (_swapTimer >= 0.1f)
            {
                allowToSwapTab = true;
            }
            else
            {
                allowToSwapTab = false;
                _swapTimer += Time.deltaTime;
            }

            return allowToSwapTab;
        }

        private void Move(bool isRight)
        {
            float originPos = _tabsCenterTransform.anchoredPosition.x;

            if (isRight)
            {
                if (originPos - WidthStep < -MaxWidth)
                {
                    return;
                }
            }
            else
            {
                if (originPos + WidthStep > 0)
                {
                    return;
                }
            }
            
            _swapTimer = 0;

            float moveUnits = _tabHorizontalLayout.spacing + _quickTabPrefab.RectTransform.rect.width;
            float sign = isRight ? -1 : 1;
            
            float targetPos = originPos + (sign * moveUnits);
            
            DOVirtual.Float(originPos, targetPos, 0.099f, (value =>
            {
                var anchoredPosition = Vector2.right * value;

                _tabsCenterTransform.anchoredPosition = anchoredPosition;
            }));
            
            _currentTabIndex += 1 * -(int) sign;
    
            _currentTab = _quickTabs[_currentTabIndex];
        }
        
        private void OnQuickTabChosen(EventContext<QuickTab> context)
        {
            _onTabChosen?.Invoke(context.Value.TabId);
        }
    }
}