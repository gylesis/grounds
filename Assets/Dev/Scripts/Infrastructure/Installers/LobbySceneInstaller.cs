using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class LobbySceneInstaller : MonoInstaller
    {
        [SerializeField] private InteractorView _interactorView;
        [SerializeField] private ItemsDataService _itemsDataService;
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private PlayersSpawner _playersSpawner;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            Container.Bind<SceneLoader>().FromInstance(_sceneLoader).AsSingle();
            Container.Bind<ItemsDataService>().FromInstance(_itemsDataService).AsSingle();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
        }
    }
}