using System.Collections.Generic;
using Dev.Scripts.Infrastructure;
using Fusion;
using UniRx;
using Zenject;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayersDataService : NetworkContext
    {
        private PlayersSpawner _playersSpawner;

        public Subject<PlayerRef> PlayerSpawned => _playersSpawner.PlayerSpawned;
        public Subject<PlayerRef> PlayerDeSpawned => _playersSpawner.PlayerDeSpawned;

        [Inject]
        private void Construct(PlayersSpawner playersSpawner)
        {
            _playersSpawner = playersSpawner;
        }

        public List<PlayerCharacter> GetAllPlayers()
        {
            return _playersSpawner.AllPlayers;
        }
    
        public PlayerCharacter GetPlayer(PlayerRef playerRef)
        {
            PlayerCharacter playerCharacter = _playersSpawner.GetPlayer(playerRef);

            return playerCharacter;
        }
        
    }
}