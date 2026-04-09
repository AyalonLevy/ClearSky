using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI powerText;
    public TextMeshProUGUI reloadRateText;
    public TextMeshProUGUI timeSurvivedText;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject buttonControl;

    [Header("Icon Groups")]
    public HorizontalLayoutGroup ammoGroup;
    public HorizontalLayoutGroup healthGroup;
    public GameObject ammoPrefab;
    public GameObject healthPrefab;

    private readonly List<GameObject> ammoIcons = new();
    private readonly List<GameObject> healthIcons = new();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeUI(int maxHealth, int maxAmmo)
    {
        // Creates all the UI elements for the changing parts (life and ammo) to improve memory usage
        for (int i = 0; i < maxHealth; i++)
        {
            healthIcons.Add(Instantiate(healthPrefab, healthGroup.transform));
        }

        for (int i = 0; i < maxAmmo; i++)
        {
            ammoIcons.Add(Instantiate(ammoPrefab, ammoGroup.transform));
        }
    }


    public void UpdateScore(int score) => scoreText.SetText("Score :  {0}", score);
    public void UpdateDamage(float damage) => powerText.text = damage.ToString();
    public void UpdateReloadTime(float reloadTime) => reloadRateText.text = reloadTime.ToString();

    public void UpdateHealthDisplay(int currentHealth)
    {
        for (int i = 0; i < healthIcons.Count; i++)
        {
            healthIcons[i].SetActive(i < currentHealth);
        }
    }
    public void UpdateAmmoDisplay(int currentAmmo)
    {
        for (int i = 0; i < ammoIcons.Count; i++)
        {
            ammoIcons[i].SetActive(i < currentAmmo);
        }
    }

    private string FloatToTimeFormat(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (minutes < 60)
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            int hours = Mathf.FloorToInt(minutes / 60);
            minutes = Mathf.FloorToInt(hours % 60);

            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
    }

    public void ToggleGameOverMenu(bool isDisplayed)
    {
        // Get time from GameManager and set the TimeSurvived varable
        float timeSurvived = GameManager.instance.GetTimePassed();
        timeSurvivedText.SetText("Time Survived :  " + FloatToTimeFormat(timeSurvived));

        gameOverMenu.SetActive(isDisplayed);
    }

    public void TogglePauseMenu(bool isDisplayed)
    {
        pauseMenu.SetActive(isDisplayed);
    }

    public void ToggleActionButton(bool isActive)
    {
        if (buttonControl != null)
        {
            buttonControl.SetActive(isActive);
        }
    }
}
