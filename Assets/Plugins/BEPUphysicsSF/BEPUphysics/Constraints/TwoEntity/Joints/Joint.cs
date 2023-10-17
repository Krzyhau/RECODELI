using System;
using BEPUutilities.FixedMath;

namespace BEPUphysics.Constraints.TwoEntity.Joints
{
    /// <summary>
    /// Superclass of position-based constraints.
    /// </summary>
    public abstract class Joint : TwoEntityConstraint, ISpringSettings
    {
        /// <summary>
        /// Maximum extra velocity that the constraint will apply in an effort to correct constraint error.
        /// </summary>
        protected fint maxCorrectiveVelocity = fint.MaxValue;

        /// <summary>
        /// Squared maximum extra velocity that the constraint will apply in an effort to correct constraint error.
        /// </summary>
        protected fint maxCorrectiveVelocitySquared = fint.MaxValue;

        protected fint softness;

        /// <summary>
        /// Spring settings define how a constraint responds to velocity and position error.
        /// </summary>
        protected SpringSettings springSettings = new SpringSettings();

        /// <summary>
        /// Gets or sets the maximum extra velocity that the constraint will apply in an effort to correct any constraint error.
        /// </summary>
        public fint MaxCorrectiveVelocity
        {
            get { return maxCorrectiveVelocity; }
            set
            {
                maxCorrectiveVelocity = fint.Max((fint)0, value);
                if (maxCorrectiveVelocity >= fint.MaxValue)
                {
                    maxCorrectiveVelocitySquared = fint.MaxValue;
                }
                else
                {
                    maxCorrectiveVelocitySquared = maxCorrectiveVelocity * maxCorrectiveVelocity;
                }
            }
        }

        #region ISpringSettings Members

        /// <summary>
        /// Gets the spring settings used by the constraint.
        /// Spring settings define how a constraint responds to velocity and position error.
        /// </summary>
        public SpringSettings SpringSettings
        {
            get { return springSettings; }
        }

        #endregion
    }
}