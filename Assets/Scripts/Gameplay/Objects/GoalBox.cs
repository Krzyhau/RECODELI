using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LudumDare.Scripts.Components
{
    public class GoalBox : MonoBehaviour
    {
        public UnityEvent OnRobotCollection;

        [SerializeField] private bool isFinalGoalBox;

        private bool collected = false;

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
