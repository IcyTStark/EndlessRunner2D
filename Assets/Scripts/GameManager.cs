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

    [Header("Class References: ")]

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private ProceduralTerrainGenerator _proceduralTerrainGenerator;
    [SerializeField] private FollowCamera _followCamera;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Starts Game
    /// </summary>
    public void StartGame()
    {
        _hasGameStarted = true;
        UIManager.Instance.HandleMenuScreen(!_hasGameStarted);
    }

    /// <summary>
    /// On Game Over Condition
    /// </summary>
    /// <param name="highScore"></param>
    public void TriggerGameOver(float highScore)
    {
        _isGameOver = true;

        HighScore = highScore;

        UIManager.Instance.HandleGameOverScreen(IsGameOver);
    }

    /// <summary>
    /// Reset Game Controller Values and Invokes Retry Callback to all class
    /// </summary>
    public void OnRetry()
    {
        _hasGameStarted = false;
        _isGameOver = false;

        _proceduralTerrainGenerator.OnRetry(() =>
        {
            StartCoroutine(RefreshFramesAndResetPlayer());
        }); 
    }

    /// <summary>
    /// Refreshes the frame as it deactivates and activates to build the terrain
    /// </summary>
    /// <returns></returns>
    private IEnumerator RefreshFramesAndResetPlayer()
    {
        _proceduralTerrainGenerator.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        _proceduralTerrainGenerator.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();

        _playerController.OnRetryClicked.Invoke();
        _followCamera.OnRetryClicked.Invoke();
    }
}
