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
    [SerializeField] private SpriteRenderer _backupPotionSR;
    private SpriteRenderer _sr;
    private Tile.Liquid _nextLiquid;
    private Tile.Liquid _backupLiquid;
    private bool _shotLoaded;
    #endregion

    //============== Setup ==============
    #region Setup
    private void Start()
    {
        GameGrid.GameTick += GameTick;

        _sr = this.GetComponent<SpriteRenderer>();

        // Pick an initial backup color
        int rand = Random.Range(1, 4);
        if (rand == 1)
            _backupLiquid = Tile.Liquid.Red;
        else if (rand == 2)
            _backupLiquid = Tile.Liquid.Yellow;
        else if (rand == 3)
            _backupLiquid = Tile.Liquid.Blue;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwapPotions();
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

    private void SwapPotions()
    {
        Tile.Liquid temp = _nextLiquid;
        _nextLiquid = _backupLiquid;
        _backupLiquid = temp;

        ColorSprite(_nextLiquid, _sr);
        ColorSprite(_backupLiquid, _backupPotionSR);
    }

    private void GetNewLiquid()
    {
        _nextLiquid = _backupLiquid;

        int rand = Random.Range(1, 4);
        if (rand == 1)
            _backupLiquid = Tile.Liquid.Red;
        else if (rand == 2)
            _backupLiquid = Tile.Liquid.Yellow;
        else if (rand == 3)
            _backupLiquid = Tile.Liquid.Blue;

        ColorSprite(_nextLiquid, _sr);
        ColorSprite(_backupLiquid, _backupPotionSR);
    }

    private void ColorSprite(Tile.Liquid liquid, SpriteRenderer sr)
    {
        if (liquid == Tile.Liquid.Red)
            sr.color = Color.red;
        else if (liquid == Tile.Liquid.Yellow)
            sr.color = Color.yellow;
        else if (liquid == Tile.Liquid.Blue)
            sr.color = Color.blue;
    }
    #endregion
}
