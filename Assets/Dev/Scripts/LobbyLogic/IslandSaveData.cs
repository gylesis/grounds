using System;

namespace Dev.Scripts.LobbyLogic
{
    [Serializable]
    public class IslandSaveData
    {
        public int OwnerId;
        public ItemContainerSaveData[] ItemsContainers;
    }   
}