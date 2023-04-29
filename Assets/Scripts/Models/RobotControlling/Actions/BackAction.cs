using System.Collections;
using UnityEngine;
using LudumDare.Scripts.Components;

namespace LudumDare.Scripts.Models
{
    public class BackAction : RobotAction
    {
        public override string Name => "BACK";

        public BackAction(float param) : base(param) 
        { 
        }

        public override IEnumerator Execute(RobotController controller)
        {
            controller.CurrentPropellingForce = -controller.PropulsionForce;

            yield return new WaitForSeconds(parameter);

            controller.CurrentPropellingForce = 0.0f;
        }
    }
}
