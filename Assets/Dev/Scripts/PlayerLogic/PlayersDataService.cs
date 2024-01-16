using Dev.Infrastructure;
using Dev.PlayerLogic;
using Fusion;

namespace Dev.Scripts.PlayerLogic
{
    public class PlayersDataService : NetworkContext
    {
        private PlayersSpawner _playersSpawner;

        private void Init(PlayersSpawner playersSpawner)
        {
            _playersSpawner = playersSpawner;
        }

        public PlayerCharacter GetPlayer(PlayerRef playerRef)
        {
            PlayerCharacter playerCharacter = Runner.GetPlayerObject(playerRef).GetComponent<PlayerCharacter>();

            return playerCharacter;
        }
        
    }
}