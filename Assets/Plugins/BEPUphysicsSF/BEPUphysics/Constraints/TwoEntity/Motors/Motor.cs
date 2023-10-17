using BEPUutilities.FixedMath;

namespace BEPUphysics.Constraints.TwoEntity.Motors
{
    /// <summary>
    /// Superclass of constraints which do work and change the velocity of connected entities, but have no specific position target.
    /// </summary>
    public abstract class Motor : TwoEntityConstraint
    {
        protected fint maxForceDt = fint.MaxValue;
        protected fint maxForceDtSquared = fint.MaxValue;

        /// <summary>
        /// Softness divided by the timestep to maintain timestep independence.
        /// </summary>
        internal fint usedSoftness;

        /// <summary>
        /// Computes the maxForceDt and maxForceDtSquared fields.
        /// </summary>
        protected void ComputeMaxForces(fint maxForce, fint dt)
        {
            //Determine maximum force
            if (maxForce < fint.MaxValue)
            {
                maxForceDt = maxForce * dt;
                maxForceDtSquared = maxForceDt * maxForceDt;
            }
            else
            {
                maxForceDt = fint.MaxValue;
                maxForceDtSquared = fint.MaxValue;
            }
        }
    }
}