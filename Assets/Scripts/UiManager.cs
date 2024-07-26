using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menu Screen Reference: ")]
    [SerializeField] private GameObject _menuScreen;

    [SerializeField] private TextMeshProUGUI _gameTitleText;
    [SerializeField] private TextMeshProUGUI _tapToPlay;

    [Header("Game Over Reference: ")]
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private TextMeshProUGUI _gameOverScoreText;

    [Header("Game Panel Reference: ")]
    [SerializeField] private TextMeshProUGUI _scoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _tapToPlay.transform.DOPunchScale(Vector3.one * 0.1f, 1f, 0, 0).SetLoops(-1);
    }

    public void HandleMenuScreen(bool activeState)
    {
        if (!activeState)
        {
            _tapToPlay.gameObject.SetActive(activeState);

            _gameTitleText.DOFade(0f, 2f).SetEase(Ease.OutSine).OnComplete(()=>
            {
                _menuScreen.SetActive(activeState);
            });
        }
        else
        {
            _menuScreen.SetActive(activeState);

            _gameTitleText.DOFade(1f, 0.25f).SetEase(Ease.OutQuart);

            _tapToPlay.gameObject.SetActive(activeState);
        }
    }

    public void HandleGameOverScreen(bool activeState)
    {
        _gameOverScoreText.text = _scoreText.text;

        _gameOverScreen.SetActive(activeState);
    }

    public void Home()
    {
        GameManager.Instance.OnRetry();

        HandleGameOverScreen(GameManager.Instance.IsGameOver);

        HandleMenuScreen(true);
    }

    public void UpdatePlayerScore(long score)
    {
        _scoreText.text = $"{score} m";
    }
}
