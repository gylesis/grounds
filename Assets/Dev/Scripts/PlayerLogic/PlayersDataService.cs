using Dev.Infrastructure;
using Dev.PlayerLogic;
using Fusion;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayersDataService : NetworkContext
    {
        private PlayersSpawner _playersSpawner;

        [Inject]
        private void Construct(PlayersSpawner playersSpawner)
        {
            _playersSpawner = playersSpawner;
        }
    
        public PlayerCharacter GetPlayer(PlayerRef playerRef)
        {
            PlayerCharacter playerCharacter = _playersSpawner.GetPlayer(playerRef);

            return playerCharacter;
        }
        
    }
}