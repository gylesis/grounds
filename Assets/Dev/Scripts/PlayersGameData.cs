using System.Collections.Generic;
using Fusion;

namespace Dev.Scripts
{
    public class PlayersGameData
    {
        private static List<PlayerRef> _playersQueue = new List<PlayerRef>(8);
        private static List<PlayerRef> _alivePlayers = new List<PlayerRef>(8);

        public static List<PlayerRef> PlayersQueue => _playersQueue;

        public static List<PlayerRef> AlivePlayers => _alivePlayers;

        public static int CountPlayersInQueue => _playersQueue.Count;
            
        public static void PutPlayerInQueue(PlayerRef playerRef)
        {
            _playersQueue.Add(playerRef);            
        }

        public static void RemovePlayerFromQueue(PlayerRef playerRef)
        {
            _playersQueue.Remove(playerRef);
        }

        public static void AddAlivePlayer(PlayerRef playerRef)
        {
            _alivePlayers.Add(playerRef);
        }
        
        public static void RemoveAlivePlayer(PlayerRef playerRef)
        {
            _alivePlayers.Add(playerRef);
        }
        
    }
}