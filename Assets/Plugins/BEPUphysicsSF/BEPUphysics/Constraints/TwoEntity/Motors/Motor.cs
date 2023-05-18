using SoftFloat;

namespace BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Superclass of constraints which do work and change the velocity of connected entities, but have no specific position target.
    /// </summary>
    public abstract class Motor : TwoEntityConstraint
    {
        protected sfloat maxForceDt = sfloat.MaxValue;
        protected sfloat maxForceDtSquared = sfloat.MaxValue;

        /// <summary>
        /// Softness divided by the timestep to maintain timestep independence.
        /// </summary>
        internal sfloat usedSoftness;

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        protected void ComputeMaxForces(sfloat maxForce, sfloat dt)
        {
            //Determine maximum force
            if (maxForce < sfloat.MaxValue)
            {
                maxForceDt = maxForce * dt;
                maxForceDtSquared = maxForceDt * maxForceDt;
            }
            else
            {
                maxForceDt = sfloat.MaxValue;
                maxForceDtSquared = sfloat.MaxValue;
            }
        }
    }
}