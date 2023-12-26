using System;
using UnityEngine;

namespace Dev.Scripts.Items
{
    public class DraggableObject : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;

        public Rigidbody Rigidbody => _rigidbody;
    }
}