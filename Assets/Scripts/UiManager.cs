using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DG.Tweening;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance;

    [Header("Menu Screen Reference: ")]
    [SerializeField] private GameObject _menuScreen;

    [SerializeField] private TextMeshProUGUI _gameTitleText;
    [SerializeField] private TextMeshProUGUI _tapToPlay;

    [Header("Game Over Reference: ")]
    [SerializeField] private GameObject _gameOverScreen;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _tapToPlay.transform.DOPunchScale(Vector3.one * 0.1f, 1f, 0, 0).SetLoops(-1);
    }

    public void HandleMenuScreen(bool activeState)
    {
        if (!activeState)
        {
            _tapToPlay.gameObject.SetActive(false);

            _gameTitleText.DOFade(0f, 2f).SetEase(Ease.OutSine).OnComplete(()=>
            {
                _menuScreen.SetActive(activeState);
            });
        }
    }

    public void HandleGameOverScreen(bool activeState)
    {
        _gameOverScreen.SetActive(activeState);
    }
}
