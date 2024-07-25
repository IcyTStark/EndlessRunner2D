using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Control Variables: ")]
    private bool _hasGameStarted = false; 
    public bool HasGameStarted => _hasGameStarted;

    private bool _isGameOver = false;
    public bool IsGameOver => _isGameOver;

    [Header("Test Variables: ")]
    public bool isRunningOnASimulator = false;

    public float HighScore = 0f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }

        Destroy(gameObject);    
    }

    public void StartGame()
    {
        _hasGameStarted = true;
        UiManager.Instance.HandleMenuScreen(!_hasGameStarted);
    }

    public void TriggerGameOver(float highScore)
    {
        Debug.Log("Game Over");
        _isGameOver = true;

        HighScore = highScore;

        UiManager.Instance.HandleGameOverScreen(IsGameOver);
    }
}
