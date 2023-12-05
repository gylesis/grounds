using Dev.Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class LobbySceneInstaller : MonoInstaller
    {
        [SerializeField] private InteractorView _interactorView;
        
        public override void InstallBindings()
        {
            Container.Bind<DependenciesContainer>().AsSingle().NonLazy();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
        }
    }
}