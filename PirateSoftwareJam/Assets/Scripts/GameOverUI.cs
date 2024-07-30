using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    //============== Refrences ==============
    [SerializeField] private RectTransform _area;
    [SerializeField] private RectTransform _scroll;
    [SerializeField] private CanvasGroup _content;
    [SerializeField] private TextMeshProUGUI _score;
    [SerializeField] private TextMeshProUGUI _highScore;
    [SerializeField] private GameObject _newHighScore;

    //============== Function ==============
    public void ShowGameOverUI(int score)
    {
        gameObject.SetActive(true);
        AnimateOpen();

        int highScore = PlayerPrefs.GetInt("HighScore");

        if(score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            _newHighScore.SetActive(true);
        }

        _score.text = score.ToString();
        _highScore.text = highScore.ToString();
    }

    private void AnimateOpen()
    {
        Vector2 newScale = new Vector2(1920, _area.sizeDelta.y);
        _scroll.DOSizeDelta(newScale, 0.4f).SetEase(Ease.InOutSine).OnComplete( () => {
            _content.DOFade(1, 0.2f).SetEase(Ease.InSine);
        });
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.R))
            RestartGame();
    }

    public void RestartGame()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }
}
