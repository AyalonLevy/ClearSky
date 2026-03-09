using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider volumeSlider;

    private void Awake()
    {
        UpdateSliders();
    }

    public void AdjustBrightness(float value)
    {
        PlayerPrefs.SetFloat("SavedBrightness", value);
        PlayerPrefs.Save();

        EnvironmentGlobalManager.instance.UpdateBrightness();
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("SavedVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);  // Save the value in dB
        PlayerPrefs.Save();

        AudioManager.instance.RestoredSavedVolume();
    }

    private void UpdateSliders()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat("SavedBrightness", 0.0f);
        
        float volumeInDB = PlayerPrefs.GetFloat("SavedVolume", 1.0f);
        volumeSlider.value = Mathf.Pow(10.0f, volumeInDB / 20.0f);  // Convert back to linear slider
    }
}
