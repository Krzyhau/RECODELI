
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecoDeli.Scripts.Utils
{
    public static class UIToolkitExtensions
    {
        public static VisualElement GetRootElement(this VisualElement element)
        {
            var root = element;
            while (root.parent != null)
            {
                root = root.parent;
            }

            return root;
        }

        public static bool ContainsElement(this VisualElement self, VisualElement element)
        {
            var check = element;
            while (check != null)
            {
                if (self == check) return true;
                check = check.parent;
            }

            return false;
        }

        static PropertyInfo _focusRingPropertyInfo;
        static PropertyInfo _navDirectionPropertyInfo;
        public static Focusable GetNextFocusable(this FocusController focusController, Focusable focusable, NavigationMoveEvent.Direction direction)
        {
            if (_focusRingPropertyInfo == null)
            {
                _focusRingPropertyInfo = typeof(FocusController).GetProperty("focusRing", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
                _navDirectionPropertyInfo = typeof(NavigationMoveEvent).GetProperty("direction", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            }

            var evt = new NavigationMoveEvent();
            _navDirectionPropertyInfo.SetValue(evt, direction);

            var focusRing = ((IFocusRing)_focusRingPropertyInfo?.GetValue(focusController));
            var focusChangeDirection = focusRing.GetFocusChangeDirection(focusable, evt);
            return focusRing.GetNextFocusable(focusable, focusChangeDirection);
        }

        static FieldInfo _focusChangeDirectionValueFieldInfo;
        public static int GetValue(this FocusChangeDirection direction)
        {
            if (_focusChangeDirectionValueFieldInfo == null)
            {
                _focusChangeDirectionValueFieldInfo = typeof(FocusChangeDirection).GetField("m_Value", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return (int)_focusChangeDirectionValueFieldInfo.GetValue(direction);
        }

        public static NavigationMoveEvent.Direction ToNavigationDirection(this FocusChangeDirection direction)
        {
            return direction.GetValue() switch
            {
                1 => NavigationMoveEvent.Direction.Left,
                2 => NavigationMoveEvent.Direction.Right,
                3 => NavigationMoveEvent.Direction.Up,
                4 => NavigationMoveEvent.Direction.Down,
                5 => NavigationMoveEvent.Direction.Next,
                6 => NavigationMoveEvent.Direction.Previous,
                _ => NavigationMoveEvent.Direction.None
            };
        }
    }
}
