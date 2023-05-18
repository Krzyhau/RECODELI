using System;
using SoftFloat;
using BEPUphysics.Entities;
using BEPUutilities;
 

namespace BEPUphysics.Constraints.SingleEntity
{
    /// <summary>
    /// Prevents the target entity from moving faster than the specified speeds.
    /// </summary>
    public class MaximumLinearSpeedConstraint : SingleEntityConstraint, I3DImpulseConstraint
    {
        private sfloat effectiveMassMatrix;
        private sfloat maxForceDt = sfloat.MaxValue;
        private sfloat maxForceDtSquared = sfloat.MaxValue;
        private Vector3 accumulatedImpulse;
        private sfloat maximumForce = sfloat.MaxValue;
        private sfloat maximumSpeed;
        private sfloat maximumSpeedSquared;

        private sfloat softness = (sfloat).00001f;
        private sfloat usedSoftness;

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// Set its Entity and MaximumSpeed to complete the configuration.
        /// IsActive also starts as false with this constructor.
        /// </summary>
        public MaximumLinearSpeedConstraint()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// </summary>
        /// <param name="e">Affected entity.</param>
        /// <param name="maxSpeed">Maximum linear speed allowed.</param>
        public MaximumLinearSpeedConstraint(Entity e, sfloat maxSpeed)
        {
            Entity = e;
            MaximumSpeed = maxSpeed;
        }

        /// <summary>
        /// Gets and sets the maximum impulse that the constraint will attempt to apply when satisfying its requirements.
        /// This field can be used to simulate friction in a constraint.
        /// </summary>
        public sfloat MaximumForce
        {
            get
            {
                if (maximumForce > sfloat.Zero)
                {
                    return maximumForce;
                }
                return sfloat.Zero;
            }
            set { maximumForce = value >= sfloat.Zero ? value : sfloat.Zero; }
        }

        /// <summary>
        /// Gets or sets the maximum linear speed that this constraint allows.
        /// </summary>
        public sfloat MaximumSpeed
        {
            get { return maximumSpeed; }
            set
            {
                maximumSpeed = MathHelper.Max(sfloat.Zero, value);
                maximumSpeedSquared = maximumSpeed * maximumSpeed;
            }
        }

        /// <summary>
        /// Gets and sets the softness of this constraint.
        /// Higher values of softness allow the constraint to be violated more.
        /// Must be greater than zero.
        /// Sometimes, if a joint system is unstable, increasing the softness of the involved constraints will make it settle down.
        /// For motors, softness can be used to implement damping.  For a damping constant k, the appropriate softness is 1/k.
        /// </summary>
        public sfloat Softness
        {
            get { return softness; }
            set { softness = sfloat.Max(sfloat.Zero, value); }
        }

        #region I3DImpulseConstraint Members

        /// <summary>
        /// Gets the current relative velocity with respect to the constraint.
        /// For a single entity constraint, this is pretty straightforward as the
        /// velocity of the entity.
        /// </summary>
        Vector3 I3DImpulseConstraint.RelativeVelocity
        {
            get { return Entity.LinearVelocity; }
        }

        /// <summary>
        /// Gets the total impulse applied by the constraint.
        /// </summary>
        public Vector3 TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        #endregion

        /// <summary>
        /// Calculates and applies corrective impulses.
        /// Called automatically by space.
        /// </summary>
        public override sfloat SolveIteration()
        {
            sfloat linearSpeed = entity.linearVelocity.LengthSquared();
            if (linearSpeed > maximumSpeedSquared)
            {
                linearSpeed = libm.sqrtf(linearSpeed);
                Vector3 impulse;
                //divide by linearSpeed to normalize the velocity.
                //Multiply by linearSpeed - maximumSpeed to get the 'velocity change vector.'
                Vector3.Multiply(ref entity.linearVelocity, -(linearSpeed - maximumSpeed) / linearSpeed, out impulse);

                //incorporate softness
                Vector3 softnessImpulse;
                Vector3.Multiply(ref accumulatedImpulse, usedSoftness, out softnessImpulse);
                Vector3.Subtract(ref impulse, ref softnessImpulse, out impulse);

                //Transform into impulse
                Vector3.Multiply(ref impulse, effectiveMassMatrix, out impulse);


                //Accumulate
                Vector3 previousAccumulatedImpulse = accumulatedImpulse;
                Vector3.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);
                sfloat forceMagnitude = accumulatedImpulse.LengthSquared();
                if (forceMagnitude > maxForceDtSquared)
                {
                    //max / impulse gives some value 0 < x < 1.  Basically, normalize the vector (divide by the length) and scale by the maximum.
                    sfloat multiplier = maxForceDt / libm.sqrtf(forceMagnitude);
                    accumulatedImpulse.X *= multiplier;
                    accumulatedImpulse.Y *= multiplier;
                    accumulatedImpulse.Z *= multiplier;

                    //Since the limit was exceeded by this corrective impulse, limit it so that the accumulated impulse remains constrained.
                    impulse.X = accumulatedImpulse.X - previousAccumulatedImpulse.X;
                    impulse.Y = accumulatedImpulse.Y - previousAccumulatedImpulse.Y;
                    impulse.Z = accumulatedImpulse.Z - previousAccumulatedImpulse.Z;
                }

                entity.ApplyLinearImpulse(ref impulse);


                return (sfloat.Abs(impulse.X) + sfloat.Abs(impulse.Y) + sfloat.Abs(impulse.Z));
            }


            return sfloat.Zero;
        }

        /// <summary>
        /// Calculates necessary information for velocity solving.
        /// Called automatically by space.
        /// </summary>
        /// <param name="dt">Time in seconds since the last update.</param>
        public override void Update(sfloat dt)
        {
            usedSoftness = softness / dt;

            effectiveMassMatrix = sfloat.One / (entity.inverseMass + usedSoftness);

            //Determine maximum force
            if (maximumForce < sfloat.MaxValue)
            {
                maxForceDt = maximumForce * dt;
                maxForceDtSquared = maxForceDt * maxForceDt;
            }
            else
            {
                maxForceDt = sfloat.MaxValue;
                maxForceDtSquared = sfloat.MaxValue;
            }

          
        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Can't do warmstarting due to the strangeness of this constraint (not based on a position error, nor is it really a motor).
            accumulatedImpulse = Toolbox.ZeroVector;
        }
    }
}