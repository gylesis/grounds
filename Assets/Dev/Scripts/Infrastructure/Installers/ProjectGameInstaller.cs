using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.Scripts.UI.PopUpsAndMenus;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Infrastructure.Installers
{
    public class ProjectGameInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private ItemStaticDataContainer _itemStaticDataContainer;
        [SerializeField] private PopUpService _popUpService;
        
        public override void InstallBindings()
        {
            Container.Bind<DiContainerSingleton>().AsSingle().NonLazy();
            Container.Bind<PopUpService>().FromInstance(_popUpService).AsSingle();

            Container.Bind<ItemStaticDataContainer>().FromInstance(_itemStaticDataContainer).AsSingle();
            Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
        }
    }
}