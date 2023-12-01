using Dev.PlayerLogic;
using Dev.Utils;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameSettings _gameSettings;
        
        public override void InstallBindings()
        {
            Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
        }
    }
}