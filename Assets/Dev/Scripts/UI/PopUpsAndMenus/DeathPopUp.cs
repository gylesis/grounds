using Dev.Scripts.Infrastructure;
using TMPro;
using UniRx;
using UnityEngine;

namespace Dev.Scripts.UI.PopUpsAndMenus
{
    public class DeathPopUp : PopUp
    {
        [SerializeField] private TMP_Text _loseText;
        [SerializeField] private DefaultTextReactiveButton _quitButton;
        
        protected override void Awake()
        {
            base.Awake();

            _quitButton.Clicked.TakeUntilDestroy(PopUpService).Subscribe((unit => OnQuitBtnClicked()));
        }

        private void OnQuitBtnClicked()
        {
            ConnectionManager.Instance.QuitToLobby();
        }
    }
}