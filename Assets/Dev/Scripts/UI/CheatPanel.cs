using System;
using Dev.Scripts.Infrastructure;
using Dev.Scripts.PlayerLogic;
using Fusion;
using UniRx;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.UI
{
    public class CheatPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private DefaultReactiveButton _takeDmgButton;
        
        private PlayersDataService _playersDataService;
        private DamageAreaSpawner _damageAreaSpawner;

        private bool IsActive => _canvasGroup.alpha != 0;
        
        private void Awake()
        {
            _takeDmgButton.Clicked.TakeUntilDestroy(this).Subscribe((unit => OnTakeDmgButton()));
        }

        [Inject]
        private void Construct(PlayersDataService playersDataService, DamageAreaSpawner damageAreaSpawner)
        {
            _damageAreaSpawner = damageAreaSpawner;
            _playersDataService = playersDataService;
        }
        
        private void OnTakeDmgButton()
        {
            RPC_MakeDamageTo(ConnectionManager.Instance.Runner.LocalPlayer);
        }

        [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
        private void RPC_MakeDamageTo(PlayerRef playerRef)
        {   
            _playersDataService.GetPlayer(playerRef).Health.TakeDamage(50);
        }

        public void Show()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
        }
        
        public void Hide()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L) && Input.GetKeyDown(KeyCode.K))
            {
                if (IsActive)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
        }
    }
}