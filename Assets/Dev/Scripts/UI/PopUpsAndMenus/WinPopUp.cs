using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Fusion;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dev.UI.PopUpsAndMenus
{
    public class WinPopUp : PopUp
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _winText;
        [SerializeField] private DefaultTextReactiveButton _continueButton;

        protected override void Awake()
        {
            base.Awake();
            _continueButton.gameObject.SetActive(false);

            _continueButton.Clicked.TakeUntilDestroy(PopUpService).Subscribe(unit => OnContinueButtonClicked());
        }

        private async void OnContinueButtonClicked()
        {
            var networkRunner = FindObjectOfType<NetworkRunner>();
            networkRunner.Shutdown(false); 

            PopUpService.ShowPopUp<LoadingScreenPopUp>();
            Hide();

            await UniTask.Delay(1500);
            await SceneManager.LoadSceneAsync(1).AsAsyncOperationObservable().ToUniTask();
        }
        
        [Rpc]
        public void RPC_Setup([RpcTarget] PlayerRef playerRef, List<ItemData> items)
        {
            _winText.text = $"You {playerRef}, looted items:\n";

            foreach (ItemData itemData in items)
            {
                _winText.text += $"{itemData.ItemName}\n";
            }

            Show();
        }

        public override void Show()
        {
            base.Show();
            
            _continueButton.Disable();
            _continueButton.SetText("CONTINUE");
            _continueButton.gameObject.SetActive(true);
            
            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe((l =>
            {
                _continueButton.Enable();
            }));

        }
    }
    
}