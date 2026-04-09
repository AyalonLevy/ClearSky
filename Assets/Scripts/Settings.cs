using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle controlTypeToggle;

    private const string ControlSettingKey = "UseButtonControls";

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

    public void SetControlMode(bool useButton)
    {
        int value = useButton ? 1 : 0;
        PlayerPrefs.SetInt(ControlSettingKey, value);
        PlayerPrefs.Save();

        if (GameManager.instance != null)
        {
            GameManager.instance.useButtonControl = useButton;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.ToggleActionButton(useButton);
        }
    }

    private void UpdateSliders()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat("SavedBrightness", 0.0f);
        
        float volumeInDB = PlayerPrefs.GetFloat("SavedVolume", 1.0f);
        volumeSlider.value = Mathf.Pow(10.0f, volumeInDB / 20.0f);  // Convert back to linear slider

        bool useButton = PlayerPrefs.GetInt(ControlSettingKey, 0) == 1;
        controlTypeToggle.isOn = useButton;

        if (GameManager.instance != null)
        {
            GameManager.instance.useButtonControl = useButton;
        }
    }
}
