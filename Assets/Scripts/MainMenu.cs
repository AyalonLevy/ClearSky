using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private LevelLoader levelLoader;


    private void Start()
    {
        levelLoader = FindAnyObjectByType<LevelLoader>();
    }

    public void PlayGame()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (levelLoader == null)
        { 
            SceneManager.LoadScene(nextSceneIndex); 
        }
        else
        {
            levelLoader.LoadNextLevel(nextSceneIndex);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
