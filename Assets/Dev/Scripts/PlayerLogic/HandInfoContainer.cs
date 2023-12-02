
using Dev.Infrastructure;
using Fusion;
using UnityEngine;

namespace Dev.Scripts.PlayerLogic
{
    public class HandInfoContainer : NetworkContext
    {
        [SerializeField] private HandType _handType;
        [SerializeField] private NetworkObject _parent;
        
        public HandType HandType => _handType;
        public NetworkObject Parent => _parent;

        [Networked] public Item CarryingItem { get; set; }


        public bool IsFreeHand => CarryingItem == null;

    }
    
    
}       