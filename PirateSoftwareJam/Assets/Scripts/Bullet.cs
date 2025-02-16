using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private float _liveTime;
    private Rigidbody2D _rb;
    private Flower.Energy _energy;
    private bool _isCatalyst;
    private bool _hasHit;
    #endregion

    //============== Function ==============
    #region Function
    public void Setup(Vector2 bulletDir, float _bulletSpeed, Flower.Energy nrg, bool isCatalyst)
    {
        _rb = this.GetComponent<Rigidbody2D>();

        _rb.AddForce(bulletDir * _bulletSpeed, ForceMode2D.Impulse);

        _isCatalyst = isCatalyst;

        _energy = nrg;

        transform.localScale = GameGrid.Instance.GetGridCellSize();

        Destroy(gameObject, _liveTime);
    }

    private void OnDestroy()
    {
        GameGrid.Instance.DoGameTick();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Flower") && !col.gameObject.CompareTag("Wall"))
            return;

        if (_hasHit)
            return;
        else
            _hasHit = true;

        if (_isCatalyst)
            GameGrid.Instance.MakeCatalyst(transform.position, _energy);
        else
            GameGrid.Instance.MakeFlower(transform.position, _energy);

        if (col.gameObject.CompareTag("Flower"))
        {
            col.gameObject.GetComponent<Flower>().ShakeOrb();
        }

        Destroy(gameObject);
    }
    #endregion
}
