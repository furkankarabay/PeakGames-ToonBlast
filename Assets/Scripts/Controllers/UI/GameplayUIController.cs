using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{

    public TextMeshProUGUI remainingMoveText, goal1Text, goal2Text, levelText;
    public Image goal1Image, goal2Image;
    public Image goal1TickImage, goal2TickImage;

    public Animation goal1Anim, goal2Anim;

    public static GameplayUIController Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Initialize(LevelManager.Instance.currentLevel);
    }
    public void Initialize(Level level)
    {
        levelText.text = string.Format("Level {0}", level.levelID);
        remainingMoveText.text = level.maxMoves.ToString();
        goal1Image.sprite = TileManager.Instance.tileSprites[(int) level.goal1Type];
        goal2Image.sprite = TileManager.Instance.tileSprites[(int)level.goal2Type];
    }


    public void TickGoalSection(int goalType)
    {
        if(goalType == 1)
        {
            goal1Text.gameObject.SetActive(false);
            goal1TickImage.gameObject.SetActive(true);
        }
        else if(goalType == 2)
        {
            goal2Text.gameObject.SetActive(false);
            goal2TickImage.gameObject.SetActive(true);
        }
    }

    public void UpdateGoalTexts(int goal1Remaining, int goal2Remaining)
    {
        goal1Text.text = goal1Remaining > 0 ? goal1Remaining.ToString() : "0";
        goal2Text.text = goal2Remaining > 0 ? goal2Remaining.ToString() : "0";
    }

    public void UpdateRemainingMoveText(int remainingMoves)
    {
        remainingMoveText.text = remainingMoves.ToString();

    }

    public void PlayGoalCollectedAnimations(int goalType)
    {
        if(goalType == 1)
        {
            if(goal1Anim)
                goal1Anim.Play();
        }
        else if(goalType == 2)
        {
            if (goal2Anim)
                goal2Anim.Play();
        }
    }
    

    public Vector3 GetGoalPosition(TileType tileType)
    {
        if (tileType == GameManager.Instance.goal1Type)
        {
            return goal1Image.transform.position;
        }
        else if (tileType == GameManager.Instance.goal2Type)
        {
            return goal2Image.transform.position;
        }

        return Vector3.zero;
    }
}
