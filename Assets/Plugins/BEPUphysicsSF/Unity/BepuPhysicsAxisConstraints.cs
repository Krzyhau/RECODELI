using System.Collections;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace BepuPhysics.Unity
{
    [Serializable]
    public struct BepuPhysicsAxisConstraints
    {
        public bool X;
        public bool Y;
        public bool Z;

        public bool Any => X || Y || Z;
    }

    [CustomPropertyDrawer(typeof(BepuPhysicsAxisConstraints))]
    public class BepuPhysicsAxisConstraintsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var xRect = new Rect(position.x + 0, position.y, 15, position.height);
            var xLabelRect = new Rect(position.x + 15, position.y, 15, position.height);
            var yRect = new Rect(position.x + 30, position.y, 15, position.height);
            var yLabelRect = new Rect(position.x + 45, position.y, 15, position.height);
            var zRect = new Rect(position.x + 60, position.y, 15, position.height);
            var zLabelRect = new Rect(position.x + 75, position.y, 15, position.height);

            EditorGUI.LabelField(xLabelRect, "X");
            EditorGUI.LabelField(yLabelRect, "Y");
            EditorGUI.LabelField(zLabelRect, "Z");

            EditorGUI.PropertyField(xRect, property.FindPropertyRelative("X"), GUIContent.none);
            EditorGUI.PropertyField(yRect, property.FindPropertyRelative("Y"), GUIContent.none);
            EditorGUI.PropertyField(zRect, property.FindPropertyRelative("Z"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}