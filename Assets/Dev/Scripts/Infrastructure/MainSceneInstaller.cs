using Dev.Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Dev.Infrastructure
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private SceneCameraController _mainSceneSceneCameraController;
        [SerializeField] private InteractorView _interactorView;
        [SerializeField] private PlayersSpawner _playersSpawner;
        [SerializeField] private DamageAreaSpawner _damageAreaSpawner;

        public override void InstallBindings()
        {
            Container.Bind<SceneCameraController>().FromInstance(_mainSceneSceneCameraController).AsSingle();
            Container.Bind<DependenciesContainer>().AsSingle().NonLazy();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
            Container.Bind<PlayersSpawner>().FromInstance(_playersSpawner).AsSingle();
            Container.Bind<DamageAreaSpawner>().FromInstance(_damageAreaSpawner).AsSingle();
        }
    }
}