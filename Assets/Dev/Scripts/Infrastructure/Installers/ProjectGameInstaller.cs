using Dev.Scripts.PlayerLogic.InventoryLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class ProjectGameInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private ItemStaticDataContainer _itemStaticDataContainer;

        public override void InstallBindings()
        {
            Container.Bind<DiContainerSingleton>().AsSingle().NonLazy();

            Container.Bind<ItemStaticDataContainer>().FromInstance(_itemStaticDataContainer).AsSingle();
            Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
        }
    }
}