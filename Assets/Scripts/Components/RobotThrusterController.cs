using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace LudumDare.Scripts.Components
{
    public class RobotThrusterController : MonoBehaviour
    {
        [Flags]
        private enum RobotThrusterFlag
        {
            None = 0,
            FrontLeft = 1,
            FrontRight = 2,
            BackLeft = 4,
            BackRight = 8,
        }

        private const RobotThrusterFlag ForwardThrust = RobotThrusterFlag.FrontLeft | RobotThrusterFlag.FrontRight;
        private const RobotThrusterFlag BackwardThrust = RobotThrusterFlag.BackLeft | RobotThrusterFlag.BackRight;
        private const RobotThrusterFlag LeftRotationThrust = RobotThrusterFlag.FrontLeft | RobotThrusterFlag.BackRight;
        private const RobotThrusterFlag RightRotationThrust = RobotThrusterFlag.BackLeft | RobotThrusterFlag.FrontRight;


        [SerializeField] private ParticleSystem frontLeftThruster;
        [SerializeField] private ParticleSystem frontRightThruster;
        [SerializeField] private ParticleSystem backLeftThruster;
        [SerializeField] private ParticleSystem backRightThruster;

        [SerializeField] private RobotController controller;

        private void FixedUpdate()
        {
            SwitchParticleSystem(frontLeftThruster, RobotThrusterFlag.FrontLeft);
            SwitchParticleSystem(frontRightThruster, RobotThrusterFlag.FrontRight);
            SwitchParticleSystem(backLeftThruster, RobotThrusterFlag.BackLeft);
            SwitchParticleSystem(backRightThruster, RobotThrusterFlag.BackRight);
        }

        private void SwitchParticleSystem(ParticleSystem particles, RobotThrusterFlag particleFlag)
        {
            var newState = (GetThrusterFlags() & particleFlag) > 0;
            var oldState = particles.isPlaying;

            if (newState == oldState)
            {
                //  _______   _____/  |_ __ _________  ____   /\ 
                //  \_  __ \_/ __ \   __\  |  \_  __ \/    \  \/ 
                //   |  | \/\  ___/|  | |  |  /|  | \/   |  \ /\ 
                //   |__|    \___  >__| |____/ |__|  |___|  / )/
                //               \/                       \/
                return;
            }

            if (newState)
            {
                particles.Play();
            }
            else
            {
                particles.Stop();
            }
        }

        private RobotThrusterFlag GetThrusterFlags()
        {
            switch (controller.CurrentPropellingForce)
            {
                case < 0: return ForwardThrust;
                case > 0: return BackwardThrust;
            }

            switch (controller.YawVelocity)
            {
                case < 0: return LeftRotationThrust;
                case > 0: return RightRotationThrust;
            }

            return RobotThrusterFlag.None;
        }
    }
}
