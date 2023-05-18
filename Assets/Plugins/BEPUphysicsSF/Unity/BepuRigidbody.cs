using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BEPUphysics.Unity
{
    public class BepuRigidbody : MonoBehaviour
    {
        [SerializeField] private float mass;
        [SerializeField] private bool kinematic;

        private BepuSimulation simulation;
        private Entity physicsEntity;

        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                Entity.Mass = (sfloat)value;
            }
        }
        public bool Kinematic
        {
            get { return kinematic; }
            set
            {
                kinematic = value;
                if (kinematic) Entity.BecomeKinematic();
                else Entity.BecomeDynamic(Entity.Mass);
            }
        }
        public Entity Entity => physicsEntity;
        public BepuSimulation Simulation => simulation;

        private void Update()
        {
            if (Entity == null) return;
            transform.position = new UnityEngine.Vector3(
                (float)Entity.position.X,
                (float)Entity.position.Y,
                (float)Entity.position.Z
            );
            transform.rotation = new UnityEngine.Quaternion(
                (float)Entity.Orientation.X,
                (float)Entity.Orientation.Y,
                (float)Entity.Orientation.Z,
                (float)Entity.Orientation.W
            );
        }

        public void Initialize(BepuSimulation simulation)
        {
            this.simulation = simulation;


            var collider = GetComponent<Collider>();
            if (collider is BoxCollider)
            {
                var boxCollider = (BoxCollider)collider;
                var size = UnityEngine.Vector3.Scale(transform.lossyScale, boxCollider.size);
                physicsEntity = new Box(BEPUutilities.Vector3.Zero, (sfloat)size.x, (sfloat)size.y, (sfloat)size.z);

            }

            if (physicsEntity == null)
            {
                Debug.LogWarning($"BepuRigidbody in game object {gameObject} doesn't have a collider!");
                return;
            }

            physicsEntity.Position = new BEPUutilities.Vector3(
                (sfloat)transform.position.x,
                (sfloat)transform.position.y,
                (sfloat)transform.position.z
            );

            physicsEntity.Orientation = new BEPUutilities.Quaternion(
                (sfloat)transform.rotation.x,
                (sfloat)transform.rotation.y,
                (sfloat)transform.rotation.z,
                (sfloat)transform.rotation.w
            );

            simulation.PhysicsSpace.Add(physicsEntity);

            // re-set the properties so physics body can be set up properly
            Mass = mass;
            Kinematic = kinematic;
        }
    }
}
