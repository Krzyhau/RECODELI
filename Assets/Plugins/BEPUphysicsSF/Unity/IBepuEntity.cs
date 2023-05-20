
namespace BEPUphysics.Unity
{
    public interface IBepuEntity
    {
        bool Initialized { get; }
        BepuSimulation Simulation { get; }
        void Initialize(BepuSimulation simulation);
        void PhysicsUpdate();
        void PostPhysicsUpdate();
    }
}