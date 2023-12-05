using Dev.Infrastructure;
using UniRx;
using UnityEngine;

namespace Dev.Levels.Interactions
{
    [RequireComponent(typeof(Collider))]
    public class TriggerZone : NetworkContext
    {
        public Subject<Collider> TriggerEntered { get; } = new Subject<Collider>();
        public Subject<Collider> TriggerExit { get; } = new Subject<Collider>();

        protected virtual void OnTriggerEnter(Collider other)
        {
            TriggerEntered.OnNext(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            TriggerExit.OnNext(other);
        }
    }
}