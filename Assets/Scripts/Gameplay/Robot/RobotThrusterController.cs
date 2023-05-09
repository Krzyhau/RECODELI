using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
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

            if (newState == oldState)  return;

            if (newState) particles.Play();
            else particles.Stop();
        }

        private RobotThrusterFlag GetThrusterFlags()
        {
            if (controller.ExecutingInstructions)
            {
                return controller.CurrentInstruction.Action.ThrustersState;
            }
            else
            {
                return RobotThrusterFlag.None;
            }
        }
    }
}
