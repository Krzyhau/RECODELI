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

        private bool initialised = false;
        private float timeSinceLastStep;

        private Space space;
        private List<IBepuEntity> rigidbodies = new List<IBepuEntity>();

        public bool Active { get; set; }
        public Space PhysicsSpace => space;
        public float InterpolationTime => timeSinceLastStep / timeStep;

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
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(sfloat.Zero, (sfloat)(-9.81f), sfloat.Zero);

            rigidbodies = GetComponentsInChildren<IBepuEntity>().ToList();

            foreach (var rigidbody in rigidbodies)
            {
                rigidbody.Initialize(this);
            }

            initialised = true;
            if (activateOnAwake) Active = true;

            Debug.Log($"Initialised BepuSimulation. Rigidbodies count: {rigidbodies.Count}");
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
