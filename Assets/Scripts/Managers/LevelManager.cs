using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public List<Level> levels = new List<Level>();
    public Level currentLevel;
    private int currentLevelIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        currentLevelIndex = PlayerPrefs.GetInt("CURRENT_LEVEL_KEY", 0);

        SetCurrentLevel();
    }

    private void SetCurrentLevel()
    {
        currentLevel = levels[currentLevelIndex];
    }

    public void LevelCompleted()
    {
        currentLevelIndex++;

        if (currentLevelIndex == levels.Count)
            currentLevelIndex = 0;

        PlayerPrefs.SetInt("CURRENT_LEVEL_KEY" , currentLevelIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
