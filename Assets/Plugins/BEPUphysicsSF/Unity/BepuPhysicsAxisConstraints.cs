using System;

namespace BEPUphysics.Unity
{
    [Serializable]
    public struct BepuPhysicsAxisConstraints
    {
        public bool X;
        public bool Y;
        public bool Z;

        public bool Any => X || Y || Z;
    }
}