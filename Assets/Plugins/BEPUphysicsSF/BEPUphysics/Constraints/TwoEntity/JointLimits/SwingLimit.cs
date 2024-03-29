﻿using System;
using BEPUutilities.FixedMath;
using BEPUphysics.Entities;

using BEPUutilities;

namespace BEPUphysics.Constraints.TwoEntity.JointLimits
{
    /// <summary>
    /// Keeps the angle between the axes attached to two entities below some maximum value.
    /// </summary>
    public class SwingLimit : JointLimit, I1DImpulseConstraintWithError, I1DJacobianConstraint
    {
        private fint accumulatedImpulse;
        private fint biasVelocity;
        private Vector3 hingeAxis;
        private fint minimumCosine = 1;
        private fint error;

        private Vector3 localAxisA;

        private Vector3 localAxisB;
        private Vector3 worldAxisA;

        private Vector3 worldAxisB;
        private fint velocityToImpulse;

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the maximum relative angle of two entities to some value.
        /// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
        /// as well as the WorldAxisA, WorldAxisB (or their entity-local versions) and the MaximumAngle.
        /// This constructor sets the constraint's IsActive property to false by default.
        /// </summary>
        public SwingLimit()
        {
            IsActive = false;
        }

        /// <summary>
        /// Constructs a new constraint which attempts to restrict the maximum relative angle of two entities to some value.
        /// </summary>
        /// <param name="connectionA">First connection of the pair.</param>
        /// <param name="connectionB">Second connection of the pair.</param>
        /// <param name="axisA">Axis attached to the first connected entity.</param>
        /// <param name="axisB">Axis attached to the second connected entity.</param>
        /// <param name="maximumAngle">Maximum angle between the axes allowed.</param>
        public SwingLimit(Entity connectionA, Entity connectionB, Vector3 axisA, Vector3 axisB, fint maximumAngle)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;
            WorldAxisA = axisA;
            WorldAxisB = axisB;
            MaximumAngle = maximumAngle;
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in its local space.
        /// </summary>
        public Vector3 LocalAxisA
        {
            get { return localAxisA; }
            set
            {
                localAxisA = Vector3.Normalize(value);
                Matrix3x3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
            }
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in its local space.
        /// </summary>
        public Vector3 LocalAxisB
        {
            get { return localAxisB; }
            set
            {
                localAxisB = Vector3.Normalize(value);
                Matrix3x3.Transform(ref localAxisB, ref connectionA.orientationMatrix, out worldAxisB);
            }
        }

        /// <summary>
        /// Maximum angle allowed between the two axes, from 0 to pi.
        /// </summary>
        public fint MaximumAngle
        {
            get { return fint.Acos(minimumCosine); }
            set { minimumCosine = fint.Cos(MathHelper.Clamp(value, 0, MathHelper.Pi)); }
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in world space.
        /// </summary>
        public Vector3 WorldAxisA
        {
            get { return worldAxisA; }
            set
            {
                worldAxisA = Vector3.Normalize(value);
                Quaternion conjugate;
                Quaternion.Conjugate(ref connectionA.orientation, out conjugate);
                Quaternion.Transform(ref worldAxisA, ref conjugate, out localAxisA);
            }
        }

        /// <summary>
        /// Gets or sets the axis attached to the first connected entity in world space.
        /// </summary>
        public Vector3 WorldAxisB
        {
            get { return worldAxisB; }
            set
            {
                worldAxisB = Vector3.Normalize(value);
                Quaternion conjugate;
                Quaternion.Conjugate(ref connectionB.orientation, out conjugate);
                Quaternion.Transform(ref worldAxisB, ref conjugate, out localAxisB);
            }
        }

        #region I1DImpulseConstraintWithError Members

        /// <summary>
        /// Gets the current relative velocity between the connected entities with respect to the constraint.
        /// </summary>
        public fint RelativeVelocity
        {
            get
            {
                if (isLimitActive)
                {
                    Vector3 relativeVelocity;
                    Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out relativeVelocity);
                    fint lambda;
                    Vector3.Dot(ref relativeVelocity, ref hingeAxis, out lambda);
                    return lambda;
                }
                return 0;
            }
        }

        /// <summary>
        /// Gets the total impulse applied by this constraint.
        /// </summary>
        public fint TotalImpulse
        {
            get { return accumulatedImpulse; }
        }

        /// <summary>
        /// Gets the current constraint error.
        /// </summary>
        public fint Error
        {
            get { return error; }
        }

        #endregion

        #region I1DJacobianConstraint Members

        /// <summary>
        /// Gets the linear jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobian">Linear jacobian entry for the first connected entity.</param>
        public void GetLinearJacobianA(out Vector3 jacobian)
        {
            jacobian = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the linear jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Linear jacobian entry for the second connected entity.</param>
        public void GetLinearJacobianB(out Vector3 jacobian)
        {
            jacobian = Toolbox.ZeroVector;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the first connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the first connected entity.</param>
        public void GetAngularJacobianA(out Vector3 jacobian)
        {
            jacobian = hingeAxis;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
        public void GetAngularJacobianB(out Vector3 jacobian)
        {
            jacobian = -hingeAxis;
        }

        /// <summary>
        /// Gets the mass matrix of the constraint.
        /// </summary>
        /// <param name="outputMassMatrix">Constraint's mass matrix.</param>
        public void GetMassMatrix(out fint outputMassMatrix)
        {
            outputMassMatrix = velocityToImpulse;
        }

        #endregion

        /// <summary>
        /// Applies the sequential impulse.
        /// </summary>
        public override fint SolveIteration()
        {
            fint lambda;
            Vector3 relativeVelocity;
            Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out relativeVelocity);
            //Transform the velocity to with the jacobian
            Vector3.Dot(ref relativeVelocity, ref hingeAxis, out lambda);
            //Add in the constraint space bias velocity
            lambda = -lambda + biasVelocity - softness * accumulatedImpulse;

            //Transform to an impulse
            lambda *= velocityToImpulse;

            //Clamp accumulated impulse (can't go negative)
            fint previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Max(accumulatedImpulse + lambda, 0);
            lambda = accumulatedImpulse - previousAccumulatedImpulse;

            //Apply the impulse
            Vector3 impulse;
            Vector3.Multiply(ref hingeAxis, lambda, out impulse);
            if (connectionA.isDynamic)
            {
                connectionA.ApplyAngularImpulse(ref impulse);
            }
            if (connectionB.isDynamic)
            {
                Vector3.Negate(ref impulse, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }

            return (fint.Abs(lambda));
        }

        /// <summary>
        /// Initializes the constraint for this frame.
        /// </summary>
        /// <param name="dt">Time since the last frame.</param>
        public override void Update(fint dt)
        {
            Matrix3x3.Transform(ref localAxisA, ref connectionA.orientationMatrix, out worldAxisA);
            Matrix3x3.Transform(ref localAxisB, ref connectionB.orientationMatrix, out worldAxisB);

            fint dot;
            Vector3.Dot(ref worldAxisA, ref worldAxisB, out dot);

            //Keep in mind, the dot is the cosine of the angle.
            //1: 0 radians
            //0: pi/2 radians
            //-1: pi radians
            if (dot > minimumCosine)
            {
                isActiveInSolver = false;
                error = 0;
                accumulatedImpulse = 0;
                isLimitActive = false;
                return;
            }
            isLimitActive = true;

            //Hinge axis is actually the jacobian entry for angular A (negative angular B).
            Vector3.Cross(ref worldAxisA, ref worldAxisB, out hingeAxis);
            fint lengthSquared = hingeAxis.LengthSquared();
            if (lengthSquared < Toolbox.Epsilon)
            {
                //They're parallel; for the sake of continuity, pick some axis which is perpendicular to both that ISN'T the zero vector.
                Vector3.Cross(ref worldAxisA, ref Toolbox.UpVector, out hingeAxis);
                lengthSquared = hingeAxis.LengthSquared();
                if (lengthSquared < Toolbox.Epsilon)
                {
                    //That's improbable; b's world axis was apparently parallel with the up vector!
                    //So just use the right vector (it can't be parallel with both the up and right vectors).
                    Vector3.Cross(ref worldAxisA, ref Toolbox.RightVector, out hingeAxis);
                }
            }


            fint errorReduction;
            springSettings.ComputeErrorReductionAndSoftness(dt, 1 / dt, out errorReduction, out softness);

            //Further away from 0 degrees is further negative; if the dot is below the minimum cosine, it means the angle is above the maximum angle.
            error = fint.Max(0, minimumCosine - dot - margin);
            biasVelocity = MathHelper.Clamp(errorReduction * error, -maxCorrectiveVelocity, maxCorrectiveVelocity);

            if (bounciness > 0)
            {
                //Compute the speed around the axis.
                fint relativeSpeed;
                Vector3 relativeVelocity;
                Vector3.Subtract(ref connectionA.angularVelocity, ref connectionB.angularVelocity, out relativeVelocity);
                Vector3.Dot(ref relativeVelocity, ref hingeAxis, out relativeSpeed);

                biasVelocity = MathHelper.Max(biasVelocity, ComputeBounceVelocity(-relativeSpeed));
            }

            //Connection A's contribution to the mass matrix
            fint entryA;
            Vector3 transformedAxis;
            if (connectionA.isDynamic)
            {
                Matrix3x3.Transform(ref hingeAxis, ref connectionA.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref hingeAxis, out entryA);
            }
            else
                entryA = 0;

            //Connection B's contribution to the mass matrix
            fint entryB;
            if (connectionB.isDynamic)
            {
                Matrix3x3.Transform(ref hingeAxis, ref connectionB.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref hingeAxis, out entryB);
            }
            else
                entryB = 0;

            //Compute the inverse mass matrix
            velocityToImpulse = 1 / (softness + entryA + entryB);


        }

        public override void ExclusiveUpdate()
        {
            //Apply accumulated impulse
            Vector3 impulse;
            Vector3.Multiply(ref hingeAxis, accumulatedImpulse, out impulse);
            if (connectionA.isDynamic)
            {
                connectionA.ApplyAngularImpulse(ref impulse);
            }
            if (connectionB.isDynamic)
            {
                Vector3.Negate(ref impulse, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }
        }
    }
}