using LudumDare.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare.Scripts.Components
{
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float propulsionForce;

        public float CurrentPropellingForce { get; set; }
        public float PitchVelocity { get; set; }
        public float YawVelocity { get; set; }

        public int CurrentCommandIndex { get; private set; }
        public bool ExecutingCommands { get; private set; }

        public List<RobotAction> CurrentCommands { get; set; }

        public float RotationSpeed => rotationSpeed;
        public float PropulsionForce => propulsionForce;

        public Rigidbody Rigidbody { get; private set; }

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();

            // dla testu
            ExecuteCommands(new List<RobotAction>()
        {
            new ForwardAction(1.0f),
            new BackAction(3.0f),
            new TurnLeftAction(0.5f),
            new WaitAction(2.0f),
        });


            CurrentPropellingForce = 0;
        }

        private void FixedUpdate()
        {
            HandleRotation();
            HandlePropelling();
        }

        private void HandleRotation()
        {
            Rigidbody.angularVelocity = new Vector3(PitchVelocity, YawVelocity, 0.0f) * Time.fixedDeltaTime;
        }

        private void HandlePropelling()
        {
            Rigidbody.AddForce(transform.forward * propulsionForce, ForceMode.Acceleration);
        }

        private IEnumerator CommandExecutionCoroutine(List<RobotAction> commands)
        {
            ExecutingCommands = true;
            CurrentCommandIndex = 0;
            while (CurrentCommandIndex < commands.Count)
            {
                var command = commands[CurrentCommandIndex];
                yield return command.Execute(this);
                CurrentCommandIndex++;
            }
            yield return null;
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
            if (ExecutingCommands) return false;

            StartCoroutine(CommandExecutionCoroutine(CurrentCommands));

            return true;
        }
    }
}
