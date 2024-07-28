using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/*
 * CODE CREDIT TO:
 * 'Memory Leak' on Youtube
 * https://www.youtube.com/watch?v=69sBjqMtZCc
 */

public class WaterSpring : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R / V
    private int _waveIndex = 0;
    private static SpriteShapeController _spriteShapeController = null;
    [System.NonSerialized] public float Velocity = 0;
    [System.NonSerialized] public float Height = 0f;
    private float _force = 0;
    private float _target_height = 0f;
    private float _impact = 0.1f;
    #endregion

    //============== Setup ==============
    #region Setup
    public void Setup(SpriteShapeController ssc)
    {
        var index = transform.GetSiblingIndex();
        _waveIndex = index + 1;
        _spriteShapeController = ssc;

        Velocity = 0;
        Height = transform.localPosition.y;
        _target_height = transform.localPosition.y;
    }
    #endregion

    //============== Function ==============
    #region Function
    public void WaveSpringUpdate(float springStiffness, float dampening)
    {
        Height = transform.localPosition.y;

        var x = Height - _target_height;
        var loss = -dampening * Velocity;

        _force = -springStiffness * x + loss;
        Velocity += _force;
        var y = transform.localPosition.y;
        transform.localPosition = new Vector3(transform.localPosition.x, y + Velocity, transform.localPosition.z);
    }

    public void WavePointUpdate()
    {
        if (_spriteShapeController == null)
            return;

        Spline waterSpline = _spriteShapeController.spline;
        Vector3 wavePosition = waterSpline.GetPosition(_waveIndex);
        waterSpline.SetPosition(_waveIndex, new Vector3(wavePosition.x, transform.localPosition.y, wavePosition.z));
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Bullet"))
        {
            Velocity += _impact;
        }
    }
    #endregion
}
