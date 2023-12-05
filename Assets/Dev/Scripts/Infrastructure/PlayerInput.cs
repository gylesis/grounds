using Fusion;
using UnityEngine;

namespace Dev.Infrastructure
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
    }

}