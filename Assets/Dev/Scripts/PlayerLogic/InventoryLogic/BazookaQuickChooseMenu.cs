using System;
using System.Collections.Generic;
using System.Linq;
using Dev.UI.PopUpsAndMenus;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class BazookaQuickChooseMenu : PopUp
    {
        [SerializeField] private Transform _chooseArrowTransform;
        [SerializeField] private Transform _quickTabsParent;
        [SerializeField] private QuickTab _quickTabPrefab;
        [SerializeField] private HorizontalLayoutGroup _tabHorizontalLayout;
        [SerializeField] private RectTransform _tabsCenterTransform;
        [SerializeField] private TMP_Text _noItemsSign;
        
        private float MaxWidth => (TabsCount - 1) * WidthStep;
        private float WidthStep => (_tabHorizontalLayout.spacing + _quickTabPrefab.RectTransform.rect.width);
        
        private ItemStaticDataContainer _itemStaticDataContainer;

        private List<QuickTab> _quickTabs = new();
        private Action<int> _onTabChosen;

        private QuickTab _currentSelectedTab;
        private QuickTab _currentScrollingTab;
        
        private int _currentTabIndex = 0;
        private float _swapTimer;
        private int TabsCount => _quickTabs.Count;

        [Inject]
        private void Construct(ItemStaticDataContainer itemStaticDataContainer)
        {
            _itemStaticDataContainer = itemStaticDataContainer;
        }

        private void Start()    
        {
            _quickTabs = _quickTabsParent.GetComponentsInChildren<QuickTab>().ToList();
            
            if(TabsCount == 0) return;
            
            _currentSelectedTab = _quickTabs[_currentTabIndex];
        }

        public void Setup(QuickMenuSetupContext setupContext)
        {
            foreach (var quickTab in _quickTabs)
            {
                Destroy(quickTab.gameObject);
            }
            
            _quickTabs.Clear();

            int itemsCount = setupContext.Items.Count;
            
            if (itemsCount == 0)
            {
                _noItemsSign.text = "No items to load";
                _noItemsSign.gameObject.SetActive(true);
                _chooseArrowTransform.gameObject.SetActive(false);
                return;
            }
            
            _chooseArrowTransform.gameObject.SetActive(true);
            _noItemsSign.gameObject.SetActive(false);
            
            _onTabChosen = setupContext.TabChosen;

            for (var index = 0; index < itemsCount; index++)
            {
                var itemData = setupContext.Items[index];
                int itemId = itemData.ItemId;
                
                var hasData = _itemStaticDataContainer.TryGetItemStaticDataById(itemId, out var itemStaticData);

                if (hasData == false) continue;

                QuickTab quickTab = Instantiate(_quickTabPrefab, _quickTabsParent);

                quickTab.Setup(itemStaticData.ItemIcon, itemId);
                
                _quickTabs.Add(quickTab);
            }
            
            Show();
        }

        private void Update()
        {
            if(IsActive == false && _quickTabs.Count != 0) return;

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
                var isSelected = index == tabId;

                if (isSelected)
                {
                    _currentSelectedTab = quickTab;
                }
                
                quickTab.SetSelectionState(isSelected);
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
    
            _currentScrollingTab = _quickTabs[_currentTabIndex];
        }
    }
}