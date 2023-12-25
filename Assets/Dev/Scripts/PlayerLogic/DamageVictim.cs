using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public interface IDamageVictim
    {
        public string GameObjectName { get;}
        public void TakeDamage(float value, IDamageInflictor damageInflictor);
    }
}