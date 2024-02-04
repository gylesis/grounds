using Dev.Scripts.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.LobbyLogic
{
    public class IslandCloudsController : NetworkContext
    {
        [SerializeField] private IslandCloudWall _leftCloudWall;
        [SerializeField] private IslandCloudWall _rightCloudWall;
  
        
        public void RemoveWall(bool isLeft)
        {
            Transform island;
            
            if (isLeft)
            {
                island = _leftCloudWall.transform;
                _leftCloudWall.StartAnimation();
            }
            else
            {
                island = _rightCloudWall.transform;
                _rightCloudWall.StartAnimation();
            }
            
            island.gameObject.SetActive(false);
            
        }
        
    }
}