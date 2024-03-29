using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Unity;
using BEPUutilities.FixedMath;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RecoDeli.Scripts.Gameplay.Robot
{
    public class RobotThrusterController : MonoBehaviour
    {
        [SerializeField] private RobotController controller;
        [SerializeField] private ParticleSystem frontLeftThruster;
        [SerializeField] private ParticleSystem frontRightThruster;
        [SerializeField] private ParticleSystem backLeftThruster;
        [SerializeField] private ParticleSystem backRightThruster;

        [SerializeField] private AudioSource thrusterAudio;

        [SerializeField] private float linearThreshold;
        [SerializeField] private float angularThreshold;
        [SerializeField] private Vector3 thrusterAbsoluteOffset;

        [System.Serializable]
        [Flags]
        public enum ThrusterFlag
        {
            None = 0,
            FrontLeft = 0x1,
            FrontRight = 0x2,
            BackLeft = 0x4,
            BackRight = 0x8,
            Front = FrontLeft | FrontRight,
            Back = BackLeft | BackRight,
            All = Front | Back,
        }

        private ThrusterFlag overrideThrustersState;

        public ThrusterFlag GetState()
        {
            fint forwardMovement = BEPUutilities.Vector3.Dot(controller.LinearAcceleration, controller.Rigidbody.Entity.OrientationMatrix.Forward);
            fint yawRotation = controller.AngularAcceleration.Y;

            bool noMovement = fint.Abs(forwardMovement) < (fint)linearThreshold;

            bool frontLeftState = forwardMovement < -(fint)linearThreshold || (noMovement && yawRotation < -(fint)angularThreshold);
            bool frontRightState = forwardMovement < -(fint)linearThreshold || (noMovement && yawRotation > (fint)angularThreshold);
            bool backLeftState = forwardMovement > (fint)linearThreshold || (noMovement && yawRotation > (fint)angularThreshold);
            bool backRightState = forwardMovement > (fint)linearThreshold || (noMovement && yawRotation < -(fint)angularThreshold);

            ThrusterFlag state =
                (frontLeftState ? ThrusterFlag.FrontLeft : ThrusterFlag.None) |
                (frontRightState ? ThrusterFlag.FrontRight : ThrusterFlag.None) |
                (backLeftState ? ThrusterFlag.BackLeft : ThrusterFlag.None) |
                (backRightState ? ThrusterFlag.BackRight : ThrusterFlag.None);

            return state | overrideThrustersState;
        }

        public void UpdateThrustersVisuals()
        {
            var state = GetState();

            SwitchParticleSystem(frontLeftThruster, (state & ThrusterFlag.FrontLeft) > 0);
            SwitchParticleSystem(frontRightThruster, (state & ThrusterFlag.FrontRight) > 0);
            SwitchParticleSystem(backLeftThruster, (state & ThrusterFlag.BackLeft) > 0);
            SwitchParticleSystem(backRightThruster, (state & ThrusterFlag.BackRight) > 0);

            var any = state != ThrusterFlag.None;

            if (any && !thrusterAudio.isPlaying)
            {
                thrusterAudio.Play();
            }
            else if (!any && thrusterAudio.isPlaying)
            {
                thrusterAudio.Stop();
            }
        }

        public void UpdateThrustersPhysics()
        {
            var state = GetState();

            if ((state & ThrusterFlag.FrontLeft) > 0) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3((fint)(-1), (fint)0, (fint)1));
            if ((state & ThrusterFlag.FrontRight) > 0) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3((fint)1, (fint)0, (fint)1));
            if ((state & ThrusterFlag.BackLeft) > 0) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3((fint)(-1), (fint)0, (fint)(-1)));
            if ((state & ThrusterFlag.BackRight) > 0) HandleThrusterRepulsion(controller, new BEPUutilities.Vector3((fint)1, (fint)0, (fint)(-1)));
        }

        private void SwitchParticleSystem(ParticleSystem particles, bool newState)
        {
            var oldState = particles.isEmitting;

            if (newState == oldState) return;

            if (newState) particles.Play();
            else particles.Stop();
        }

        private void HandleThrusterRepulsion(RobotController controller, BEPUutilities.Vector3 normalizedOffset)
        {
            var maxPushForce = (fint)controller.ThrusterRepulsionForce;
            var maxDistance = (fint)controller.ThrusterRepulsionMaxDistance;
            if (maxPushForce <= (fint)0 || maxDistance <= (fint)0) return;

            var offset = normalizedOffset * thrusterAbsoluteOffset.ToBEPU();

            var robotEntity = controller.Rigidbody.Entity;

            var worldPosition = BEPUutilities.Quaternion.Transform(offset, robotEntity.Orientation) + robotEntity.Position;
            var worldDirection = BEPUutilities.Quaternion.Transform(new BEPUutilities.Vector3((fint)0, (fint)0, normalizedOffset.Z), robotEntity.Orientation);

            var raycastRay = new BEPUutilities.Ray(worldPosition, worldDirection);
            var hits = new List<BEPUphysics.RayCastResult>();
            robotEntity.Space.RayCast(raycastRay, maxDistance, hits);

            foreach(var hit in hits)
            {
                var entityCollision = (hit.HitObject as EntityCollidable);
                if (entityCollision == null || entityCollision.Entity == null || entityCollision.Entity == robotEntity) continue;

                var distance = (hit.HitData.Location - worldPosition).Length();

                var pushVelocity = worldDirection * (((fint)1 - distance / maxDistance) * maxPushForce);

                entityCollision.Entity.ApplyImpulse(hit.HitData.Location, pushVelocity);
            }
        }

        public void ForceStartThruster(ThrusterFlag flag)
        {
            overrideThrustersState |= flag;
        }

        public void ResetForcedThruster(ThrusterFlag flag)
        {
            overrideThrustersState &= ~flag;
        }
    }
}
