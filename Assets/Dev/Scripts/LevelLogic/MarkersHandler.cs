using Dev.Scripts.Infrastructure;
using UnityEngine;

namespace Dev.Scripts.LevelLogic
{
    public class MarkersHandler : NetworkContext
    {
        [SerializeField] private WorldMarker _worldMarkerPrefab;

        public void SpawnWorldMarkerAt(Vector3 pos)
        {
            WorldMarker worldMarker = Runner.Spawn(_worldMarkerPrefab, pos, Quaternion.identity);
        }
        
    }
}