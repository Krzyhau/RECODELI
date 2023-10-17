using System;
using BEPUutilities.FixedMath;
using BEPUphysics.Constraints.TwoEntity.Joints;
using BEPUutilities;
 

namespace BEPUphysics.Constraints.TwoEntity.JointLimits
{
    /// <summary>
    /// Superclass of constraints which have a limited area of free movement.
    /// </summary>
    public abstract class JointLimit : Joint
    {
        /// <summary>
        /// Minimum velocity necessary for a bounce to occur at a joint limit.
        /// </summary>
        protected fint bounceVelocityThreshold = (fint)1;

        /// <summary>
        /// Bounciness of this joint limit.  0 is completely inelastic; 1 is completely elastic.
        /// </summary>
        protected fint bounciness;

        protected bool isLimitActive;

        /// <summary>
        /// Small area that the constraint can be violated without applying position correction.  Helps avoid jitter.
        /// </summary>
        protected fint margin = (fint)0.005f;

        /// <summary>
        /// Gets or sets the minimum velocity necessary for a bounce to occur at a joint limit.
        /// </summary>
        public fint BounceVelocityThreshold
        {
            get { return bounceVelocityThreshold; }
            set { bounceVelocityThreshold = fint.Max((fint)0, value); }
        }

        /// <summary>
        /// Gets or sets the bounciness of this joint limit.  0 is completely inelastic; 1 is completely elastic.
        /// </summary>
        public fint Bounciness
        {
            get { return bounciness; }
            set { bounciness = MathHelper.Clamp(value, (fint)0, (fint)1); }
        }

        /// <summary>
        /// Gets whether or not the limit is currently exceeded.  While violated, the constraint will apply impulses in an attempt to stop further violation and to correct any current error.
        /// This is true whenever the limit is touched.
        /// </summary>
        public bool IsLimitExceeded
        {
            get { return isLimitActive; }
        }

        /// <summary>
        /// Gets or sets the small area that the constraint can be violated without applying position correction.  Helps avoid jitter.
        /// </summary>
        public fint Margin
        {
            get { return margin; }
            set { margin = MathHelper.Max(value, (fint)0); }
        }

        /// <summary>
        /// Computes the bounce velocity for this limit.
        /// </summary>
        /// <param name="impactVelocity">Velocity of the impact on the limit.</param>
        /// <returns>The resulting bounce velocity of the impact.</returns>
        protected fint ComputeBounceVelocity(fint impactVelocity)
        {
            var lowThreshold = bounceVelocityThreshold * (fint)0.3f;
            var velocityFraction = MathHelper.Clamp((impactVelocity - lowThreshold) / (bounceVelocityThreshold - lowThreshold + Toolbox.Epsilon), (fint)0, (fint)1);
            return velocityFraction * impactVelocity * Bounciness;
        }

    }
}