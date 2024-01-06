using Dev.Levels.Interactions;
using Dev.Scripts.Items;
using Dev.Scripts.PlayerLogic;
using Dev.Scripts.PlayerLogic.InventoryLogic;
using Dev.UI.PopUpsAndMenus;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private SceneCameraController _mainSceneSceneCameraController;
        [SerializeField] private InteractorView _interactorView;
        [SerializeField] private PlayersSpawner _playersSpawner;
        [SerializeField] private DamageAreaSpawner _damageAreaSpawner;
        [SerializeField] private MarkersHandler _markersHandler;
        [SerializeField] private PopUpService _popUpService;
        [SerializeField] private GameInventory _gameInventory;
        [SerializeField] private ImpactApplier _impactApplier;
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private CraftStation _craftStation;
        [SerializeField] private ItemsDataService _itemsDataService;
        public override void InstallBindings()
        {
            Container.Bind<CraftStation>().FromInstance(_craftStation).AsSingle();
            Container.Bind<InventoryView>().FromInstance(_inventoryView).AsSingle();
            Container.Bind<GameInventory>().FromInstance(_gameInventory).AsSingle();
            Container.Bind<ItemsDataService>().FromInstance(_itemsDataService).AsSingle();
            
            Container.Bind<SceneCameraController>().FromInstance(_mainSceneSceneCameraController).AsSingle();
            Container.Bind<PopUpService>().FromInstance(_popUpService).AsSingle();
            
            Container.Bind<MarkersHandler>().FromInstance(_markersHandler).AsSingle();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            
            Container.Bind<DamageAreaSpawner>().FromInstance(_damageAreaSpawner).AsSingle();
            Container.Bind<ImpactApplier>().FromInstance(_impactApplier).AsSingle();
            
            Container.Bind<DependenciesContainer>().AsSingle().NonLazy();
        }
    }
}