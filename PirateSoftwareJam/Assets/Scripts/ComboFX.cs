using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ComboFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fx;
    [SerializeField] private TextMeshProUGUI _points;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] float _animTime;

    public void Setup(int pointVal, Color color)
    {
        ParticleSystem.MainModule ma2 = _fx.main;
        color.a = 0.2f;
        ma2.startColor = color;
        _fx.Emit(Random.Range(3, 5));

        _points.text = pointVal + "X";
        _canvasGroup.alpha = 1;

        _points.transform.DOPunchScale(new Vector3(0.1f, 0.1f), _animTime);
        _points.transform.DORotateQuaternion(Quaternion.Euler(0, 0, Random.Range(-15, 15)), _animTime);
        _canvasGroup.DOFade(0.8f, _animTime).SetEase(Ease.InSine);
    }

    public void Hide()
    {
        _canvasGroup.DOFade(0, 1).SetEase(Ease.InSine);
    }
}
