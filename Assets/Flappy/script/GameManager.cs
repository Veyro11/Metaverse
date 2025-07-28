using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager gameManager;
    UImgr uiManager;
    public UImgr UIManager
    {
        get { return uiManager; }
    }

    private void Awake()
    {
        gameManager = this;
        uiManager = FindObjectOfType<UImgr>();
    }

    private void Start()
    {
        uiManager.UpdateScore(0);
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
        uiManager.SetRestart();
    }

    public static GameManager Instance
    {
        get { return gameManager; }
    }

    private int currentScore = 0;

    public void EndGame()
    {
        SceneManager.LoadScene("2d_metaverse");
    }

    public void AddScore(int score)
    {
        currentScore += score;
        uiManager.UpdateScore(currentScore);
        Debug.Log("Score: " + currentScore);
    }

}