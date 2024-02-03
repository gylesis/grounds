using Dev.Scripts.Infrastructure;
using Fusion;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.UI.PopUpsAndMenus
{
    public class DeathPopUp : PopUp
    {
        [SerializeField] private TMP_Text _loseText;
        
        [SerializeField] private DefaultTextReactiveButton _quitButton;
        [SerializeField] private DefaultTextReactiveButton _respawnButton;
        
        private PlayersSpawner _playersSpawner;

        protected override void Awake()
        {
            base.Awake();

            _quitButton.Clicked.TakeUntilDestroy(PopUpService).Subscribe((unit => OnQuitBtnClicked()));
            _respawnButton.Clicked.TakeUntilDestroy(this).Subscribe((unit => OnRespawnButtonClicked()));
        }

        [Inject]
        private void Construct(PlayersSpawner playersSpawner)
        {
            _playersSpawner = playersSpawner;
        }

        private void OnRespawnButtonClicked()
        {
            PlayerRef playerRef = ConnectionManager.Instance.Runner.LocalPlayer;
            _playersSpawner.RPC_RequestToRespawnPlayer(playerRef);              
        }

        private void OnQuitBtnClicked()
        {
            ConnectionManager.Instance.QuitToLobby();
        }
    }
}