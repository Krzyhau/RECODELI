using Codice.CM.Client.Differences;
using Jitter.Collision;
using Jitter.Dynamics;
using Jitter.LinearMath;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Jitter.Unity
{
    public class JitterSimulation : MonoBehaviour
    {
        [SerializeField] private float timeStep;
        [SerializeField] private int maxStepsPerFrame;
        [SerializeField] private bool activateOnAwake;

        private bool initialised = false;
        private float timeSinceLastStep;

        private CollisionSystem collisionSystem;
        private World world;
        private List<JitterRigidbody> rigidbodies = new List<JitterRigidbody>();

        public bool Active { get; set; }
        public CollisionSystem CollisionSystem => collisionSystem;
        public World PhysicsWorld => world;

        private void Awake() => Initialize();
        private void Initialize()
        {
            if (initialised) return;

            collisionSystem = new CollisionSystemSAP();
            collisionSystem.CollisionDetected += CollisionDetected;
            world = new World(collisionSystem);
            //world.Gravity = (JVector)Physics.gravity;

            rigidbodies = GetComponentsInChildren<JitterRigidbody>().ToList();

            foreach(var rigidbody in rigidbodies)
            {
                rigidbody.Initialize(this);
            }

            initialised = true;
            if (activateOnAwake) Active = true;

            Debug.Log($"Initialised JitterSimulation. Rigidbodies count: {rigidbodies.Count}");
        }

        private void Update()
        {
            if (!Active) return;

            timeSinceLastStep += Time.deltaTime;
            if(timeSinceLastStep > timeStep)
            {
                world.Step((sfloat)timeSinceLastStep, false, (sfloat)timeStep, maxStepsPerFrame);
                timeSinceLastStep = 0.0f;
            }
        }

        private void CollisionDetected(RigidBody body1, RigidBody body2, JVector point1, JVector point2, JVector normal, sfloat penetration)
        {
            Debug.Log($"COLLISION!!! {normal} {penetration}");
        }
    }
}
