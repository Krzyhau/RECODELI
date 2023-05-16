using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jitter.Unity
{
    public class JitterRigidbody : MonoBehaviour
    {
        [SerializeField] private float mass;
        [SerializeField] private bool kinematic;

        private JitterSimulation simulation;
        private RigidBody physicsRigidbody;

        public float Mass
        {
            get { return mass; }
            set { 
                mass = value;
                PhysicsBody.Mass = (sfloat)value;
            }
        }
        public bool Kinematic
        {
            get { return kinematic; }
            set { 
                kinematic = value;
                PhysicsBody.IsStatic = value;
            }
        }
        public RigidBody PhysicsBody => physicsRigidbody;
        public JitterSimulation Simulation => simulation;

        private void Update()
        {
            if (PhysicsBody == null) return;
            transform.position = (Vector3)PhysicsBody.Position;
            transform.rotation = (Quaternion)PhysicsBody.Orientation;
        }

        private void OnValidate() 
        {
            if(PhysicsBody == null) return;
            PhysicsBody.Position = (JVector)transform.position;
            PhysicsBody.Orientation = (JMatrix)transform.rotation;
        }

        public void Initialize(JitterSimulation simulation)
        {
            this.simulation = simulation;

            Shape shape = null;

            var collider = GetComponent<Collider>();
            if (collider is BoxCollider)
            {
                var boxCollider = (BoxCollider)collider;
                var size = transform.TransformVector(boxCollider.size);
                shape = new BoxShape((JVector)size);
            }

            if (shape == null)
            {
                Debug.LogWarning($"JitterRigidbody in game object {gameObject} doesn't have a collider!");
                return;
            }

            physicsRigidbody = new RigidBody(shape);

            PhysicsBody.Position = (JVector)transform.position;
            PhysicsBody.Orientation = (JMatrix)transform.rotation;

            simulation.PhysicsWorld.AddBody(physicsRigidbody);

            // re-set the properties so physics body can be set up properly
            Mass = mass;
            Kinematic = kinematic;
        }
    }
}
