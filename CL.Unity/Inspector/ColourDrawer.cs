using UnityEditor;
using UnityEngine;

using static CL.Common.CustomLicense;

namespace CL.Unity.Inspector
{
    [CustomPropertyDrawer(typeof(Colour))]
    internal class ColourDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var r = property.FindPropertyRelative(nameof(Colour.R));
            var g = property.FindPropertyRelative(nameof(Colour.G));
            var b = property.FindPropertyRelative(nameof(Colour.B));
            var a = property.FindPropertyRelative(nameof(Colour.A));

            EditorGUI.BeginProperty(position, label, property);

            var result = EditorGUI.ColorField(position, label, new Color(r.floatValue, g.floatValue, b.floatValue, a.floatValue));

            r.floatValue = result.r;
            g.floatValue = result.g;
            b.floatValue = result.b;
            a.floatValue = result.a;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
