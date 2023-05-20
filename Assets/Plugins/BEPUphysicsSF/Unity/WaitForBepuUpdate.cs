using BEPUphysics.Unity;
using SoftFloat;
using UnityEngine;

namespace BEPUphysics.Unity
{
    public class WaitForBepuUpdate : CustomYieldInstruction
    {
        private readonly BepuSimulation simulation;
        private sfloat cachedTime;
        public override bool keepWaiting => simulation != null && cachedTime == simulation.SimulationTime;

        public WaitForBepuUpdate(BepuSimulation simulation)
        {
            this.simulation = simulation;
            cachedTime = (simulation != null) ? simulation.SimulationTime : sfloat.Zero;
        }
        public WaitForBepuUpdate(IBepuEntity entity) : this(entity.Simulation) { }
    }
}
