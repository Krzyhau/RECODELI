
namespace BEPUphysics.Unity
{
    public interface IBepuEntity
    {
        void Initialize(BepuSimulation simulation);
        void PhysicsUpdate();
        void PostPhysicsUpdate();
    }
}