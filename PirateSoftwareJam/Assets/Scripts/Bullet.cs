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

    [SerializeField] private Tile.Liquid _liquid;
    #endregion

    //============== Function ==============
    #region Function
    public void Setup(Vector2 bulletDir, float _bulletSpeed, Tile.Liquid liquid)
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _sr = this.GetComponent<SpriteRenderer>();

        _rb.AddForce(bulletDir * _bulletSpeed, ForceMode2D.Impulse);

        _liquid = liquid;
        if (_liquid == Tile.Liquid.Red)
            _sr.color = Color.red;
        else if (_liquid == Tile.Liquid.Yellow)
            _sr.color = Color.yellow;
        else if (_liquid == Tile.Liquid.Blue)
            _sr.color = Color.blue;

        Destroy(gameObject, _liveTime);
    }

    private void OnDestroy()
    {
        Vector3 gridSize = GameGrid.Instance.GetGridSize();
        Vector2 deathPos = transform.position;

        GameGrid.Instance.PaintGridSpace(deathPos, _liquid);
        GameGrid.Instance.PaintGridSpace(deathPos + new Vector2(gridSize.x, 0), _liquid);
        GameGrid.Instance.PaintGridSpace(deathPos + new Vector2(-gridSize.x, 0), _liquid);
        GameGrid.Instance.PaintGridSpace(deathPos + new Vector2(0, gridSize.y), _liquid);
        GameGrid.Instance.PaintGridSpace(deathPos + new Vector2(0, -gridSize.y), _liquid);

        GameGrid.Instance.DoGameTick();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Enemy"))
            return;

        Destroy(gameObject);
    }
    #endregion
}
