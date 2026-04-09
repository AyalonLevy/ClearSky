using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public bool isPaused = false;
    [HideInInspector] public bool useButtonControl = false;

    public static GameManager instance;
    [SerializeField] private Player player;

    public AudioEvent gameOverSound;

    [Header("Level Settings")]
    [SerializeField] private int pointsPerLevel = 10;
    private int currentLevel = 1;

    private LevelLoader levelLoader;
    private int score = 0;
    private float elapsedTime = 0.0f;


    // TDD
    public void SetPlayer(Player p) => player = p;

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

    private void Start()
    {
        elapsedTime = 0.0f;

        if (UIManager.instance != null)
        {
            UIManager.instance.InitializeUI(player.maxHealth, player.maxAmmo);
            RefreshAllUI();
        }

        levelLoader = FindAnyObjectByType<LevelLoader>();
        useButtonControl = PlayerPrefs.GetInt("UseButtonControls", 0) == 1;
        UIManager.instance.ToggleActionButton(useButtonControl);
    }

    public void AddScore()
    {
        score++;
        UIManager.instance.UpdateScore(score);

        if (score > 0 && score % pointsPerLevel == 0)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentLevel++;

        // TODO: Notify UI and show LEVEL X message
        UIManager.instance.ShowLevelMessage(currentLevel);

        Spawner spawner = FindFirstObjectByType<Spawner>();
        if (spawner != null)
        {
            spawner.IncreaseLevelDifficulty(currentLevel);
        }
    }

    public void RefreshAllUI()
    {
        UIManager.instance.UpdateScore(score);
        UIManager.instance.UpdateHealthDisplay(player.currentHealth);
        UIManager.instance.UpdateAmmoDisplay(player.currentAmmo);
        UIManager.instance.UpdateDamage(player.TotalDamage());
        UIManager.instance.UpdateReloadTime(player.reloadTime);
    }

    public void UpdateHealth()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateHealthDisplay(player.currentHealth);
        }
    }

    public void UpdateAmmo()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateAmmoDisplay(player.currentAmmo);
        }
    }

    public void UpdateDamage()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateDamage(player.TotalDamage());
        }
    }

    public void UpdateReloadTime()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UpdateReloadTime(player.reloadTime);
        }
    }

    private void ToggleGameState()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
    }

    public void PauseGame()
    {
        ToggleGameState();
        UIManager.instance.TogglePauseMenu(isPaused);
        AudioManager.instance.SetPausedState(isPaused);
    }

    public void ReturnToGame()
    {
        ToggleGameState();
        UIManager.instance.TogglePauseMenu(isPaused);
        AudioManager.instance.SetPausedState(isPaused);
    }

    public float GetTimePassed()
    {
        return elapsedTime;
    }

    public void GameOver()
    {
        elapsedTime = Time.time;
        ToggleGameState();

        UIManager.instance.ToggleGameOverMenu(true);

        AudioManager.instance.PlaySound(gameOverSound, transform.position);
    }

    public void RestartGame()
    {
        ToggleGameState();
        UIManager.instance.ToggleGameOverMenu(false);
        levelLoader.LoadNextLevel(SceneManager.GetActiveScene().buildIndex);
    }
}
