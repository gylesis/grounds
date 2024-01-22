using Dev.PlayerLogic;
using Zenject;

namespace Dev.Infrastructure
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerCharacter>().FromComponentOnRoot();
        }
    }
    
}