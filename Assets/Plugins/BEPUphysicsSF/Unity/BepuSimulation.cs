using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BEPUphysics.Unity
{
    public class BepuSimulation : MonoBehaviour
    {
        [SerializeField] private float timeStep;
        [SerializeField] private int maxStepsPerFrame;
        [SerializeField] private bool activateOnAwake;

        private bool initialised = false;
        private float timeSinceLastStep;

        private Space space;
        private List<BepuRigidbody> rigidbodies = new List<BepuRigidbody>();

        public bool Active { get; set; }
        public Space PhysicsSpace => space;

        private void Awake() => Initialize();
        private void Initialize()
        {
            if (initialised) return;

            space = new Space();
            space.TimeStepSettings = new TimeStepSettings()
            {
                MaximumTimeStepsPerFrame = maxStepsPerFrame,
                TimeStepDuration = (sfloat)timeStep
            };
            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(sfloat.Zero, (sfloat)(-9.81f), sfloat.Zero);

            rigidbodies = GetComponentsInChildren<BepuRigidbody>().ToList();

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
            if (timeSinceLastStep > timeStep)
            {
                space.Update();
                timeSinceLastStep %= timeStep;
            }
        }
    }
}
