using System;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using UnityEngine;
using Zenject;

namespace Dev.Scripts.Infrastructure.Installers
{
    public class LobbySceneInstaller : MonoInstaller
    {
        [SerializeField] private InteractorView _interactorView;
        [SerializeField] private ItemsDataService _itemsDataService;
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private PlayersSpawner _playersSpawner;
        [SerializeField] private DamageAreaSpawner _damageAreaSpawner;
        [SerializeField] private PlayersDataService _playersDataService;
        [SerializeField] private SceneCameraController _sceneCameraController;
        [SerializeField] private PlayersDeathController _playersDeathController;
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private CraftStation _craftStation;
        [SerializeField] private GameInventory _gameInventory;
                    
        public override void InstallBindings()
        {
            Container.Bind<SceneCameraController>().FromInstance(_sceneCameraController).AsSingle();
            Container.Bind<DiContainerSingleton>().AsSingle().NonLazy();
            Container.Bind<DamageAreaSpawner>().FromInstance(_damageAreaSpawner).AsSingle();
            
            Container.Bind<PlayersDeathController>().FromInstance(_playersDeathController).AsSingle();
            Container.Bind<PlayersDataService>().FromInstance(_playersDataService).AsSingle();
            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            
            Container.Bind<InventoryView>().FromInstance(_inventoryView).AsSingle();
            Container.Bind<GameInventory>().FromInstance(_gameInventory).AsSingle();
            Container.Bind<CraftStation>().FromInstance(_craftStation).AsSingle();
            
            Container.Bind<SceneLoader>().FromInstance(_sceneLoader).AsSingle();
            Container.Bind<ItemsDataService>().FromInstance(_itemsDataService).AsSingle();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
        }
    }
}