using Dev.PlayerLogic;
using UnityEngine;
using Zenject;

namespace Dev.Infrastructure
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerCharacter>().FromComponentOnRoot();

            Debug.Log("Install bindings");
        }
    }
    
}