using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Menu Screen Reference: ")]
    [SerializeField] private GameObject _menuScreen;

    [Header("Game Over Reference: ")]
    [SerializeField] private GameObject _gameOverScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void HandleMenuScreen(bool activeState)
    {
        _menuScreen.SetActive(activeState);
    }

    public void HandleGameOverScreen(bool activeState)
    {
        _gameOverScreen.SetActive(activeState);
    }
}
