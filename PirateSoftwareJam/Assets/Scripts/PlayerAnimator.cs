using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimator : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [Header("Refrences")]
    [SerializeField] private Transform _visuals;
    [SerializeField] private Transform _tail;
    [SerializeField] private SpriteRenderer _woosh;

    [Header("Variables")]
    [SerializeField] private float _hoverDist;
    [SerializeField] private float _hoverLoopTime;
    [SerializeField] private float _tailWag;
    [SerializeField] private float _tailLoopTime;

    #endregion

    //============== Animation ==============
    #region Animation
    void Start()
    {
        _visuals.DOLocalMoveY(_hoverDist, _hoverLoopTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        _tail.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, _tailWag), _tailLoopTime).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }


    public void Move(int dir)
    {
        _woosh.gameObject.SetActive(true);
        Color col = Color.white;
        col.a = 0.6f;
        _woosh.color = col;

        if (dir > 0)
        {
            _woosh.DOFade(0, 0.2f);
            _woosh.transform.localPosition = new Vector3(-0.6f, _woosh.transform.localPosition.y, 0);
            _woosh.transform.DOLocalMoveX(-1.2f, 0.2f).SetEase(Ease.OutSine).OnComplete(() => { _woosh.gameObject.SetActive(false); });
        }
        else if (dir < 0)
        {
            _woosh.DOFade(0, 0.2f);
            _woosh.transform.localPosition = new Vector3(0.6f, _woosh.transform.localPosition.y, 0);
            _woosh.transform.DOLocalMoveX(1.2f, 0.2f).SetEase(Ease.OutSine).OnComplete(() => { _woosh.gameObject.SetActive(false); });
        }
    }
    #endregion
}
