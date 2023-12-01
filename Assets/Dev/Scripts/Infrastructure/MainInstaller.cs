using Dev.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class MainInstaller : MonoInstaller
    {
        [SerializeField] private TeamsService _teamsService;
        
        public override void InstallBindings()
        {
            Container.Bind<TeamsService>().FromInstance(_teamsService).AsSingle();
        }
    }
}