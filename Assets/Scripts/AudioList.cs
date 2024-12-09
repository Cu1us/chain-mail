using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio List", menuName = "Audio List")]
public class AudioList : ScriptableObject
{
    [SerializeField] AudioAsset[] audioAssets;

    static bool initialized = false;
    readonly Dictionary<string, AudioData> audioDictionary = new();

    [RuntimeInitializeOnLoadMethod]
    static void OnStartup()
    {
        initialized = false;
    }
    public void InitializeDictionary()
    {
        Debug.Log("Initializing sound list");
        foreach (AudioAsset audioAsset in audioAssets)
        {
            audioDictionary[audioAsset.name.ToLower()] = audioAsset.data;
        }
        initialized = true;
    }

    public AudioData Get(string soundName)
    {
        if (!initialized) InitializeDictionary();
        return audioDictionary.TryGetValue(soundName.ToLower(), out AudioData data) ? data : null;
    }
}

[System.Serializable]
public class AudioAsset
{
    [SerializeField] public string name;
    [SerializeField] public AudioData data;
}

[System.Serializable]
public class AudioData
{
    [SerializeField] public AudioClip[] clips;
    [SerializeField][Min(0)] public float pitchVariation;
    [SerializeField][Range(0, 1)] public float volume = 1;
}