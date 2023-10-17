using System;
using BEPUutilities.FixedMath;
using BEPUphysics.Entities;
using BEPUutilities;
 

namespace BEPUphysics.Constraints.SingleEntity
{
    /// <summary>
    /// Prevents the target entity from moving faster than the specified speeds.
    /// </summary>
    public class MaximumAngularSpeedConstraint : SingleEntityConstraint, I3DImpulseConstraint
    {
        private Matrix3x3 effectiveMassMatrix;
        private fint maxForceDt = fint.MaxValue;
        private fint maxForceDtSquared = fint.MaxValue;
        private Vector3 accumulatedImpulse;
        private fint maximumForce = fint.MaxValue;
        private fint maximumSpeed;
        private fint maximumSpeedSquared;

        private fint softness = (fint).00001f;
        private fint usedSoftness;

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// Set its Entity and MaximumSpeed to complete the configuration.
        /// IsActive also starts as false with this constructor.
        /// </summary>
        public MaximumAngularSpeedConstraint()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a maximum speed constraint.
        /// </summary>
        /// <param name="e">Affected entity.</param>
        /// <param name="maxSpeed">Maximum angular speed allowed.</param>
        public MaximumAngularSpeedConstraint(Entity e, fint maxSpeed)
        {
            Entity = e;
            MaximumSpeed = maxSpeed;
        }

        /// <summary>
        /// Gets and sets the maximum impulse that the constraint will attempt to apply when satisfying its requirements.
        /// This field can be used to simulate friction in a constraint.
        /// </summary>
        public fint MaximumForce
        {
            get
            {
                if (maximumForce > (fint)0)
                {
                    return maximumForce;
                }
                return (fint)0;
            }
            set { maximumForce = value >= (fint)0 ? value : (fint)0; }
        }

        /// <summary>
        /// Gets or sets the maximum angular speed that this constraint allows.
        /// </summary>
        public fint MaximumSpeed
        {
            get { return maximumSpeed; }
            set
            {
                maximumSpeed = MathHelper.Max((fint)0, value);
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
        public fint Softness
        {
            get { return softness; }
            set { softness = fint.Max((fint)0, value); }
        }

        #region I3DImpulseConstraint Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        Vector3 I3DImpulseConstraint.RelativeVelocity
        {
            get { return entity.angularVelocity; }
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
        public override fint SolveIteration()
        {
            fint angularSpeed = entity.angularVelocity.LengthSquared();
            if (angularSpeed > maximumSpeedSquared)
            {
                angularSpeed = fint.Sqrt(angularSpeed);
                Vector3 impulse;
                //divide by angularSpeed to normalize the velocity.
                //Multiply by angularSpeed - maximumSpeed to get the 'velocity change vector.'
                Vector3.Multiply(ref entity.angularVelocity, -(angularSpeed - maximumSpeed) / angularSpeed, out impulse);

                //incorporate softness
                Vector3 softnessImpulse;
                Vector3.Multiply(ref accumulatedImpulse, usedSoftness, out softnessImpulse);
                Vector3.Subtract(ref impulse, ref softnessImpulse, out impulse);

                //Transform into impulse
                Matrix3x3.Transform(ref impulse, ref effectiveMassMatrix, out impulse);


                //Accumulate
                Vector3 previousAccumulatedImpulse = accumulatedImpulse;
                Vector3.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);
                fint forceMagnitude = accumulatedImpulse.LengthSquared();
                if (forceMagnitude > maxForceDtSquared)
                {
                    //max / impulse gives some value 0 < x < 1.  Basically, normalize the vector (divide by the length) and scale by the maximum.
                    fint multiplier = maxForceDt / fint.Sqrt(forceMagnitude);
                    accumulatedImpulse.X *= multiplier;
                    accumulatedImpulse.Y *= multiplier;
                    accumulatedImpulse.Z *= multiplier;

                    //Since the limit was exceeded by this corrective impulse, limit it so that the accumulated impulse remains constrained.
                    impulse.X = accumulatedImpulse.X - previousAccumulatedImpulse.X;
                    impulse.Y = accumulatedImpulse.Y - previousAccumulatedImpulse.Y;
                    impulse.Z = accumulatedImpulse.Z - previousAccumulatedImpulse.Z;
                }

                entity.ApplyAngularImpulse(ref impulse);


                return (fint.Abs(impulse.X) + fint.Abs(impulse.Y) + fint.Abs(impulse.Z));
            }

            return (fint)0;
        }

        /// <summary>
        /// Calculates necessary information for velocity solving.
        /// Called automatically by space.
        /// </summary>
        /// <param name="dt">Time in seconds since the last update.</param>
        public override void Update(fint dt)
        {
            usedSoftness = softness / dt;

            effectiveMassMatrix = entity.inertiaTensorInverse;

            effectiveMassMatrix.M11 += usedSoftness;
            effectiveMassMatrix.M22 += usedSoftness;
            effectiveMassMatrix.M33 += usedSoftness;

            Matrix3x3.Invert(ref effectiveMassMatrix, out effectiveMassMatrix);

            //Determine maximum force
            if (maximumForce < fint.MaxValue)
            {
                maxForceDt = maximumForce * dt;
                maxForceDtSquared = maxForceDt * maxForceDt;
            }
            else
            {
                maxForceDt = fint.MaxValue;
                maxForceDtSquared = fint.MaxValue;
            }

        }


        public override void ExclusiveUpdate()
        {

            //Can't do warmstarting due to the strangeness of this constraint (not based on a position error, nor is it really a motor).
            accumulatedImpulse = Toolbox.ZeroVector;
        }
    }
}