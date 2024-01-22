using System;
using UniRx;
using UnityEngine;

namespace Dev.Levels.Interactions
{
    public class SpawnPoint : MonoBehaviour
    {
        public bool IsBusy { get; private set; }

        public Vector3 SpawnPos => transform.position;

        public void UseSpawnPoint()
        {
            IsBusy = true;
            
            Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe((l =>
            {
                IsBusy = false;
            }));
        }
    }
}