﻿using Fusion;
using UnityEngine;

namespace Dev.Infrastructure
{
    public struct PlayerInput : INetworkInput
    {
        public Vector2 MoveDirection;
        public Vector2 LookDirection;
        
        public int WeaponNum;
    }
}