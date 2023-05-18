
namespace BEPUphysics.Unity
{
    public interface IBepuEntity
    {
        bool Initialized { get; }
        void Initialize(BepuSimulation simulation);
        void PhysicsUpdate();
        void PostPhysicsUpdate();
    }
}