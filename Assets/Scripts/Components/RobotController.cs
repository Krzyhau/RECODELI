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

        public int CurrentCommandIndex { get; private set; }
        public bool ExecutingCommands { get; private set; }
        public RobotAction CurrentCommand => ExecutingCommands ? CurrentCommands[CurrentCommandIndex] : null;

        public List<RobotAction> CurrentCommands { get; set; }

        public float RotationSpeed => rotationSpeed;
        public float PropulsionForce => propulsionForce;
        public AnimationCurve RotationCurve => rotationCurve;

        public Rigidbody Rigidbody { get; private set; }

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();

            // dla testu
            ExecuteCommands(new List<RobotAction>()
            {
                new WaitAction(1.0f),
                new ForwardAction(1.0f),
                new WaitAction(1.0f),
                new BackwardAction(1.0f),
                new WaitAction(1.0f),
                new TurnLeftAction(1.0f),
                new WaitAction(1.0f),
                new TurnRightAction(1.0f),
            });
        }

        private IEnumerator CommandExecutionCoroutine()
        {
            ExecutingCommands = true;
            CurrentCommandIndex = 0;
            while (CurrentCommandIndex < CurrentCommands.Count)
            {
                yield return CurrentCommand.Execute(this);
                CurrentCommandIndex++;
            }
            ExecutingCommands = false;
        }


        public bool ExecuteCommands(List<RobotAction> commands)
        {
            if (ExecutingCommands) return false;

            CurrentCommands = commands;
            return ExecuteCommands();
        }

        public bool ExecuteCommands()
        {
            if (ExecutingCommands || CurrentCommands.Count < 0) return false;

            StartCoroutine(CommandExecutionCoroutine());

            return true;
        }
    }
}