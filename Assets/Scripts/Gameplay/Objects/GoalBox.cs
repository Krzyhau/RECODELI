using RecoDeli.Scripts.Gameplay.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RecoDeli.Scripts.Gameplay
{
    public class GoalBox : MonoBehaviour
    {
        public UnityEvent OnRobotCollection;

        [SerializeField] private bool isFinalGoalBox;

        private bool collected = false;

        public bool IsFinalGoalBox => isFinalGoalBox;

        private void OnCollisionEnter(Collision collision)
        {
            if (collected) return;
            if (collision.collider.TryGetComponent(out RobotController controller))
            {
                OnRobotCollection.Invoke();

                if (isFinalGoalBox)
                {
                    controller.ReachedGoalBox = this;
                }
                collected = true;
            }
            
        }
    }
}
