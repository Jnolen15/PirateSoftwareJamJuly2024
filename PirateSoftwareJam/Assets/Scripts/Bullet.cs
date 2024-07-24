using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private float _liveTime;
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;

    [SerializeField] private Flower.Energy _energy;
    #endregion

    //============== Function ==============
    #region Function
    public void Setup(Vector2 bulletDir, float _bulletSpeed, Flower.Energy nrg)
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _sr = this.GetComponent<SpriteRenderer>();

        _rb.AddForce(bulletDir * _bulletSpeed, ForceMode2D.Impulse);

        _energy = nrg;
        if (_energy == Flower.Energy.Red)
            _sr.color = Color.red;
        else if (_energy == Flower.Energy.Yellow)
            _sr.color = Color.yellow;
        else if (_energy == Flower.Energy.Blue)
            _sr.color = Color.blue;

        Destroy(gameObject, _liveTime);
    }

    private void OnDestroy()
    {
        GameGrid.Instance.DoGameTick();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Flower"))
            return;

        Vector2Int hitPos = col.gameObject.GetComponent<Flower>().GetGridPosition();
        GameGrid.Instance.ColorFlower(hitPos, _energy);

        Destroy(gameObject);
    }
    #endregion
}
