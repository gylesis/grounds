using System;
using Dev.PlayerLogic;
using Dev.Scripts.Items;
using TMPro;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class InteractorView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _itemName;

        private Item _targetItem;
        private Camera _camera;
        
        private ItemsDataService _itemsDataService;

        [Inject]
        private void Construct(ItemsDataService itemsDataService)
        {
            _itemsDataService = itemsDataService;
        }
        
        public void ShowItem(Item item, PlayerCharacter playerCharacter)
        {
            _camera = playerCharacter.CameraController.CharacterCamera; // TODO temp
            _targetItem = item;
            
            _itemName.text = _itemsDataService.GetItemNameById(item.ItemId);
            _canvasGroup.alpha = 1;
        }

        private void Update()
        {
            if(_targetItem == null) return;
            
            Vector3 screenPoint = _camera.WorldToScreenPoint(_targetItem.transform.position + Vector3.up * 1.3f);
            _rectTransform.position = screenPoint;
        }

        public void Hide()
        {
            _camera = null;
            _canvasGroup.alpha = 0;
            _itemName.text = "None";

            _targetItem = null;
        }
    }
}