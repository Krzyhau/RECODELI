using BEPUphysics.BroadPhaseEntries;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace BEPUphysics.Unity
{
    public class BepuSimulation : MonoBehaviour
    {
        [SerializeField] private float timeStep;
        [SerializeField] private int maxStepsPerFrame;
        [SerializeField] private int solverIterationLimit;
        [SerializeField] private bool activateOnAwake;
        [SerializeField] private Vector3 gravity = Vector3.down * 9.81f;

        private bool initialised = false;
        private float timeSinceLastStep;
        private sfloat simulationTime;

        private Space space;
        private List<IBepuEntity> rigidbodies = new List<IBepuEntity>();

        public bool Active { get; set; }
        public Space PhysicsSpace => space;
        public float InterpolationTime => timeSinceLastStep / timeStep;
        public sfloat SimulationTime => simulationTime;

        private void Awake() => Initialize();
        private void Initialize()
        {
            if (initialised) return;

            space = new Space();

            space.Solver.IterationLimit = solverIterationLimit;

            space.TimeStepSettings = new TimeStepSettings()
            {
                MaximumTimeStepsPerFrame = maxStepsPerFrame,
                TimeStepDuration = (sfloat)timeStep
            };
            space.ForceUpdater.Gravity = gravity.ToBEPU();

            InitializeRigidbodiesAndColliders();

            initialised = true;
            if (activateOnAwake) Active = true;

            Debug.Log($"Initialised BepuSimulation. Rigidbodies count: {rigidbodies.Count}");
        }
        private void InitializeRigidbodiesAndColliders()
        {
            // initialize custom-made rigidbodies
            rigidbodies = new List<IBepuEntity>();
            foreach(Transform child in transform)
            {
                if (!child.TryGetComponent<IBepuEntity>(out var bepuEntity)) continue;
                bepuEntity.Initialize(this);
                if (!bepuEntity.Initialized) continue;
                rigidbodies.Add(bepuEntity);
            }

            // initialize static colliders
            //var staticCollidersWithoutRigidbodies = GetComponentsInChildren<Collider>()
            //    .Where(collider => collider.GetComponent<BepuRigidbody>() == null)
            //    .Select(collider => collider.ToBEPUCollideable(true)).ToList();

            //space.Add(new StaticGroup(staticCollidersWithoutRigidbodies));
        }

        private void Update()
        {
            if (!Active) return;

            timeSinceLastStep += Time.deltaTime;
            int updates = 0;
            while (timeSinceLastStep > timeStep)
            {
                Profiler.BeginSample("BEPUphysics Simulation Step");
                foreach (var rigidbody in rigidbodies) rigidbody.PhysicsUpdate();
                space.Update();
                foreach (var rigidbody in rigidbodies) rigidbody.PostPhysicsUpdate();
                Profiler.EndSample();

                simulationTime += (sfloat)timeStep;

                timeSinceLastStep -= timeStep;
                updates++;
                if(updates >= maxStepsPerFrame)
                {
                    timeSinceLastStep %= timeStep;
                    break;
                }
            }
        }
    }
}
