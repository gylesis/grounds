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
        
        public override void InstallBindings()
        {
            Container.Bind<InventoryView>().FromInstance(_inventoryView).AsSingle();
            Container.Bind<GameInventory>().FromInstance(_gameInventory).AsSingle();
            Container.Bind<SceneCameraController>().FromInstance(_mainSceneSceneCameraController).AsSingle();
            Container.Bind<PopUpService>().FromInstance(_popUpService).AsSingle();
            Container.Bind<MarkersHandler>().FromInstance(_markersHandler).AsSingle();
            Container.Bind<DependenciesContainer>().AsSingle().NonLazy();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            Container.Bind<DamageAreaSpawner>().FromInstance(_damageAreaSpawner).AsSingle();
            Container.Bind<ImpactApplier>().FromInstance(_impactApplier).AsSingle();
        }
    }
}