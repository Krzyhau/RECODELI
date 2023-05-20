using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace BEPUphysics.Unity
{
    public interface IBepuEntityListener
    {
        public void BepuUpdate() { }
        public void OnBepuCollisionEnter(Collidable other, CollidablePairHandler info) { }
        public void OnBepuCollisionStay(Collidable other, CollidablePairHandler info) { }
        public void OnBepuCollisionExit(Collidable other, CollidablePairHandler info) { } 
    }
}
