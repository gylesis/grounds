using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class InventoryItemInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _infoText;

        public void Show(string itemInfo)
        {
            _infoText.DOFade(1, 0.3f);
            _infoText.text = $"{itemInfo}";
        }

        public void Hide()
        {
            _infoText.DOFade(0, 0.3f);
        }
        
    }
}