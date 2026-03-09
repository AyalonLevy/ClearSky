using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class PooledAudioSource : MonoBehaviour
{
    private AudioSource audioSource;

    private AudioPool myPool;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(AudioEvent data, AudioMixerGroup group)
    {
        audioSource.clip = data.clip;
        audioSource.outputAudioMixerGroup = group;

        audioSource.volume = data.volume + Random.Range(-data.volumeVariance, data.volumeVariance);
        audioSource.pitch = data.pitch + Random.Range(-data.pitchVariance, data.pitchVariance);
    }

    public void SetPool(AudioPool pool)
    {
        myPool = pool;
    }

    public void PlaySound(Vector3 soundPosition)
    {
        audioSource.transform.position = soundPosition;
        audioSource.Play();

        StartCoroutine(ReturnToPoolAfterFinished());
    }

    private IEnumerator ReturnToPoolAfterFinished()
    {
        yield return new WaitForSeconds(audioSource.clip.length);

        if (myPool != null)
        {
            myPool.ReturnObject(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
