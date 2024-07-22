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

    [Header("Test Variables: ")]
    public bool isRunningOnASimulator = false; 

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
    }
}
