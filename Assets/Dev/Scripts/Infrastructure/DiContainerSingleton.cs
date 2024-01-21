using Zenject;

namespace Dev.Infrastructure
{
    public class DiContainerSingleton
    {
        private DiContainer _diContainer;

        public static DiContainerSingleton Instance;
        
        public DiContainerSingleton(DiContainer diContainer)
        {
            _diContainer = diContainer;
            Instance = this;
        }

        public void Inject(object injectable)
        {
            _diContainer.Inject(injectable);
        }
        
    }
}