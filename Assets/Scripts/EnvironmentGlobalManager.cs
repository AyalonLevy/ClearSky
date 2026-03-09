using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EnvironmentGlobalManager : MonoBehaviour
{
    public static EnvironmentGlobalManager instance;

    private Volume volume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            ApplyAllSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplyAllSettings();

    public void ApplyAllSettings()
    {
        UpdateBrightness();
    }

    public void UpdateBrightness()
    {
        if (volume == null)
        {
            volume = FindFirstObjectByType<Volume>();
        }

        if (volume != null && volume.profile.TryGet(out ColorAdjustments color))
        {
            color.postExposure.value = PlayerPrefs.GetFloat("SavedBrightness", 0.0f);
        }
    }
}
