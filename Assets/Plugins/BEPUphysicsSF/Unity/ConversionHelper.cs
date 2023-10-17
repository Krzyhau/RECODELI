
using BEPUutilities.FixedMath;

namespace BEPUphysics.Unity
{
    public static class ConversionHelper
    {
        public static UnityEngine.Vector2 ToUnity(this BEPUutilities.Vector2 vector)
        {
            return new UnityEngine.Vector2(
                (float)vector.X,
                (float)vector.Y
            );
        }

        public static BEPUutilities.Vector2 ToBEPU(this UnityEngine.Vector2 vector)
        {
            return new BEPUutilities.Vector2(
                (fint)vector.x,
                (fint)vector.y
            );
        }
        public static UnityEngine.Vector3 ToUnity(this BEPUutilities.Vector3 vector)
        {
            return new UnityEngine.Vector3(
                (float)vector.X,
                (float)vector.Y, 
                (float)vector.Z
            );
        }

        public static BEPUutilities.Vector3 ToBEPU(this UnityEngine.Vector3 vector)
        {
            return new BEPUutilities.Vector3(
                (fint)vector.x,
                (fint)vector.y,
                (fint)vector.z
            );
        }

        public static UnityEngine.Quaternion ToUnity(this BEPUutilities.Quaternion vector)
        {
            return new UnityEngine.Quaternion(
                (float)vector.X,
                (float)vector.Y,
                (float)vector.Z,
                (float)vector.W
            );
        }

        public static BEPUutilities.Quaternion ToBEPU(this UnityEngine.Quaternion vector)
        {
            return new BEPUutilities.Quaternion(
                (fint)vector.x,
                (fint)vector.y,
                (fint)vector.z,
                (fint)vector.w
            );
        }
    }
}