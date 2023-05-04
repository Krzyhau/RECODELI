using LudumDare.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private AnimationCurve rotationCurve;
        [SerializeField] private float propulsionForce;

        public int CurrentInstructionIndex { get; private set; }
        public bool ExecutingInstructions { get; private set; }
        public RobotInstruction CurrentInstruction => ExecutingInstructions ? CurrentInstructions[CurrentInstructionIndex] : null;

        public List<RobotInstruction> CurrentInstructions { get; set; }

        public float RotationSpeed => rotationSpeed;
        public float PropulsionForce => propulsionForce;
        public AnimationCurve RotationCurve => rotationCurve;

        public Rigidbody Rigidbody { get; private set; }

        public GoalBox ReachedGoalBox { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private IEnumerator CommandExecutionCoroutine()
        {
            ExecutingInstructions = true;
            CurrentInstructionIndex = 0;
            while (CurrentInstructionIndex < CurrentInstructions.Count)
            {
                yield return CurrentInstruction.Execute(this);
                CurrentInstructionIndex++;
            }
            ExecutingInstructions = false;
            CurrentInstructionIndex = -1;
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

            StartCoroutine(CommandExecutionCoroutine());

            return true;
        }
    }
}
