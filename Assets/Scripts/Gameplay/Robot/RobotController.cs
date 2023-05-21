using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Unity;
using RecoDeli.Scripts.Prototyping;
using SoftFloat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class RobotController : MonoBehaviour, IBepuEntityListener
    {
        [SerializeField] private RobotThrusterController thrusterController;

        [Header("Properties")]
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float propulsionForce;
        [SerializeField] private float propulsionRotationDrag;
        [SerializeField] private float thrusterRepulsionForce;
        [SerializeField] private float thrusterRepulsionMaxDistance;

        private IEnumerator<int> currentInstructionExecution = null;
        private int currentInstructionWaitTicks = 0;

        public int CurrentInstructionIndex { get; private set; }
        public bool ExecutingInstructions { get; private set; }
        public RobotInstruction CurrentInstruction => 
            (ExecutingInstructions && CurrentInstructionIndex >= 0 && CurrentInstructionIndex < CurrentInstructions.Count)
            ? CurrentInstructions[CurrentInstructionIndex] 
            : null;

        public List<RobotInstruction> CurrentInstructions { get; set; }

        public BEPUutilities.Vector3 LinearAcceleration { get; set; }
        public BEPUutilities.Vector3 AngularAcceleration { get; set; }

        public float RotationSpeed => rotationSpeed;
        public float PropulsionForce => propulsionForce;
        public float PropulsionRotationDrag => propulsionRotationDrag;
        public float ThrusterRepulsionForce => thrusterRepulsionForce;
        public float ThrusterRepulsionMaxDistance => thrusterRepulsionMaxDistance;

        public BepuRigidbody Rigidbody { get; private set; }
        public GoalBox ReachedGoalBox { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<BepuRigidbody>();
        }

        private void Update()
        {
            //Debug.Log(Rigidbody.Entity.Orientation.EulerAngles * (sfloat)Mathf.Rad2Deg); 
        }

        public bool ExecuteCommands(List<RobotInstruction> commands)
        {
            if (ExecutingInstructions) return false;

            CurrentInstructions = commands;
            return ExecuteCommands();
        }

        public bool ExecuteCommands()
        {
            if (ExecutingInstructions || CurrentInstructions.Count < 0) return false;

            if (CurrentInstructions.Count != 0)
            {
                ExecutingInstructions = true;
                CurrentInstructionIndex = 0;
                currentInstructionWaitTicks = 0;
                currentInstructionExecution = CurrentInstruction.Execute(this);
            }

            return true;
        }

        public void StopExecution()
        {
            ExecutingInstructions = false;
            CurrentInstructionIndex = -1;
        }

        public void BepuUpdate()
        {
            if (!ExecutingInstructions) return;

            var linearVelociotyPreInstruction = Rigidbody.Entity.LinearVelocity;
            var angularVelociotyPreInstruction = Rigidbody.Entity.AngularVelocity;

            PerformInstructionStep();

            LinearAcceleration = Rigidbody.Entity.LinearVelocity - linearVelociotyPreInstruction;
            AngularAcceleration = Rigidbody.Entity.AngularVelocity - angularVelociotyPreInstruction;

            if (currentInstructionExecution == null) StopExecution();

            thrusterController.UpdateThrusters(this);
        }

        private void PerformInstructionStep()
        {
            if (!ExecutingInstructions) return;

            if (currentInstructionWaitTicks > 0) currentInstructionWaitTicks--;
            while (currentInstructionExecution != null && currentInstructionWaitTicks == 0)
            {
                while (currentInstructionExecution.MoveNext())
                {
                    currentInstructionWaitTicks = currentInstructionExecution.Current;
                    if (currentInstructionWaitTicks > 0) return; // wait for next step
                }

                // instruction ended
                currentInstructionWaitTicks = 0;
                currentInstructionExecution = null;
                CurrentInstructionIndex++;

                if (CurrentInstruction != null)
                {
                    currentInstructionExecution = CurrentInstruction.Execute(this);
                }
            }
        }
    }
}
