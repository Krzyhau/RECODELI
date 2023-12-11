using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay
{
    public enum PhysicsMaterialType
    {
        Cardboard,
        Wood,
        Plastic,
        Metal,
        Container
    }

    public class PhysicsElement : MonoBehaviour
    {
        [SerializeField] private PhysicsMaterialType material;
        [SerializeField] private bool flammable;

        public PhysicsMaterialType Material => material;
        public bool Flammable => flammable;
    }
}
