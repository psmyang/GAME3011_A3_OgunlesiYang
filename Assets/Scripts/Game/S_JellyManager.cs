using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum E_Difficulty
{
    Easy,
    Medium,
    Hard
}

public class S_JellyManager : MonoBehaviour
{
    public E_Difficulty difficulty = E_Difficulty.Easy;
    public int Score = 0;
    public int Goal = 5000;
    public int TimeCounter = 250;
    public bool isGameOver = false;
    public bool isGameWin = false;

    [SerializeField] Text ModeText;
    [SerializeField] Text ScoreText;
    [SerializeField] Text TimeCounterText;
    [SerializeField] Text GoalText;

    private S_JellyTable board;

    void Start()
    {
        board = FindObjectOfType<S_JellyTable>();
        ScoreText.text = Score.ToString();

        //ScoreText.text = "Score: " + board.score;
        TimeCounterText.text = "Time: " + TimeCounter.ToString();
        GoalText.text = "Goal: " + Goal.ToString();
        StartCoroutine(CountTime());
    }

    IEnumerator CountTime()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(1.0f);
            TimeCounter--;
            if (TimeCounter <= 0)
            {
                TimeCounter = 0;
                isGameOver = true;
            }
            TimeCounterText.text = "Time: " + TimeCounter.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver)
        {
            SceneManager.LoadScene("Map_End");
        }

        if (isGameWin)
        {
            SceneManager.LoadScene("Map_Win");
        }
    }

    public void EasyMode()
    {
        difficulty = E_Difficulty.Easy;
        TimeCounter = 250;
        Goal = 5000;
        ModeText.text = "Mode: Easy";
        board.maxCandy = 3;
        board.Setup();
        ManaReset();

    }

    public void MediumMode()
    {
        difficulty = E_Difficulty.Medium;
        TimeCounter = 200;
        Goal = 10000;
        ModeText.text = "Mode: Medium";
        board.maxCandy = 4;
        board.Setup();
        ManaReset();

    }

    public void HardMode()
    {
        difficulty = E_Difficulty.Hard;
        TimeCounter = 250;
        Goal = 15000;
        ModeText.text = "Mode: Hard";
        board.maxCandy = 5;
        board.Setup();
        ManaReset();

    }



    public void AddScore(int score)
    {
        Score += score;
        ScoreText.text = Score.ToString();
        if (Score >= Goal)
        {
            isGameWin = true;
        }
    }

    void ManaReset()
    {
        isGameOver = false;
        isGameWin = false;

        Score = 0;
        ScoreText.text = Score.ToString();
        TimeCounterText.text = "Time: " + TimeCounter.ToString();
        GoalText.text = "Goal: " + Goal.ToString();
        board = FindObjectOfType<S_JellyTable>();

    }
}
