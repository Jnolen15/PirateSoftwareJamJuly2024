using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/*
 * CODE CREDIT TO:
 * 'Memory Leak' on Youtube
 * https://www.youtube.com/watch?v=69sBjqMtZCc
 */

public class WaterShapeController : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private GameObject _wavePointPref;
    private int _corsnersCount = 2;
    [SerializeField] private SpriteShapeController _spriteShapeController;
    [SerializeField] private int _wavesCount = 10;
    [SerializeField] private GameObject _wavePoints;
    // How much to spread to the other springs
    [Range(1, 7)]
    [SerializeField] private float _spread = 5f;
    // Slowing the movement over time
    [SerializeField] private float _dampening = 3f;
    // How stiff should our spring be constnat
    [SerializeField] private float _springStiffness = 0.1f;
    [SerializeField] private List<WaterSpring> _springs = new();
    #endregion

    //============== Setup ==============
    #region Setup
    private void Start()
    {
        SetWaves();
    }

    private void SetWaves()
    {
        Spline waterSpline = _spriteShapeController.spline;
        int waterPointsCount = waterSpline.GetPointCount();

        // Remove middle points for the waves, Keep only the corners
        for (int i = _corsnersCount; i < waterPointsCount - _corsnersCount; i++)
            waterSpline.RemovePointAt(_corsnersCount);

        Vector3 waterTopLeftCorner = waterSpline.GetPosition(1);
        Vector3 waterTopRightCorner = waterSpline.GetPosition(2);
        float waterWidth = waterTopRightCorner.x - waterTopLeftCorner.x;

        float spacingPerWave = waterWidth / (_wavesCount + 1);
        // Set new points for the waves
        for (int i = _wavesCount; i > 0; i--)
        {
            int index = _corsnersCount;

            float xPosition = waterTopLeftCorner.x + (spacingPerWave * i);
            Vector3 wavePoint = new Vector3(xPosition, waterTopLeftCorner.y, waterTopLeftCorner.z);
            waterSpline.InsertPointAt(index, wavePoint);
            waterSpline.SetHeight(index, 0.1f);
            waterSpline.SetCorner(index, false);
            waterSpline.SetTangentMode(index, ShapeTangentMode.Continuous);

        }

        CreateSprings(waterSpline);
    }

    private void CreateSprings(Spline waterSpline)
    {
        _springs = new();

        for (int i = 0; i <= _wavesCount + 1; i++)
        {
            int index = i + 1;

            Smoothen(waterSpline, index);

            GameObject wavePoint = Instantiate(_wavePointPref, _wavePoints.transform, false);
            wavePoint.transform.localPosition = waterSpline.GetPosition(index);

            WaterSpring waterSpring = wavePoint.GetComponent<WaterSpring>();
            waterSpring.Setup(_spriteShapeController);
            _springs.Add(waterSpring);
        }
    }

    private void Smoothen(Spline waterSpline, int index)
    {
        Vector3 position = waterSpline.GetPosition(index);
        Vector3 positionPrev = position;
        Vector3 positionNext = position;
        if (index > 1)
        {
            positionPrev = waterSpline.GetPosition(index - 1);
        }
        if (index - 1 <= _wavesCount)
        {
            positionNext = waterSpline.GetPosition(index + 1);
        }

        Vector3 forward = gameObject.transform.forward;

        float scale = Mathf.Min((positionNext - position).magnitude, (positionPrev - position).magnitude) * 0.33f;

        Vector3 leftTangent = (positionPrev - position).normalized * scale;
        Vector3 rightTangent = (positionNext - position).normalized * scale;

        SplineUtility.CalculateTangents(position, positionPrev, positionNext, forward, scale, out rightTangent, out leftTangent);

        waterSpline.SetLeftTangent(index, leftTangent);
        waterSpline.SetRightTangent(index, rightTangent);
    }
    #endregion

    //============== Function ==============
    #region Function
    void FixedUpdate()
    {
        foreach (WaterSpring waterSpringComponent in _springs)
        {
            waterSpringComponent.WaveSpringUpdate(_springStiffness, _dampening/100);
            waterSpringComponent.WavePointUpdate();
        }

        UpdateSprings();
    }

    private void UpdateSprings()
    {
        int count = _springs.Count;
        float[] left_deltas = new float[count];
        float[] right_deltas = new float[count];

        for (int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                left_deltas[i] = (_spread / 1000) * (_springs[i].Height - _springs[i - 1].Height);
                _springs[i - 1].Velocity += left_deltas[i];
            }
            if (i < _springs.Count - 1)
            {
                right_deltas[i] = (_spread / 1000) * (_springs[i].Height - _springs[i + 1].Height);
                _springs[i + 1].Velocity += right_deltas[i];
            }
        }
    }
    #endregion
}
