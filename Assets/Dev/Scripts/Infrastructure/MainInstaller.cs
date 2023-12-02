using Dev.PlayerLogic;
using Dev.Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private InteractorView _interactorView;
      //  [SerializeField] private TeamsService _teamsService;
        
        public override void InstallBindings()
        {
            Container.Bind<DependenciesContainer>().AsSingle().NonLazy();
            //Container.Bind<TeamsService>().FromInstance(_teamsService).AsSingle();
            Container.Bind<InteractorView>().FromInstance(_interactorView).AsSingle();
        }
    }
}