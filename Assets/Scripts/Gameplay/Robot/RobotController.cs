using RecoDeli.Scripts.Prototyping;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private AnimationCurve rotationCurve;
        [SerializeField] private float propulsionForce;
        [SerializeField] private float propulsionRotationDrag;

        public int CurrentInstructionIndex { get; private set; }
        public bool ExecutingInstructions { get; private set; }
        public RobotInstruction CurrentInstruction => 
            (ExecutingInstructions && CurrentInstructionIndex >= 0 && CurrentInstructionIndex < CurrentInstructions.Count)
            ? CurrentInstructions[CurrentInstructionIndex] 
            : null;

        public List<RobotInstruction> CurrentInstructions { get; set; }

        public float RotationSpeed => rotationSpeed;
        public float PropulsionForce => propulsionForce;
        public float PropulsionRotationDrag => propulsionRotationDrag;
        public AnimationCurve RotationCurve => rotationCurve;

        public Rigidbody Rigidbody { get; private set; }
        public GoalBox ReachedGoalBox { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Rigidbody.angularDrag = RotationMethodSelector.ShouldUseDragMethod ? 0.1f : 0.0f;
        }

        private IEnumerator CommandExecutionCoroutine()
        {
            if (CurrentInstructions.Count == 0) yield break;
            ExecutingInstructions = true;
            CurrentInstructionIndex = 0;
            yield return new WaitForFixedUpdate();
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

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(
                $"Contact with a prop.\n"+
                $"Robot position: {Rigidbody.position.ToString("N10")}\n" +
                $"Box position: {collision.collider.attachedRigidbody.position.ToString("N10")}"
            );
        }
    }
}
