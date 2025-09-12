using UnityEngine;
using UnityEditor;

public class ReadOnlyAttribute : PropertyAttribute
{
    // This class is intentionally left empty.
    // It serves as a marker for the custom property drawer.
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Disable the GUI to make the field read-only
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true; // Re-enable the GUI
    }
}

#endif