using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private Grid _gameGrid;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _bulletSpeed;
    private SpriteRenderer _sr;
    private Tile.Liquid _nextLiquid;
    private bool _shotLoaded;
    #endregion

    //============== Setup ==============
    #region Setup
    private void Start()
    {
        GameGrid.GameTick += GameTick;

        _sr = this.GetComponent<SpriteRenderer>();

        GetNewLiquid();
        _shotLoaded = true;
    }

    private void OnDestroy()
    {
        GameGrid.GameTick -= GameTick;
    }
    #endregion

    //============== Function ==============
    #region Function
    void Update()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        if (Input.GetMouseButtonDown(0) && _shotLoaded)
        {
            SpawnBullet(dir);
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetNewLiquid();
            GameGrid.Instance.DoGameTick();
        }
    }

    private void GameTick()
    {
        _shotLoaded = true;
    }

    private void SpawnBullet(Vector3 dir)
    {
        _shotLoaded = false;

        Vector3 bulletDir = dir.normalized;
        Bullet bullet = Instantiate(_bullet, transform.position, transform.rotation).GetComponent<Bullet>();
        bullet.Setup(bulletDir, _bulletSpeed, _nextLiquid);

        GetNewLiquid();
    }

    private void GetNewLiquid()
    {
        int rand = Random.Range(1, 4);
        if (rand == 1)
        {
            _nextLiquid = Tile.Liquid.Red;
            _sr.color = Color.red;
        }
        else if (rand == 2)
        {
            _nextLiquid = Tile.Liquid.Yellow;
            _sr.color = Color.yellow;
        }
        else if (rand == 3)
        {
            _nextLiquid = Tile.Liquid.Blue;
            _sr.color = Color.blue;
        }
    }
    #endregion
}
