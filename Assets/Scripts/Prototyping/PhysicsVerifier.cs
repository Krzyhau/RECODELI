using RecoDeli.Scripts.Controllers;
using RecoDeli.Scripts.Gameplay.Robot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RecoDeli.Scripts.Prototyping
{
    public class PhysicsVerifier : MonoBehaviour
    {
        private struct PhysicsSampleFrame
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }

        private Dictionary<float, PhysicsSampleFrame> firstPhysicsLog = new Dictionary<float, PhysicsSampleFrame>();
        private List<RobotInstruction> testedRobotInstructions = new List<RobotInstruction>();
        [SerializeField] private SimulationManager simulationManager;

        private bool logging = false;
        private bool verifying = false;
        private bool failed = false;

        private int verificationCount = 0;

        private void FixedUpdate()
        {
            if(simulationManager.PlayingSimulation && !logging && !verifying && !failed)
            {
                if (AreTestedInstructionsDifferent())
                {
                    testedRobotInstructions = new List<RobotInstruction>();
                    foreach(var instruction in simulationManager.RobotController.CurrentInstructions)
                    {
                        testedRobotInstructions.Add((RobotInstruction)instruction.Clone());
                    }
                    firstPhysicsLog.Clear();
                    logging = true;
                    verificationCount = 0;
                    Debug.Log("Recording simulation log.");
                }
                else
                {
                    verifying = true;
                    verificationCount++;
                    Debug.Log($"Verification no. {verificationCount} started.");
                }
            }

            if (!simulationManager.PlayingSimulation)
            {
                if (logging)
                {
                    Debug.Log("Simulation log ended.");
                }
                if (verifying)
                {
                    Debug.Log($"Verification no. {verificationCount} ended.");
                }

                logging = false;
                verifying = false;
                failed = false;

                return;
            }

            var robotPos = simulationManager.RobotController.Rigidbody.position;
            var robotRot = simulationManager.RobotController.Rigidbody.rotation.eulerAngles;

            if (logging)
            {
                firstPhysicsLog.Add(simulationManager.SimulationTime, new PhysicsSampleFrame
                {
                    Position = robotPos,
                    Rotation = robotRot
                });
            }

            if (verifying && !failed)
            {
                
                if (firstPhysicsLog.TryGetValue(simulationManager.SimulationTime, out var sample))
                {
                    if(sample.Position != robotPos || sample.Rotation != robotRot)
                    {
                        Debug.Log(
                            $"Inconsistency in robot's position detected at {simulationManager.SimulationTime}!\n" + 
                            $"Expected position: {sample.Position.ToString("F10")}, got: {robotPos.ToString("F10")}\n"+
                            $"Expected rotation: {sample.Rotation.ToString("F10")}, got {robotRot.ToString("F10")}"
                        );

                        failed = true;
                    }
                }
            }
        }

        private bool AreTestedInstructionsDifferent()
        {
            var currentInstructions = simulationManager.RobotController.CurrentInstructions;

            if (testedRobotInstructions.Count() != currentInstructions.Count())
            {
                return true;
            }

            for (int i = 0; i < testedRobotInstructions.Count(); i++)
            {
                if (!testedRobotInstructions[i].Equals(currentInstructions[i])) return true;
            }

            return false;
        }
    }
}
