using Dev.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.Utils;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private ItemStaticDataContainer _itemStaticDataContainer;

        public override void InstallBindings()
        {
            Container.Bind<ItemStaticDataContainer>().FromInstance(_itemStaticDataContainer).AsSingle();
            Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
        }
    }
}