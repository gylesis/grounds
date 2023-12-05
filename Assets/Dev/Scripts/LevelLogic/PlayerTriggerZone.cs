using Dev.PlayerLogic;
using Dev.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dev.Levels.Interactions
{
    public class PlayerTriggerZone : TriggerZone
    {
        [SerializeField] private TriggerZone _triggerZone;

        public Subject<PlayerCharacter> PlayerEntered { get; } = new Subject<PlayerCharacter>();
        public Subject<PlayerCharacter> PlayerExit { get; } = new Subject<PlayerCharacter>();

        protected override void ServerSubscriptions()
        {
            base.ServerSubscriptions();

            _triggerZone.TriggerEntered.TakeUntilDestroy(this).Subscribe((OnZoneEntered));
            _triggerZone.TriggerExit.TakeUntilDestroy(this).Subscribe((OnZoneExit));
        }

        private void OnZoneEntered(Collider obj)
        {   
            if (obj.CompareTag("Player"))
            {
                Debug.Log($"Player entered zone", obj);
                PlayerCharacter playerCharacter = obj.GetComponent<PlayerCharacter>();

                PlayerEntered.OnNext(playerCharacter);
            }
        }

        private void OnZoneExit(Collider obj)
        {
            if (obj.CompareTag("Player"))
            {
                PlayerCharacter playerCharacter = obj.GetComponent<PlayerCharacter>();

                PlayerExit.OnNext(playerCharacter);
            }
        }
    }
}