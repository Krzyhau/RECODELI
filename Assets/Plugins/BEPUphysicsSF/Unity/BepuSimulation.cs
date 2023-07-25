using SoftFloat;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace BEPUphysics.Unity
{
    public class BepuSimulation : MonoBehaviour
    {
        [SerializeField] private float timeStep = 0.02f;
        [SerializeField] private float maxUpdateRealTimeWindow = 0.1f;
        [SerializeField] private int solverIterationLimit = 6;
        [SerializeField] private bool activateOnAwake = false;
        [SerializeField] private Vector3 gravity = Vector3.down * 9.81f;

        private bool initialised = false;
        private float timeSinceLastStep;
        private sfloat simulationTime;

        private Space space;
        private List<IBepuEntity> rigidbodies = new List<IBepuEntity>();

        public Action OnInitialized;
        public Action OnPhysicsUpdate;
        public Action OnPostPhysicsUpdate;

        public bool Active { get; set; }
        public Space PhysicsSpace => space;
        public float InterpolationTime => timeSinceLastStep / timeStep;
        public sfloat SimulationTime => simulationTime;
        public sfloat TimeStep => (sfloat)timeStep;
        public List<IBepuEntity> Rigidbodies => rigidbodies;

        private void Awake()
        {
            if(activateOnAwake) Initialize();
        }

        public void Initialize()
        {
            if (initialised) return;

            space = new Space();

            space.Solver.IterationLimit = solverIterationLimit;

            space.TimeStepSettings = new TimeStepSettings()
            {
                MaximumTimeStepsPerFrame = 1,
                TimeStepDuration = (sfloat)timeStep
            };
            space.ForceUpdater.Gravity = gravity.ToBEPU();

            InitializeRigidbodiesAndColliders();

            initialised = true;
            if (activateOnAwake) Active = true;

            Debug.Log($"Initialised BepuSimulation. Rigidbodies count: {rigidbodies.Count}");

            if (OnInitialized != null) OnInitialized.Invoke();
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
            if (!initialised || !Active) return;

            if(Time.timeScale == 0.0f)
            {
                return;
            }

            timeSinceLastStep += Time.deltaTime;
            var updateBeginTime = DateTime.Now;

            while (timeSinceLastStep > timeStep)
            {
                Profiler.BeginSample("BEPUphysics Simulation Step");
                foreach (var rigidbody in rigidbodies) rigidbody.PhysicsUpdate();
                if (OnPhysicsUpdate != null) OnPhysicsUpdate.Invoke();
                space.Update();
                foreach (var rigidbody in rigidbodies) rigidbody.PostPhysicsUpdate();
                if (OnPostPhysicsUpdate != null) OnPostPhysicsUpdate.Invoke();
                Profiler.EndSample();

                simulationTime += (sfloat)timeStep;

                timeSinceLastStep -= timeStep;

                var duration = DateTime.Now - updateBeginTime;
                if(duration.TotalSeconds > maxUpdateRealTimeWindow)
                {
                    timeSinceLastStep %= timeStep;
                    break;
                }

                // special case: if we're paused mid-multiple iterations,
                // make sure to not iterate further.
                if(Time.timeScale == 0.0f)
                {
                    break;
                }
            }
        }

        public void RemoveEntity(IBepuEntity entity)
        {
            rigidbodies.Remove(entity);
            PhysicsSpace.Remove(entity.Entity);
        }
    }
}
