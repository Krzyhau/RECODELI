﻿using System;
using BEPUutilities.FixedMath;
using BEPUphysics.Entities;
 
using BEPUutilities;

namespace BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Tries to rotate two entities so that they reach a specified relative orientation or speed around an axis.
    /// </summary>
    public class RevoluteMotor : Motor, I1DImpulseConstraintWithError, I1DJacobianConstraint
    {
        private readonly JointBasis2D basis = new JointBasis2D();
        private readonly MotorSettings1D settings;
        private fint accumulatedImpulse;
        protected fint biasVelocity;
        private Vector3 jacobianA, jacobianB;
        private fint error;

        private Vector3 localTestAxis;


        private Vector3 worldTestAxis;
        private fint velocityToImpulse;

        /// <summary>
        /// Constructs a new constraint tries to rotate two entities so that they reach a specified relative orientation around an axis.
        /// To finish the initialization, specify the connections (ConnectionA and ConnectionB) 
        /// as well as the Basis and TestAxis.
        /// This constructor sets the constraint's IsActive property to false by default.
        /// </summary>
        public RevoluteMotor()
        {
            settings = new MotorSettings1D(this);
            IsActive = false;
        }

        /// <summary>
        /// Constructs a new constraint tries to rotate two entities so that they reach a specified relative orientation around an axis.
        /// </summary>
        /// <param name="connectionA">First connection of the pair.</param>
        /// <param name="connectionB">Second connection of the pair.</param>
        /// <param name="motorizedAxis">Rotation axis to control in world space.</param>
        public RevoluteMotor(Entity connectionA, Entity connectionB, Vector3 motorizedAxis)
        {
            ConnectionA = connectionA;
            ConnectionB = connectionB;
            SetupJointTransforms(motorizedAxis);

            settings = new MotorSettings1D(this);
        }

        /// <summary>
        /// Gets the basis attached to entity A.
        /// The primary axis represents the motorized axis of rotation.  The 'measurement plane' which the test axis is tested against is based on this primary axis.
        /// The x axis defines the 'base' direction on the measurement plane corresponding to 0 degrees of relative rotation.
        /// </summary>
        public JointBasis2D Basis
        {
            get { return basis; }
        }

        /// <summary>
        /// Gets or sets the axis attached to entity B in its local space.
        /// This axis is projected onto the x and y axes of transformA to determine the hinge angle.
        /// </summary>
        public Vector3 LocalTestAxis
        {
            get { return localTestAxis; }
            set
            {
                localTestAxis = Vector3.Normalize(value);
                Matrix3x3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);
            }
        }

        /// <summary>
        /// Gets the motor's velocity and servo settings.
        /// </summary>
        public MotorSettings1D Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Gets or sets the axis attached to entity B in world space.
        /// This axis is projected onto the x and y axes of the Basis attached to entity A to determine the hinge angle.
        /// </summary>
        public Vector3 TestAxis
        {
            get { return worldTestAxis; }
            set
            {
                worldTestAxis = Vector3.Normalize(value);
                Matrix3x3.TransformTranspose(ref worldTestAxis, ref connectionB.orientationMatrix, out localTestAxis);
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
                fint velocityA, velocityB;
                Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out velocityA);
                Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out velocityB);
                return velocityA + velocityB;
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
        /// If the motor is in velocity only mode, the error is zero.
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
            jacobian = jacobianA;
        }

        /// <summary>
        /// Gets the angular jacobian entry for the second connected entity.
        /// </summary>
        /// <param name="jacobian">Angular jacobian entry for the second connected entity.</param>
        public void GetAngularJacobianB(out Vector3 jacobian)
        {
            jacobian = jacobianB;
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
        /// Sets up the joint transforms by automatically creating perpendicular vectors to complete the bases.
        /// </summary>
        /// <param name="motorizedAxis">Axis around which the motor acts.</param>
        public void SetupJointTransforms(Vector3 motorizedAxis)
        {
            //Compute a vector which is perpendicular to the axis.  It'll be added in local space to both connections.
            Vector3 xAxis;
            Vector3.Cross(ref motorizedAxis, ref Toolbox.UpVector, out xAxis);
            fint length = xAxis.LengthSquared();
            if (length < Toolbox.Epsilon)
            {
                Vector3.Cross(ref motorizedAxis, ref Toolbox.RightVector, out xAxis);
            }

            //Put the axes into the joint transform of A.
            basis.rotationMatrix = connectionA.orientationMatrix;
            basis.SetWorldAxes(motorizedAxis, xAxis);


            //Put the axes into the 'joint transform' of B too.
            TestAxis = basis.xAxis;
        }

        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(fint dt)
        {
            //Transform the axes into world space.
            basis.rotationMatrix = connectionA.orientationMatrix;
            basis.ComputeWorldSpaceAxes();
            Matrix3x3.Transform(ref localTestAxis, ref connectionB.orientationMatrix, out worldTestAxis);

            fint updateRate = (fint)1 / dt;
            if (settings.mode == MotorMode.Servomechanism)
            {
                fint y, x;
                Vector3 yAxis;
                Vector3.Cross(ref basis.primaryAxis, ref basis.xAxis, out yAxis);
                Vector3.Dot(ref worldTestAxis, ref yAxis, out y);
                Vector3.Dot(ref worldTestAxis, ref basis.xAxis, out x);
                var angle = fint.Atan2(y, x);

                //****** VELOCITY BIAS ******//
                //Compute the correction velocity.
                error = GetDistanceFromGoal(angle);


                fint absErrorOverDt = fint.Abs(error * updateRate);
                fint errorReduction;
                settings.servo.springSettings.ComputeErrorReductionAndSoftness(dt, updateRate, out errorReduction, out usedSoftness);
                biasVelocity = fint.Sign(error) * MathHelper.Min(settings.servo.baseCorrectiveSpeed, absErrorOverDt) + error * errorReduction;

                biasVelocity = MathHelper.Clamp(biasVelocity, -settings.servo.maxCorrectiveVelocity, settings.servo.maxCorrectiveVelocity);
            }
            else
            {
                biasVelocity = settings.velocityMotor.goalVelocity;
                usedSoftness = settings.velocityMotor.softness * updateRate;
                error = (fint)0;
            }


            //Compute the jacobians
            jacobianA = basis.primaryAxis;
            jacobianB.X = -jacobianA.X;
            jacobianB.Y = -jacobianA.Y;
            jacobianB.Z = -jacobianA.Z;


            //****** EFFECTIVE MASS MATRIX ******//
            //Connection A's contribution to the mass matrix
            fint entryA;
            Vector3 transformedAxis;
            if (connectionA.isDynamic)
            {
                Matrix3x3.Transform(ref jacobianA, ref connectionA.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref jacobianA, out entryA);
            }
            else
                entryA = (fint)0;

            //Connection B's contribution to the mass matrix
            fint entryB;
            if (connectionB.isDynamic)
            {
                Matrix3x3.Transform(ref jacobianB, ref connectionB.inertiaTensorInverse, out transformedAxis);
                Vector3.Dot(ref transformedAxis, ref jacobianB, out entryB);
            }
            else
                entryB = (fint)0;

            //Compute the inverse mass matrix
            velocityToImpulse = (fint)1 / (usedSoftness + entryA + entryB);


            //Update the maximum force
            ComputeMaxForces(settings.maximumForce, dt);



        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //****** WARM STARTING ******//
            //Apply accumulated impulse
            Vector3 impulse;
            if (connectionA.isDynamic)
            {
                Vector3.Multiply(ref jacobianA, accumulatedImpulse, out impulse);
                connectionA.ApplyAngularImpulse(ref impulse);
            }
            if (connectionB.isDynamic)
            {
                Vector3.Multiply(ref jacobianB, accumulatedImpulse, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }
        }

        /// <summary>
        /// Computes one iteration of the constraint to meet the solver updateable's goal.
        /// </summary>
        /// <returns>The rough applied impulse magnitude.</returns>
        public override fint SolveIteration()
        {
            fint velocityA, velocityB;
            //Find the velocity contribution from each connection
            Vector3.Dot(ref connectionA.angularVelocity, ref jacobianA, out velocityA);
            Vector3.Dot(ref connectionB.angularVelocity, ref jacobianB, out velocityB);
            //Add in the constraint space bias velocity
            fint lambda = -(velocityA + velocityB) - biasVelocity - usedSoftness * accumulatedImpulse;

            //Transform to an impulse
            lambda *= velocityToImpulse;

            //Accumulate the impulse
            fint previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Clamp(accumulatedImpulse + lambda, -maxForceDt, maxForceDt);
            lambda = accumulatedImpulse - previousAccumulatedImpulse;

            //Apply the impulse
            Vector3 impulse;
            if (connectionA.isDynamic)
            {
                Vector3.Multiply(ref jacobianA, lambda, out impulse);
                connectionA.ApplyAngularImpulse(ref impulse);
            }
            if (connectionB.isDynamic)
            {
                Vector3.Multiply(ref jacobianB, lambda, out impulse);
                connectionB.ApplyAngularImpulse(ref impulse);
            }

            return fint.Abs(lambda);
        }



        private fint GetDistanceFromGoal(fint angle)
        {

            fint forwardDistance;
            fint goalAngle = MathHelper.WrapAngle(settings.servo.goal);
            if (goalAngle > (fint)0)
            {
                if (angle > goalAngle)
                    forwardDistance = angle - goalAngle;
                else if (angle > (fint)0)
                    forwardDistance = MathHelper.TwoPi - goalAngle + angle;
                else //if (angle <= 0)
                    forwardDistance = MathHelper.TwoPi - goalAngle + angle;
            }
            else
            {
                if (angle < goalAngle)
                    forwardDistance = MathHelper.TwoPi - goalAngle + angle;
                else //if (angle < 0)
                    forwardDistance = angle - goalAngle;
                //else //if (currentAngle >= 0)
                //    return angle - myMinimumAngle;
            }
            return forwardDistance > MathHelper.Pi ? MathHelper.TwoPi - forwardDistance : -forwardDistance;
        }
    }
}