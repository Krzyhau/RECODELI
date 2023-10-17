using BEPUphysics.Unity;
using BEPUutilities.FixedMath;
using UnityEngine;

namespace BEPUphysics.Unity
{
    public class WaitForBepuUpdate : CustomYieldInstruction
    {
        private readonly BepuSimulation simulation;
        private fint cachedTime;
        public override bool keepWaiting => simulation != null && cachedTime == simulation.SimulationTime;

        public WaitForBepuUpdate(BepuSimulation simulation)
        {
            this.simulation = simulation;
            cachedTime = (simulation != null) ? simulation.SimulationTime : (fint)0;
        }
        public WaitForBepuUpdate(IBepuEntity entity) : this(entity.Simulation) { }
    }
}
