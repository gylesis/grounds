using DG.Tweening;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _curtain;  
        [SerializeField] private DragHandler _dragHandler;

        public void Show()
        {
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _dragHandler.SetActive(true);
        }

        public void Hide()
        {
            _curtain.alpha = 1;
            _curtain.DOFade(0, 0.2f);
            _dragHandler.SetActive(false);
        }
        
    }
}