using UnityEngine;

[CreateAssetMenu(fileName = "AudioEvent", menuName = "Scriptable Objects/AudioEvent")]
public class AudioEvent : ScriptableObject
{
    public AudioClip clip;

    [Range(0.0f, 1.0f)] public float volume = 1f;
    [Range(0.0f, 0.5f)] public float volumeVariance = 0.1f;

    [Range(0.1f, 3.0f)] public float pitch = 1f;
    [Range(0.0f, 0.5f)] public float pitchVariance = 0.1f;
}
