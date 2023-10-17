using BEPUutilities.FixedMath;

namespace BEPUphysics
{
    ///<summary>
    /// Contains settings for the instance's time step.
    ///</summary>
    public class TimeStepSettings
    {
        /// <summary>
        /// Maximum number of timesteps to perform during a given frame when Space.Update(sfloat) is used.  The unsimulated time will be accumulated for subsequent calls to Space.Update(sfloat).
        /// Defaults to 3.
        /// </summary>
        public int MaximumTimeStepsPerFrame = 3;

        /// <summary>
        /// Length of each integration step.  Calling a Space's Update() method moves time forward this much.
        /// The other method, Space.Update(sfloat), will try to move time forward by the amount specified in the parameter by taking steps of TimeStepDuration size.
        /// Defaults to 1/60.
        /// </summary>
        public fint TimeStepDuration = (fint)1 / (fint)60;

        /// <summary>
        /// Amount of time accumulated by previous calls to Space.Update(sfloat) that has not yet been simulated.
        /// </summary>
        public fint AccumulatedTime;
    }
}
