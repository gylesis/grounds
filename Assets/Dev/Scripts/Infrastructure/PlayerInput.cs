using Fusion;
using UnityEngine;

namespace Dev.Scripts.Infrastructure
{
    public struct PlayerInput : INetworkInput
    {
        public NetworkButtons Buttons;
        
        public Vector2 MoveDirection;
        public Vector2 LookDirection;

        public bool Sprint;
        public bool Jump;
    }
    
    enum Buttons {
        Jump = 0,
        Sprint = 1,
        Swing = 2,
        Throw = 3,
        AlternateHand = 4,
        PickItem = 5,
        DropItem = 6,
        UseItem,
        ToggleInventory,
        PutItemToInventory,
        ReloadWeapon
    }

}