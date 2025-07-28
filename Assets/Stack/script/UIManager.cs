using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public enum UIState
{
    Home,
    Game,
    Score,
}

public class UIManager : MonoBehaviour
{
    static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    UIState currentState = UIState.Home;

    HomeUI homeUI = null;

    GameUI gameUI = null;

    ScoreUI scoreUI = null;

    block TheStack = null;
    private void Awake()
    {
        instance = this;
        TheStack = FindObjectOfType<block>();

        homeUI = GetComponentInChildren<HomeUI>(true);
        homeUI?.Init(this);
        gameUI = GetComponentInChildren<GameUI>(true);
        gameUI?.Init(this);
        scoreUI = GetComponentInChildren<ScoreUI>(true);
        scoreUI?.Init(this);

        ChangeState(UIState.Home);
    }


    public void ChangeState(UIState state)
    {
        currentState = state;
        homeUI?.SetActive(currentState);
        gameUI?.SetActive(currentState);
        scoreUI?.SetActive(currentState);
    }

    public void OnClickStart()
    {
        TheStack.Restart();
        ChangeState(UIState.Game);
    }


    public void SetScoreUI()
    {
        scoreUI.SetUI(TheStack.Score, TheStack.MaxCombo, TheStack.BestScore, TheStack.BestCombo);

        ChangeState(UIState.Score);
    }
    public void UpdateScore()
    {
        gameUI.SetUI(TheStack.Score, TheStack.Combo, TheStack.MaxCombo);
    }

}
