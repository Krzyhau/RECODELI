using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UVector3 = UnityEngine.Vector3;
using UQuaternion = UnityEngine.Quaternion;
using BVector3 = BEPUutilities.Vector3;
using BQuaternion = BEPUutilities.Quaternion;
using System.Linq;
using BEPUphysics.CollisionShapes;
using BEPUphysics.BroadPhaseEntries;

namespace BEPUphysics.Unity
{
    public class BepuRigidbody : MonoBehaviour, IBepuEntity
    {
        [SerializeField] private float mass = 1.0f;
        [SerializeField] private bool kinematic = false;

        private BepuSimulation simulation;
        private Entity physicsEntity;
        private UVector3 previousRenderPosition;
        private UQuaternion previousRenderRotation;
        private UVector3 previousLocalScale;

        private UVector3 previousPhysicsPosition;
        private UVector3 currentPhysicsPosition;
        private UQuaternion previousPhysicsRotation = UQuaternion.identity;
        private UQuaternion currentPhysicsRotation = UQuaternion.identity;

        private BVector3 localOffset;

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
        public bool Initialized => Entity != null;

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
            if(previousRenderPosition != transform.position || previousRenderRotation != transform.rotation)
            {
                physicsEntity.Orientation = transform.rotation.ToBEPU();
                currentPhysicsRotation = physicsEntity.Orientation.ToUnity();
                previousPhysicsRotation = physicsEntity.Orientation.ToUnity();

                var offset = BQuaternion.Transform(localOffset * transform.lossyScale.ToBEPU(), physicsEntity.Orientation);
                physicsEntity.Position = transform.position.ToBEPU() + offset;
                currentPhysicsPosition = physicsEntity.Position.ToUnity();
                previousPhysicsPosition = physicsEntity.Position.ToUnity();
            }
            else
            {
                var offset = BQuaternion.Transform(localOffset * transform.lossyScale.ToBEPU(), physicsEntity.Orientation);
                transform.position = UVector3.Lerp(previousPhysicsPosition, currentPhysicsPosition, simulation.InterpolationTime);
                transform.position -= offset.ToUnity();
                transform.rotation = UQuaternion.Lerp(previousPhysicsRotation, currentPhysicsRotation, simulation.InterpolationTime);
            }

            previousRenderPosition = transform.position;
            previousRenderRotation = transform.rotation;
        }

        private void OnDestroy()
        {
            ClearPhysicsEntity();
        }

        private void ClearPhysicsEntity()
        {
            if (Initialized)
            {
                Entity.Space.Remove(Entity);
                physicsEntity = null;
            }
        }

        public void Initialize(BepuSimulation simulation)
        {
            // deinitialize previous state of the entity
            ClearPhysicsEntity();

            this.simulation = simulation;


            var collider = GetComponent<Collider>();

            var scale = transform.lossyScale.ToBEPU();
            switch (collider)
            {
                case BoxCollider boxCollider:
                    var boxSize = scale * boxCollider.size.ToBEPU();
                    physicsEntity = new Box(BVector3.Zero, boxSize.X, boxSize.Y, boxSize.Z);
                    localOffset = boxCollider.center.ToBEPU();
                    break;
                case SphereCollider sphereCollider:
                    if (scale.X != scale.Y || scale.X != scale.Z)
                    {
                        Debug.LogWarning($"Non-uniform scale for SphereCollider {gameObject}! {scale.X} {scale.Y} {scale.Z}");
                    }
                    var sphereRadius = (scale.X + scale.Y + scale.Z) / (sfloat)3.0f * (sfloat)sphereCollider.radius;
                    physicsEntity = new Sphere(BVector3.Zero, sphereRadius);
                    localOffset = sphereCollider.center.ToBEPU();
                    break;
                case MeshCollider meshCollider:
                    var mesh = meshCollider.sharedMesh;
                    var vertices = mesh.vertices
                        .Select(vertex => vertex.ToBEPU() * scale)
                        .ToArray();
                    var indices = mesh.GetIndices(0);
                    var defaultTransform = new AffineTransform(BVector3.One, BQuaternion.Identity, BVector3.Zero);
                    physicsEntity = new MobileMesh(vertices, indices, defaultTransform, MobileMeshSolidity.DoubleSided);
                    break;
            }

            if (physicsEntity == null)
            {
                if(collider != null)
                {
                    Debug.LogWarning($"BepuRigidbody in game object {gameObject} has unsupported collider type {collider.GetType().Name}!");
                }
                else
                {
                    Debug.LogWarning($"BepuRigidbody in game object {gameObject} doesn't have a collider!");
                }
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
