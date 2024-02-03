using Dev.Scripts.PlayerLogic;
using Fusion;

namespace Dev.Scripts.Infrastructure
{
    public class World : IDamageInflictor
    {
        private static World _world; 
        public static IDamageInflictor world
        {
            get
            {
                if (_world == null)
                {
                    _world = new World();
                }
                return _world;
            }
        }

        public PlayerRef PlayerRef => PlayerRef.None;
    }
}