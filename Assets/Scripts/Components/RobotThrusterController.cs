using LudumDare.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace LudumDare.Scripts.Components
{
    public class RobotThrusterController : MonoBehaviour
    {

        [SerializeField] private ParticleSystem frontLeftThruster;
        [SerializeField] private ParticleSystem frontRightThruster;
        [SerializeField] private ParticleSystem backLeftThruster;
        [SerializeField] private ParticleSystem backRightThruster;

        [SerializeField] private RobotController controller;
        [SerializeField] private AudioSource thrusterAudio;

        private void FixedUpdate()
        {
            SwitchParticleSystem(frontLeftThruster, RobotThrusterFlag.FrontLeft);
            SwitchParticleSystem(frontRightThruster, RobotThrusterFlag.FrontRight);
            SwitchParticleSystem(backLeftThruster, RobotThrusterFlag.BackLeft);
            SwitchParticleSystem(backRightThruster, RobotThrusterFlag.BackRight);

            if(GetThrusterFlags() > 0 && !thrusterAudio.isPlaying)
            {
                thrusterAudio.Play();
            }
            else if(GetThrusterFlags() == 0 && thrusterAudio.isPlaying)
            {
                thrusterAudio.Stop();
            }
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
            if (controller.ExecutingCommands)
            {
                return controller.CurrentCommand.Thrusters;
            }

            return RobotThrusterFlag.None;
        }
    }
}
