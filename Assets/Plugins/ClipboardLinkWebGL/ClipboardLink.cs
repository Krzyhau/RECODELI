using System.Runtime.InteropServices;
using UnityEngine;

namespace ClipboardLinkWebGL
{
    public class ClipboardLink
    {
        [DllImport("__Internal")]
        private static extern string GetClipboardContent();

        [DllImport("__Internal")]
        private static extern void SetClipboardContent(string clipboardContent);

        [DllImport("__Internal")]
        private static extern bool IsClipboardAccessible();

        public static string Content
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                if (IsClipboardAccessible()) return GetClipboardContent();
                else return GUIUtility.systemCopyBuffer;
#else
                return GUIUtility.systemCopyBuffer;
#endif
            }
            set
            {
                GUIUtility.systemCopyBuffer = value;
#if UNITY_WEBGL && !UNITY_EDITOR
                if (IsClipboardAccessible()) SetClipboardContent(value);
#endif
            }
        }
    }
}
