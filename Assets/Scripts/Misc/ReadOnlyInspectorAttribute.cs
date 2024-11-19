using UnityEngine;

public class ReadOnlyInspectorAttribute : PropertyAttribute
{
    // small class that can be used to make certain serialized fields readonly in the inspector
    // use it this way:
    // [ReadOnlyInspector][SerializeField] float ChainRotationSpeed;
    // or
    // [ReadOnlyInspector] public float ChainRotationSpeed;
}
