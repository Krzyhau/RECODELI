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

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out RobotController controller))
            {
                OnRobotCollection.Invoke();

                if (isFinalGoalBox)
                {
                    controller.ReachedGoalBox = this;
                }
            }
            
        }
    }
}
