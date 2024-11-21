using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Current { get; protected set; }

    [ReadOnlyInspector][SerializeField] AudioSource audioSource;
    [SerializeField] AudioList audioList;

    void Reset()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Awake()
    {
        Current = this;
    }

    public static void Play(string soundName)
    {
        AudioData audioData = Current.audioList.Get(soundName);

        if (audioData == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Sound effect '" + soundName + "' cannot be played - no such sound can be found among the audio list assets!");
#endif
            return;
        }
        if (audioData.clips.Length == 0)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Sound effect '" + soundName + "' cannot be played - its array of audio clips is empty!");
#endif
            return;
        }

        float basePitch = 1;
        if (audioData.pitchVariation > 0)
        {
            float pitchRandomness = Random.Range(-audioData.pitchVariation, audioData.pitchVariation);
            Current.audioSource.pitch = basePitch + pitchRandomness;
        }
        else { Current.audioSource.pitch = basePitch; }

        //Current.audioSource.clip = audioData.clips[Random.Range(0, audioData.clips.Length)];
        //Current.audioSource.volume = audioData.volume <= 0 ? 1 : audioData.volume;

        Current.audioSource.PlayOneShot(audioData.clips[Random.Range(0, audioData.clips.Length)], audioData.volume);
    }
}
