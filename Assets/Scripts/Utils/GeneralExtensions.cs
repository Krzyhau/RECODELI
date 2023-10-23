using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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

        #region Transform
        public static Transform Clear(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }
        #endregion

        #region TaskAwaiter
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<AsyncOperation>();
            asyncOp.completed += operation => { tcs.SetResult(operation); };
            return ((Task)tcs.Task).GetAwaiter();
        }

        #endregion
    }
}
