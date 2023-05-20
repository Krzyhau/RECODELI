
using BEPUphysics.Entities;
using SoftFloat;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BEPUphysics.CollisionShapes;
using BEPUphysics.Materials;

using UVector3 = UnityEngine.Vector3;
using UQuaternion = UnityEngine.Quaternion;
using BVector3 = BEPUutilities.Vector3;
using BQuaternion = BEPUutilities.Quaternion;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Unity
{
    public class BepuRigidbody : MonoBehaviour, IBepuEntity
    {
        [SerializeField] private float mass = 1.0f;
        [SerializeField] private bool kinematic = false;
        [SerializeField] private float angularDamping = .15f;
        [SerializeField] private float linearDamping = .03f;
        [Header("Material")]
        [SerializeField] private float kineticFriction = (float)MaterialManager.DefaultKineticFriction;
        [SerializeField] private float staticFriction = (float)MaterialManager.DefaultStaticFriction;
        [SerializeField] private float bounciness = (float)MaterialManager.DefaultBounciness;
        [Header("Constraints")]
        [SerializeField] private BepuPhysicsAxisConstraints fixedPosition;
        [SerializeField] private BepuPhysicsAxisConstraints fixedRotation;

        private BepuSimulation simulation;
        private Entity physicsEntity;
        private UVector3 previousRenderPosition;
        private UQuaternion previousRenderRotation;
        private UVector3 previousLocalScale;

        private BVector3 previousPhysicsPosition;
        private BVector3 currentPhysicsPosition;
        private BQuaternion previousPhysicsRotation = BQuaternion.Identity;
        private BQuaternion currentPhysicsRotation = BQuaternion.Identity;

        private BVector3 massCenterOffset;

        private IBepuEntityListener[] listeners;
        public float Mass
        {
            get { return mass; }
            set
            {
                mass = value;
                if (Entity != null) Entity.Mass = (sfloat)value;
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
        
        public GameObject GameObject => gameObject;
        public bool Initialized => Entity != null;

        private void Awake()
        {
            if (transform.parent.GetComponent<BepuSimulation>() == null)
            {
                Debug.LogWarning($"BepuRigidbody {gameObject} has to be a direct child of BepuSimulation!");
            }
        }

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
            if(previousRenderPosition != transform.localPosition || previousRenderRotation != transform.localRotation)
            {
                physicsEntity.Orientation = transform.localRotation.ToBEPU();
                currentPhysicsRotation = physicsEntity.Orientation;
                previousPhysicsRotation = physicsEntity.Orientation;

                var offset = BQuaternion.Transform(massCenterOffset, physicsEntity.Orientation);

                physicsEntity.Position = transform.localPosition.ToBEPU() + offset;
                currentPhysicsPosition = physicsEntity.Position;
                previousPhysicsPosition = physicsEntity.Position;
            }
            else
            {
                var offset = BQuaternion.Transform(massCenterOffset, physicsEntity.Orientation);
                transform.localPosition = UVector3.Lerp(previousPhysicsPosition.ToUnity(), currentPhysicsPosition.ToUnity(), simulation.InterpolationTime);
                transform.localPosition -= offset.ToUnity();
                transform.localRotation = UQuaternion.Lerp(previousPhysicsRotation.ToUnity(), currentPhysicsRotation.ToUnity(), simulation.InterpolationTime);
            }

            previousRenderPosition = transform.localPosition;
            previousRenderRotation = transform.localRotation;
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


            var colliders = GetComponents<Collider>();

            var shapeEntries = new List<CompoundShapeEntry>();
            foreach (var collider in colliders)
            {
                var collidable = collider.ToBEPUCollideable();
                if (collidable == null) continue;
                shapeEntries.Add(new CompoundShapeEntry(
                    collidable.Shape,
                    collidable.WorldTransform
                ));
            }

            if (shapeEntries.Count() == 0)
            {
                Debug.LogWarning($"BepuRigidbody in game object {gameObject} doesn't have a collider!");
                return;
            }

            physicsEntity = new Entity(new CompoundShape(shapeEntries, out massCenterOffset));
            physicsEntity.Tag = this;
            physicsEntity.CollisionInformation.Tag = this;
            physicsEntity.Material = new Materials.Material
            {
                StaticFriction = (sfloat)staticFriction,
                KineticFriction = (sfloat)kineticFriction,
                Bounciness = (sfloat)bounciness
            };
            physicsEntity.AngularDamping = (sfloat)angularDamping;
            physicsEntity.LinearDamping = (sfloat)linearDamping;

            simulation.PhysicsSpace.Add(physicsEntity);

            RegisterEvents();
            OnValidate();
            SyncTransform();
        }

        private void RegisterEvents()
        {
            if (physicsEntity == null) return;

            physicsEntity.CollisionInformation.Events.InitialCollisionDetected += OnBepuCollisionEnter;
            physicsEntity.CollisionInformation.Events.PairTouched += OnBepuCollisionStay;
            physicsEntity.CollisionInformation.Events.CollisionEnded += OnBepuCollisionExit;

            listeners = GetComponents<IBepuEntityListener>();
        }

        private void OnBepuCollisionEnter(EntityCollidable self, Collidable other, CollidablePairHandler pair)
        {
            foreach (var listener in listeners) listener.OnBepuCollisionEnter(other, pair);
        }

        private void OnBepuCollisionStay(EntityCollidable self, Collidable other, CollidablePairHandler pair)
        {
            foreach (var listener in listeners) listener.OnBepuCollisionStay(other, pair);
        }

        private void OnBepuCollisionExit(EntityCollidable self, Collidable other, CollidablePairHandler pair)
        {
            foreach (var listener in listeners) listener.OnBepuCollisionExit(other, pair);
        }



        public void PhysicsUpdate()
        {
            foreach (var listener in listeners) listener.BepuUpdate();
        }

        public void PostPhysicsUpdate()
        {
            // apply position constrains
            if (fixedPosition.X)
            {
                Entity.position.X = currentPhysicsPosition.X;
                Entity.linearVelocity.X = sfloat.Zero;
            }
            if (fixedPosition.Y)
            {
                Entity.position.Y = currentPhysicsPosition.Y;
                Entity.linearVelocity.Y = sfloat.Zero;
            }
            if (fixedPosition.Z)
            {
                Entity.position.Z = currentPhysicsPosition.Z;
                Entity.linearVelocity.Z = sfloat.Zero;
            }

            // apply rotation constrains
            if (fixedRotation.Any)
            {
                var rotationDelta = BQuaternion.Inverse(currentPhysicsRotation) * Entity.Orientation;
                BQuaternion.GetAxisAngleFromQuaternion(ref rotationDelta, out var axis, out var angle);

                if (fixedRotation.X)
                {
                    axis.X = sfloat.Zero;
                    Entity.angularVelocity.X = sfloat.Zero;
                }
                if (fixedRotation.Y)
                {
                    axis.Y = sfloat.Zero;
                    Entity.angularVelocity.Y = sfloat.Zero;
                }
                if (fixedRotation.Z)
                {
                    axis.Z = sfloat.Zero;
                    Entity.angularVelocity.Z = sfloat.Zero;
                }
                rotationDelta = BQuaternion.Identity;
                var axisLength = axis.Length();
                if (axisLength > sfloat.Zero)
                {
                    angle *= axisLength;
                    axis.Normalize();
                    rotationDelta = BQuaternion.CreateFromAxisAngle(axis, angle);
                }
                Entity.Orientation = rotationDelta * currentPhysicsRotation;
            }

            // cache previous and current physics positions for rendering
            previousPhysicsPosition = currentPhysicsPosition;
            previousPhysicsRotation = currentPhysicsRotation;
            currentPhysicsPosition = Entity.Position;
            currentPhysicsRotation = Entity.Orientation;
        }
    }
}
