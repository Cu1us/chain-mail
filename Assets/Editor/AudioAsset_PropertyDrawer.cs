using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;


[CustomPropertyDrawer(typeof(AudioAsset))]
public class AudioAsset_PropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var foldout = new Foldout();

        SerializedProperty soundName = property.FindPropertyRelative("name");
        SerializedProperty data = property.FindPropertyRelative("data");

        foldout.text = string.IsNullOrEmpty(soundName.stringValue) ? "Sound Effect" : soundName.stringValue;
        foldout.Add(new PropertyField(soundName, "Name"));
        foldout.Add(new PropertyField(data, "Data"));
        container.Add(foldout);

        return container;
    }
}

[CustomPropertyDrawer(typeof(AudioData))]
public class AudioData_PropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var container = new VisualElement();

        var popup = new UnityEngine.UIElements.PopupWindow();

        SerializedProperty soundClips = property.FindPropertyRelative("clips");
        SerializedProperty pitchVariation = property.FindPropertyRelative("pitchVariation");
        SerializedProperty volume = property.FindPropertyRelative("volume");

        popup.text = "Audio Data";
        popup.Add(new PropertyField(soundClips, "Clips"));
        popup.Add(new PropertyField(pitchVariation, "Pitch Variation"));
        popup.Add(new PropertyField(volume, "Volume"));

        container.Add(popup);

        return container;
    }
}
