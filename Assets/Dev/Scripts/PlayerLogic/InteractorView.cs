using System;
using TMPro;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class InteractorView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _itemName;

        private Item _targetItem;

        public void ShowItem(Item item)
        {
            _targetItem = item;
            
            _itemName.text = item.TestName;
            _canvasGroup.alpha = 1;
        }

        private void Update()
        {
            if(_targetItem == null) return;
            
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(_targetItem.transform.position + Vector3.up * 1.3f);
            _rectTransform.position = screenPoint;
        }

        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _itemName.text = "None";

            _targetItem = null;
        }
    }
}