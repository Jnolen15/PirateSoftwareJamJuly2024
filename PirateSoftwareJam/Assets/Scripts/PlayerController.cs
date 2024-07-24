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
    private Flower.Energy _nextEnergy;
    private Flower.Energy _backupEnergy;
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
            _backupEnergy = Flower.Energy.Red;
        else if (rand == 2)
            _backupEnergy = Flower.Energy.Yellow;
        else if (rand == 3)
            _backupEnergy = Flower.Energy.Blue;
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
        bullet.Setup(bulletDir, _bulletSpeed, _nextEnergy);

        GetNewLiquid();
    }

    private void SwapPotions()
    {
        Flower.Energy temp = _nextEnergy;
        _nextEnergy = _backupEnergy;
        _backupEnergy = temp;

        ColorSprite(_nextEnergy, _sr);
        ColorSprite(_backupEnergy, _backupPotionSR);
    }

    private void GetNewLiquid()
    {
        _nextEnergy = _backupEnergy;

        int rand = Random.Range(1, 4);
        if (rand == 1)
            _backupEnergy = Flower.Energy.Red;
        else if (rand == 2)
            _backupEnergy = Flower.Energy.Yellow;
        else if (rand == 3)
            _backupEnergy = Flower.Energy.Blue;

        ColorSprite(_nextEnergy, _sr);
        ColorSprite(_backupEnergy, _backupPotionSR);
    }

    private void ColorSprite(Flower.Energy nrg, SpriteRenderer sr)
    {
        if (nrg == Flower.Energy.Red)
            sr.color = Color.red;
        else if (nrg == Flower.Energy.Yellow)
            sr.color = Color.yellow;
        else if (nrg == Flower.Energy.Blue)
            sr.color = Color.blue;
    }
    #endregion
}
