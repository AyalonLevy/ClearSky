using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioPool audioPool;
    public AudioClip background;

    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup SFXGroup;

    [Header("Mixer Snapshots")]
    public AudioMixer masterMixer;
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot pausedSnapshot;

    private AudioSource musicSource;
    private readonly float transitionDuration = 0.2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        musicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        RestoredSavedVolume();

        if (background != null)
        {
            PlayMusic(background, true);
        }
    }

    public void PlayMusic(AudioClip musicClip, bool loop)
    {
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.clip = musicClip;
        musicSource.loop = loop;

        musicSource.Play();
    }

    public void PlaySound(AudioEvent data, Vector3 position)
    {
        PooledAudioSource audioData = audioPool.GetObject();
        audioData.Initialize(data, SFXGroup);

        audioData.SetPool(audioPool);

        audioData.PlaySound(position);
    }

    public void SetPausedState(bool isPaused)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionSnapshots(isPaused ? 1.0f : 0.0f, transitionDuration));
    }

    private IEnumerator TransitionSnapshots(float targetPausedWeight, float duration)
    {
        float time = 0;
        AudioMixerSnapshot[] snapshots = {normalSnapshot, pausedSnapshot};

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;

            float[] weights = { 1.0f - targetPausedWeight * t, targetPausedWeight * t };

            masterMixer.TransitionToSnapshots(snapshots, weights, 0.0f);
            yield return null;
        }

        float[] finalWeights = { 1.0f - targetPausedWeight, targetPausedWeight };
        masterMixer.TransitionToSnapshots(snapshots, finalWeights, 0.0f);
    }
    public void RestoredSavedVolume()
    {
        float value = PlayerPrefs.GetFloat("SavedVolume", 1.0f);  // Default volume level is 0dB

        masterMixer.SetFloat("MasterVolume", value);
    }
}
