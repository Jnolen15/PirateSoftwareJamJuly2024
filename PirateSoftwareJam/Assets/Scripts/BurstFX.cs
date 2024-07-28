using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class BurstFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fx;
    [SerializeField] private ParticleSystem _fx2;
    [SerializeField] private TextMeshProUGUI _points;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] int _liveTime;

    public void Setup(int pointVal, Color color, Vector2Int numParticlesRange)
    {
        ParticleSystem.MainModule ma = _fx.main;
        color.a = 120;
        ma.startColor = color;
        _fx.Emit(Random.Range(numParticlesRange.x, numParticlesRange.y));

        ParticleSystem.MainModule ma2 = _fx2.main;
        color.a = 0.2f;
        ma2.startColor = color;
        _fx2.Emit(Random.Range(numParticlesRange.x, numParticlesRange.y));

        if (pointVal != 0)
        {
            _points.text = pointVal.ToString();
            _canvasGroup.DOFade(0, _liveTime).SetEase(Ease.InSine);
        }
        else
            _canvasGroup.alpha = 0;

        Destroy(gameObject, _liveTime);
    }

    private void OnDestroy()
    {
        _canvasGroup.DOKill();
    }
}
