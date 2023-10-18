using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public int currentRemainingMoves, goal1Remaining, goal2Remaining;

    [HideInInspector] public TileType goal1Type, goal2Type;

    private bool goal1Completed = false, goal2Completed = false;
    private bool levelCompleted = false;
    public static GameManager Instance;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }

    private void Start()
    {
        Initialize(LevelManager.Instance.currentLevel);

        StartGame();
    }

    public void StartGame()
    {
        TileManager.Instance.StartGame();
    }


    public void Initialize(Level level)
    {
        currentRemainingMoves = level.maxMoves;
        goal1Remaining = level.goal1;
        goal2Remaining = level.goal2;
        goal1Type = level.goal1Type;
        goal2Type = level.goal2Type;

        GameplayUIController.Instance.UpdateGoalTexts(goal1Remaining, goal2Remaining);
    }

    public void LowerGoals(int goal1LowerAmount, int goal2LowerAmount)
    {
        goal1Remaining -= goal1LowerAmount;
        goal2Remaining -= goal2LowerAmount;

        if (goal1Remaining == 0 && !goal1Completed)
        {
            goal1Completed = true;

            GameplayUIController.Instance.TickGoalSection(1);
        }
        else if(goal2Remaining == 0 && !goal2Completed)
        {
            goal2Completed = true;

            GameplayUIController.Instance.TickGoalSection(2);
        }

        if (goal1Completed && goal2Completed && currentRemainingMoves >= 0 && !levelCompleted)
        {
            levelCompleted = true;

            LevelManager.Instance.LevelCompleted();
        }

        GameplayUIController.Instance.UpdateGoalTexts(goal1Remaining, goal2Remaining);
    }
    public bool CheckEnoughMoves()
    {
        return currentRemainingMoves > 0;
    }
    public void LowerRemainingMoves()
    {
        currentRemainingMoves--;

        if (currentRemainingMoves <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
            GameplayUIController.Instance.UpdateRemainingMoveText(currentRemainingMoves);

    }

    internal void OnTileDestroyed(TileType type, Vector3 spawnPosition) // EVENT YAPILABILIR.
    {
        if (type == goal1Type)
        {
            GameplayUIController.Instance.PlayGoalCollectedAnimations(1);

            LowerGoals(1, 0);
        }
        else if (type == goal2Type)
        {
            GameplayUIController.Instance.PlayGoalCollectedAnimations(2);

            LowerGoals(0, 1);
        }
    }

    public bool IsTileOnGoal(TileType tileType)
    {

        if (tileType == goal1Type)
        {
            return true;
        }
        else if (tileType == goal2Type)
        {
            return true;
        }

        return false;
    }
}
