using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public interface IDamageVictim
    {
        public GameObject GameObject { get;}
        public void TakeDamage(float value, IDamageInflictor damageInflictor);
    }
}