using Dev.Scripts.UI.PopUpsAndMenus;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Infrastructure.Installers
{
    public class PopUpsLinker : MonoBehaviour
    {
        [SerializeField] private Transform _popUpsParent;
        
        private PopUpService _popUpService;

        [Inject]
        private void Construct(PopUpService popUpService)
        {
            _popUpService = popUpService;
        }

        private void Start()
        {
            _popUpService.AddPopUps(_popUpService.GetComponentsInChildren<PopUp>());
        }
    }
}