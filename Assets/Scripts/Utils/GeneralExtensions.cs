using UnityEngine;

namespace RecoDeli.Scripts.Utils
{
    public static class GeneralExtensions
    {
        #region Vector3
        public static Vector3 ChangeX
            (this Vector3 vector3, float newValueX) => new (newValueX, vector3.y, vector3.z);

        public static Vector3 ChangeY
           (this Vector3 vector3, float newValueY) => new (vector3.x, newValueY, vector3.z);

        public static Vector3 ChangeZ
            (this Vector3 vector3, float newValueZ) => new (vector3.x, vector3.y, newValueZ);
        #endregion

        #region Vector2
        public static Vector2 ChangeX
            (this Vector2 vector2, float newValueX) => new (newValueX, vector2.y);

        public static Vector2 ChangeY
          (this Vector2 vector3, float newValueY) => new (vector3.x, newValueY);

        #endregion
    }
}
