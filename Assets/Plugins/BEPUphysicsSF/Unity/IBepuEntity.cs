
using BEPUphysics.Entities;
using UnityEngine;

namespace BEPUphysics.Unity
{
    public interface IBepuEntity
    {
        bool Initialized { get; }
        Entity Entity { get; }
        BepuSimulation Simulation { get; }
        GameObject GameObject { get; }
        void Initialize(BepuSimulation simulation);
        void PhysicsUpdate();
        void PostPhysicsUpdate();
    }
}