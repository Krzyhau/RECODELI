﻿using BEPUphysics.Entities;
using BEPUphysics.CollisionTests;
using BEPUphysics.Settings;
using BEPUutilities;
using System;
using BEPUutilities.FixedMath;
using BEPUutilities.DataStructures;

namespace BEPUphysics.Constraints.Collision
{
    /// <summary>
    /// Computes the forces necessary to keep two entities from going through each other at a contact point.
    /// </summary>
    public class ContactPenetrationConstraint : SolverUpdateable
    {
        internal Contact contact;

        ///<summary>
        /// Gets the contact associated with this penetration constraint.
        ///</summary>
        public Contact Contact { get { return contact; } }
        internal fint accumulatedImpulse;
        //sfloat linearBX, linearBY, linearBZ;
        internal fint angularAX, angularAY, angularAZ;
        internal fint angularBX, angularBY, angularBZ;

        private fint softness;
        private fint bias;
        private fint linearAX, linearAY, linearAZ;
        private Entity entityA, entityB;
        private bool entityADynamic, entityBDynamic;
        //Inverse effective mass matrix
        internal fint velocityToImpulse;
        private ContactManifoldConstraint contactManifoldConstraint;

        internal Vector3 ra, rb;

        ///<summary>
        /// Constructs a new penetration constraint.
        ///</summary>
        public ContactPenetrationConstraint()
        {
            isActive = false;
        }


        ///<summary>
        /// Configures the penetration constraint.
        ///</summary>
        ///<param name="contactManifoldConstraint">Owning manifold constraint.</param>
        ///<param name="contact">Contact associated with the penetration constraint.</param>
        public void Setup(ContactManifoldConstraint contactManifoldConstraint, Contact contact)
        {
            this.contactManifoldConstraint = contactManifoldConstraint;
            this.contact = contact;
            isActive = true;

            entityA = contactManifoldConstraint.EntityA;
            entityB = contactManifoldConstraint.EntityB;

        }

        ///<summary>
        /// Cleans up the constraint.
        ///</summary>
        public void CleanUp()
        {
            accumulatedImpulse = (fint)0;
            contactManifoldConstraint = null;
            contact = null;
            entityA = null;
            entityB = null;
            isActive = false;


        }

        /// <summary>
        /// Gets the total normal impulse applied by this penetration constraint to maintain the separation of the involved entities.
        /// </summary>
        public fint NormalImpulse
        {
            get { return accumulatedImpulse; }
        }

        ///<summary>
        /// Gets the relative velocity between the associated entities at the contact point along the contact normal.
        ///</summary>
        public fint RelativeVelocity
        {
            get
            {
                fint lambda = (fint)0;
                if (entityA != null)
                {
                    lambda = entityA.linearVelocity.X * linearAX + entityA.linearVelocity.Y * linearAY + entityA.linearVelocity.Z * linearAZ +
                             entityA.angularVelocity.X * angularAX + entityA.angularVelocity.Y * angularAY + entityA.angularVelocity.Z * angularAZ;
                }
                if (entityB != null)
                {
                    lambda += -entityB.linearVelocity.X * linearAX - entityB.linearVelocity.Y * linearAY - entityB.linearVelocity.Z * linearAZ +
                              entityB.angularVelocity.X * angularBX + entityB.angularVelocity.Y * angularBY + entityB.angularVelocity.Z * angularBZ;
                }
                return lambda;
            }
        }




        ///<summary>
        /// Performs the frame's configuration step.
        ///</summary>
        ///<param name="dt">Timestep duration.</param>
        public override void Update(fint dt)
        {

            entityADynamic = entityA != null && entityA.isDynamic;
            entityBDynamic = entityB != null && entityB.isDynamic;

            //Set up the jacobians.
            linearAX = -contact.Normal.X;
            linearAY = -contact.Normal.Y;
            linearAZ = -contact.Normal.Z;
            //linearBX = -linearAX;
            //linearBY = -linearAY;
            //linearBZ = -linearAZ;



            //angular A = Ra x N
            if (entityA != null)
            {
                Vector3.Subtract(ref contact.Position, ref entityA.position, out ra);
                angularAX = (ra.Y * linearAZ) - (ra.Z * linearAY);
                angularAY = (ra.Z * linearAX) - (ra.X * linearAZ);
                angularAZ = (ra.X * linearAY) - (ra.Y * linearAX);
            }


            //Angular B = N x Rb
            if (entityB != null)
            {
                Vector3.Subtract(ref contact.Position, ref entityB.position, out rb);
                angularBX = (linearAY * rb.Z) - (linearAZ * rb.Y);
                angularBY = (linearAZ * rb.X) - (linearAX * rb.Z);
                angularBZ = (linearAX * rb.Y) - (linearAY * rb.X);
            }


            //Compute inverse effective mass matrix
            fint entryA, entryB;

            //these are the transformed coordinates
            fint tX, tY, tZ;
            if (entityADynamic)
            {
                tX = angularAX * entityA.inertiaTensorInverse.M11 + angularAY * entityA.inertiaTensorInverse.M21 + angularAZ * entityA.inertiaTensorInverse.M31;
                tY = angularAX * entityA.inertiaTensorInverse.M12 + angularAY * entityA.inertiaTensorInverse.M22 + angularAZ * entityA.inertiaTensorInverse.M32;
                tZ = angularAX * entityA.inertiaTensorInverse.M13 + angularAY * entityA.inertiaTensorInverse.M23 + angularAZ * entityA.inertiaTensorInverse.M33;
                entryA = tX * angularAX + tY * angularAY + tZ * angularAZ + entityA.inverseMass;
            }
            else
                entryA = (fint)0;

            if (entityBDynamic)
            {
                tX = angularBX * entityB.inertiaTensorInverse.M11 + angularBY * entityB.inertiaTensorInverse.M21 + angularBZ * entityB.inertiaTensorInverse.M31;
                tY = angularBX * entityB.inertiaTensorInverse.M12 + angularBY * entityB.inertiaTensorInverse.M22 + angularBZ * entityB.inertiaTensorInverse.M32;
                tZ = angularBX * entityB.inertiaTensorInverse.M13 + angularBY * entityB.inertiaTensorInverse.M23 + angularBZ * entityB.inertiaTensorInverse.M33;
                entryB = tX * angularBX + tY * angularBY + tZ * angularBZ + entityB.inverseMass;
            }
            else
                entryB = (fint)0;

            //If we used a single fixed softness value, then heavier objects will tend to 'squish' more than light objects.
            //In the extreme case, very heavy objects could simply fall through the ground by force of gravity.
            //To see why this is the case, consider that a given dt, softness, and bias factor correspond to an equivalent spring's damping and stiffness coefficients.
            //Imagine trying to hang objects of different masses on the fixed-strength spring: obviously, heavier ones will pull it further down.

            //To counteract this, scale the softness value based on the effective mass felt by the constraint.
            //Larger effective masses should correspond to smaller softnesses so that the spring has the same positional behavior.
            //Fortunately, we're already computing the necessary values: the raw, unsoftened effective mass inverse shall be used to compute the softness.

            fint effectiveMassInverse = entryA + entryB;
            fint updateRate = (fint)1 / dt;
            softness = CollisionResponseSettings.Softness * effectiveMassInverse * updateRate;
            velocityToImpulse = (fint)(-1) / (softness + effectiveMassInverse);


            //Bounciness and bias (penetration correction)
            if (contact.PenetrationDepth >= (fint)0)
            {
                bias = MathHelper.Min(
                    MathHelper.Max((fint)0, contact.PenetrationDepth - CollisionDetectionSettings.AllowedPenetration) *
                    CollisionResponseSettings.PenetrationRecoveryStiffness * updateRate,
                    CollisionResponseSettings.MaximumPenetrationRecoverySpeed);

                if (contactManifoldConstraint.materialInteraction.Bounciness > (fint)0)
                {
                    //Target a velocity which includes a portion of the incident velocity.
                    fint bounceVelocity = -RelativeVelocity;
                    if (bounceVelocity > (fint)0)
                    {
                        var lowThreshold = CollisionResponseSettings.BouncinessVelocityThreshold * (fint)0.3f;
                        var velocityFraction = MathHelper.Clamp((bounceVelocity - lowThreshold) / (CollisionResponseSettings.BouncinessVelocityThreshold - lowThreshold + Toolbox.Epsilon), (fint)0, (fint)1);
                        var bouncinessVelocity = velocityFraction * bounceVelocity * contactManifoldConstraint.materialInteraction.Bounciness;
                        bias = MathHelper.Max(bouncinessVelocity, bias);
                    }
                }
            }
            else
            {
                //The contact is actually separated right now.  Allow the solver to target a position that is just barely in collision.
                //If the solver finds that an accumulated negative impulse is required to hit this target, then no work will be done.
                bias = contact.PenetrationDepth * updateRate;

                //This implementation is going to ignore bounciness for now.
                //Since it's not being used for CCD, these negative-depth contacts
                //only really occur in situations where no bounce should occur.

                //if (contactManifoldConstraint.materialInteraction.Bounciness > 0)
                //{
                //    //Target a velocity which includes a portion of the incident velocity.
                //    //The contact isn't colliding currently, but go ahead and target the post-bounce velocity.
                //    //The bias is added to the bounce velocity to simulate the object continuing to the surface and then bouncing off.
                //    sfloat relativeVelocity = -RelativeVelocity;
                //    if (relativeVelocity > CollisionResponseSettings.BouncinessVelocityThreshold)
                //        bias = relativeVelocity * contactManifoldConstraint.materialInteraction.Bounciness + bias;
                //}
            }


        }

        /// <summary>
        /// Performs any pre-solve iteration work that needs exclusive
        /// access to the members of the solver updateable.
        /// Usually, this is used for applying warmstarting impulses.
        /// </summary>
        public override void ExclusiveUpdate()
        {
            //Warm starting
#if !WINDOWS
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();
#else
            Vector3 linear, angular;
#endif
            linear.X = accumulatedImpulse * linearAX;
            linear.Y = accumulatedImpulse * linearAY;
            linear.Z = accumulatedImpulse * linearAZ;
            if (entityADynamic)
            {
                angular.X = accumulatedImpulse * angularAX;
                angular.Y = accumulatedImpulse * angularAY;
                angular.Z = accumulatedImpulse * angularAZ;
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.X = -linear.X;
                linear.Y = -linear.Y;
                linear.Z = -linear.Z;
                angular.X = accumulatedImpulse * angularBX;
                angular.Y = accumulatedImpulse * angularBY;
                angular.Z = accumulatedImpulse * angularBZ;
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }
        }


        /// <summary>
        /// Computes and applies an impulse to keep the colliders from penetrating.
        /// </summary>
        /// <returns>Impulse applied.</returns>
        public override fint SolveIteration()
        {

            //Compute relative velocity
            fint lambda = (RelativeVelocity - bias + softness * accumulatedImpulse) * velocityToImpulse;

            //Clamp accumulated impulse
            fint previousAccumulatedImpulse = accumulatedImpulse;
            accumulatedImpulse = MathHelper.Max((fint)0, accumulatedImpulse + lambda);
            lambda = accumulatedImpulse - previousAccumulatedImpulse;


            //Apply the impulse
#if !WINDOWS
            Vector3 linear = new Vector3();
            Vector3 angular = new Vector3();
#else
            Vector3 linear, angular;
#endif
            linear.X = lambda * linearAX;
            linear.Y = lambda * linearAY;
            linear.Z = lambda * linearAZ;
            if (entityADynamic)
            {
                angular.X = lambda * angularAX;
                angular.Y = lambda * angularAY;
                angular.Z = lambda * angularAZ;
                entityA.ApplyLinearImpulse(ref linear);
                entityA.ApplyAngularImpulse(ref angular);
            }
            if (entityBDynamic)
            {
                linear.X = -linear.X;
                linear.Y = -linear.Y;
                linear.Z = -linear.Z;
                angular.X = lambda * angularBX;
                angular.Y = lambda * angularBY;
                angular.Z = lambda * angularBZ;
                entityB.ApplyLinearImpulse(ref linear);
                entityB.ApplyAngularImpulse(ref angular);
            }

            return fint.Abs(lambda);
        }

        protected internal override void CollectInvolvedEntities(RawList<Entity> outputInvolvedEntities)
        {
            //This should never really have to be called.
            if (entityA != null)
                outputInvolvedEntities.Add(entityA);
            if (entityB != null)
                outputInvolvedEntities.Add(entityB);
        }

    }
}