using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using BEPUphysics.Unity;
using RecoDeli.Scripts.Gameplay.Robot;
using UnityEngine;
using UnityEngine.Events;

namespace RecoDeli.Scripts.Gameplay
{
    public class GoalBox : MonoBehaviour, IBepuEntityListener
    {
        public UnityEvent OnRobotCollection;

        [SerializeField] private bool isFinalGoalBox;

        private bool collected = false;

        public bool IsFinalGoalBox => isFinalGoalBox;

        public void BepuUpdate() { }

        public void OnBepuCollisionEnter(Collidable other, CollidablePairHandler info)
        {
            if (collected) return;
            if (!(other.Tag is IBepuEntity)) return;
            if ((other.Tag as IBepuEntity).GameObject.TryGetComponent(out RobotController controller))
            {
                OnRobotCollection.Invoke();

                if (isFinalGoalBox)
                {
                    controller.ReachedGoalBox = this;
                }
                collected = true;
            }
        }

        public void OnBepuCollisionExit(Collidable other, CollidablePairHandler info) { }
        public void OnBepuCollisionStay(Collidable other, CollidablePairHandler info) { }
    }
}
