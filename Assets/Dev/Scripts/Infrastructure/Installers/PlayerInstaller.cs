using Dev.Scripts.PlayerLogic;
using Zenject;

namespace Dev.Scripts.Infrastructure.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerCharacter>().FromComponentOnRoot();
        }
    }
    
}