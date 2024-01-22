using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public interface IDamageVictim
    {
        public PlayerRef PlayerRef { get;}
        public void TakeDamage(float value, IDamageInflictor damageInflictor);
    }
}   