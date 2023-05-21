using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Unity;
using SoftFloat;
using System.Collections.Generic;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class RobotThrusterController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem frontLeftThruster;
        [SerializeField] private ParticleSystem frontRightThruster;
        [SerializeField] private ParticleSystem backLeftThruster;
        [SerializeField] private ParticleSystem backRightThruster;

        [SerializeField] private AudioSource thrusterAudio;

        [SerializeField] private float linearThreshold;
        [SerializeField] private float angularThreshold;
        [SerializeField] private Vector3 thrusterAbsoluteOffset;

        public void UpdateThrusters(RobotController controller)
        {
            sfloat forwardMovement = BEPUutilities.Vector3.Dot(controller.LinearAcceleration, controller.Rigidbody.Entity.OrientationMatrix.Forward);
            sfloat yawRotation = controller.AngularAcceleration.Y;

            bool noMovement = sfloat.Abs(forwardMovement) < (sfloat)linearThreshold;

            bool frontLeftState = forwardMovement < -(sfloat)linearThreshold || (noMovement && yawRotation < -(sfloat)angularThreshold);
            bool frontRightState = forwardMovement < -(sfloat)linearThreshold || (noMovement && yawRotation > (sfloat)angularThreshold);
            bool backLeftState = forwardMovement > (sfloat)linearThreshold || (noMovement && yawRotation > (sfloat)angularThreshold);
            bool backRightState = forwardMovement > (sfloat)linearThreshold || (noMovement && yawRotation < -(sfloat)angularThreshold);

            bool any = frontLeftState || frontRightState || backLeftState || backRightState;

            SwitchParticleSystem(frontLeftThruster, frontLeftState);
            SwitchParticleSystem(frontRightThruster, frontRightState);
            SwitchParticleSystem(backLeftThruster, backLeftState);
            SwitchParticleSystem(backRightThruster, backRightState);

            if(any && !thrusterAudio.isPlaying)
            {
                thrusterAudio.Play();
            }
            else if(!any && thrusterAudio.isPlaying)
            {
                thrusterAudio.Stop();
            }

            if (frontLeftState) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3(sfloat.MinusOne, sfloat.Zero, sfloat.One));
            if (frontRightState) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3(sfloat.One, sfloat.Zero, sfloat.One));
            if (backLeftState) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3(sfloat.MinusOne, sfloat.Zero, sfloat.MinusOne));
            if (backRightState) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3(sfloat.One, sfloat.Zero, sfloat.MinusOne));
        }

        private void SwitchParticleSystem(ParticleSystem particles, bool newState)
        {
            var oldState = particles.isPlaying;

            if (newState == oldState)  return;

            if (newState) particles.Play();
            else particles.Stop();
        }

        private void HandleThrusterRepulsion(RobotController controller, BEPUutilities.Vector3 normalizedOffset)
        {
            var maxPushForce = (sfloat)controller.ThrusterRepulsionForce;
            var maxDistance = (sfloat)controller.ThrusterRepulsionMaxDistance;
            if (maxPushForce <= sfloat.Zero || maxDistance <= sfloat.Zero) return;

            var offset = normalizedOffset * thrusterAbsoluteOffset.ToBEPU();

            var robotEntity = controller.Rigidbody.Entity;

            var worldPosition = BEPUutilities.Quaternion.Transform(offset, robotEntity.Orientation) + robotEntity.Position;
            var worldDirection = BEPUutilities.Quaternion.Transform(new BEPUutilities.Vector3(sfloat.Zero, sfloat.Zero, normalizedOffset.Z), robotEntity.Orientation);

            var raycastRay = new BEPUutilities.Ray(worldPosition, worldDirection);
            var hits = new List<BEPUphysics.RayCastResult>();
            robotEntity.Space.RayCast(raycastRay, maxDistance, hits);

            foreach(var hit in hits)
            {
                var entityCollision = (hit.HitObject as EntityCollidable);
                if (entityCollision == null || entityCollision.Entity == null || entityCollision.Entity == robotEntity) continue;

                var distance = (hit.HitData.Location - worldPosition).Length();

                var pushVelocity = worldDirection * ((sfloat.One - distance / maxDistance) * maxPushForce);

                entityCollision.Entity.ApplyImpulse(hit.HitData.Location, pushVelocity);
            }
        }
    }
}
