using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyDrawer))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string displayValue = prop.propertyType switch
        {
            SerializedPropertyType.Integer => prop.intValue.ToString(),
            SerializedPropertyType.Boolean => prop.boolValue.ToString(),
            SerializedPropertyType.Float => prop.floatValue.ToString(),
            SerializedPropertyType.String => prop.stringValue,
            SerializedPropertyType.Vector2 => prop.vector2Value.ToString(),
            _ => "[UNSUPPORTED DATA TYPE]"
        };

        EditorGUI.LabelField(position, label.text, displayValue);
    }
}