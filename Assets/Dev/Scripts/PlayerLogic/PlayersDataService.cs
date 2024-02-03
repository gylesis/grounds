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
        private PlayersDeathController _playersDeathController;

        public Subject<PlayerRef> PlayerSpawned => _playersSpawner.PlayerSpawned;
        public Subject<PlayerRef> PlayerDeSpawned => _playersSpawner.PlayerDeSpawned;

        public Subject<PlayerRef> PlayerDied { get; } = new();
        
        [Inject]
        private void Construct(PlayersSpawner playersSpawner, PlayersDeathController playersDeathController)
        {
            _playersDeathController = playersDeathController;   
            _playersSpawner = playersSpawner;
        }

        protected override void OnDependenciesResolve()
        {
            base.OnDependenciesResolve();

            _playersDeathController.PlayerDied.TakeUntilDestroy(this).Subscribe((OnPlayerDied));
        }

        private void OnPlayerDied(PlayerDeathContext deathContext)
        {
            PlayerDied.OnNext(deathContext.PlayerRef);
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