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
    public class BepuRigidbody : MonoBehaviour, IBepuEntity
    {
        [SerializeField] private float mass;
        [SerializeField] private bool kinematic;

        private BepuSimulation simulation;
        private Entity physicsEntity;
        private UnityEngine.Vector3 previousRenderPosition;
        private UnityEngine.Quaternion previousRenderRotation;
        private UnityEngine.Vector3 previousLocalScale;

        private UnityEngine.Vector3 previousPhysicsPosition;
        private UnityEngine.Vector3 currentPhysicsPosition;
        private UnityEngine.Quaternion previousPhysicsRotation = UnityEngine.Quaternion.identity;
        private UnityEngine.Quaternion currentPhysicsRotation = UnityEngine.Quaternion.identity;

        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                if(Entity != null) Entity.Mass = (sfloat)value;
            }
        }
        public bool Kinematic
        {
            get { return kinematic; }
            set
            {
                kinematic = value;
                if (Entity != null)
                {
                    if (kinematic) Entity.BecomeKinematic();
                    else Entity.BecomeDynamic(Entity.Mass);
                }
            }
        }
        public Entity Entity => physicsEntity;
        public BepuSimulation Simulation => simulation;

        private void LateUpdate()
        {
            if (Entity == null) return;

            SyncTransform();
        }

        private void OnValidate()
        {
            Mass = mass;
            Kinematic = kinematic;
        }

        private void SyncTransform()
        {
            if(previousRenderPosition != transform.position)
            {
                physicsEntity.Position = transform.position.ToBEPU();
                currentPhysicsPosition = transform.position;
                previousPhysicsPosition = transform.position;
            }
            else
            {
                transform.position = UnityEngine.Vector3.Lerp(previousPhysicsPosition, currentPhysicsPosition, simulation.InterpolationTime);
            }

            if(previousRenderRotation != transform.rotation)
            {
                physicsEntity.Orientation = transform.rotation.ToBEPU();
                currentPhysicsRotation = transform.rotation;
                previousPhysicsRotation = transform.rotation;
            }
            else
            {
                transform.rotation = UnityEngine.Quaternion.Lerp(previousPhysicsRotation, currentPhysicsRotation, simulation.InterpolationTime);
            }

            previousRenderPosition = transform.position;
            previousRenderRotation = transform.rotation;
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

            simulation.PhysicsSpace.Add(physicsEntity);

            OnValidate();
        }

        public void PhysicsUpdate()
        {

        }

        public void PostPhysicsUpdate()
        {
            previousPhysicsPosition = currentPhysicsPosition;
            previousPhysicsRotation = currentPhysicsRotation;
            currentPhysicsPosition = Entity.Position.ToUnity();
            currentPhysicsRotation = Entity.Orientation.ToUnity();
        }
    }
}
