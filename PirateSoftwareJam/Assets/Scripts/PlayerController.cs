using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [Header("Refrences")]
    [SerializeField] private Grid _gameGrid;
    [SerializeField] private SpriteRenderer _mainPotionSR;
    [SerializeField] private SpriteRenderer _backupPotionSR;
    [SerializeField] private SpriteRenderer _reticleSR;

    [Header("Variables")]
    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _catalystCD;
    [SerializeField] private Color _orange;
    [SerializeField] private Color _purple;
    [SerializeField] private Color _green;
    private Vector3Int _gridPos;
    private Flower.Energy _nextEnergy;
    private Flower.Energy _backupEnergy;
    private bool _shotLoaded;
    private float _moveCD;
    #endregion

    //============== Setup ==============
    #region Setup
    private void Start()
    {
        GameGrid.GameTick += GameTick;

        // Pick an initial backup color
        int rand = Random.Range(1, 4);
        if (rand == 1)
            _backupEnergy = Flower.Energy.Red;
        else if (rand == 2)
            _backupEnergy = Flower.Energy.Yellow;
        else if (rand == 3)
            _backupEnergy = Flower.Energy.Blue;

        _catalystCD = 3;
        GetNewLiquid();
        _shotLoaded = true;

        // Set pos
        Vector3Int gridSpace = _gameGrid.WorldToCell(transform.position);
        transform.position = _gameGrid.GetCellCenterWorld(gridSpace);
        _gridPos = gridSpace;
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
        if ((Input.GetMouseButtonDown(0)  || Input.GetKeyDown(KeyCode.S)) && _shotLoaded)
            SpawnBullet();

        if (Input.GetKeyDown(KeyCode.Space))
            SwapPotions();

        // tap movement
        if (Input.GetKeyDown(KeyCode.A))
            Move(-1);
        else if (Input.GetKeyDown(KeyCode.D))
            Move(1);

        // Hold movement
        if (_moveCD > 0)
        {
            _moveCD -= Time.deltaTime;
            return;
        }

        if (Input.GetKey(KeyCode.A))
            Move(-1);
        else if (Input.GetKey(KeyCode.D))
            Move(1);
    }

    private void Move(int dir)
    {
        _gridPos.x += dir;
        transform.position = _gameGrid.GetCellCenterWorld(_gridPos);
        _moveCD = 0.2f;
    }

    private void GameTick()
    {
        _shotLoaded = true;
        _reticleSR.color = Color.black;
    }

    private void SpawnBullet()
    {
        _shotLoaded = false;
        _reticleSR.color = Color.gray;

        bool isCatalyst = false;
        if (_nextEnergy == Flower.Energy.Orange || _nextEnergy == Flower.Energy.Green || _nextEnergy == Flower.Energy.Purple)
            isCatalyst = true;

        Bullet bullet = Instantiate(_bullet, transform.position, transform.rotation).GetComponent<Bullet>();
        bullet.Setup(Vector3.down, _bulletSpeed, _nextEnergy, isCatalyst);

        GetNewLiquid();
    }

    private void SwapPotions()
    {
        Flower.Energy temp = _nextEnergy;
        _nextEnergy = _backupEnergy;
        _backupEnergy = temp;

        ColorSprite(_nextEnergy, _mainPotionSR);
        ColorSprite(_backupEnergy, _backupPotionSR);
    }

    private void GetNewLiquid()
    {
        _nextEnergy = _backupEnergy;

        if(_catalystCD > 0)
        {
            _catalystCD--;

            int rand = Random.Range(1, 4);
            if (rand == 1)
                _backupEnergy = Flower.Energy.Red;
            else if (rand == 2)
                _backupEnergy = Flower.Energy.Yellow;
            else if (rand == 3)
                _backupEnergy = Flower.Energy.Blue;
        }
        else
        {
            _catalystCD = Random.Range(3, 6);

            int rand = Random.Range(1, 4);
            if (rand == 1)
                _backupEnergy = Flower.Energy.Green;
            else if (rand == 2)
                _backupEnergy = Flower.Energy.Orange;
            else if (rand == 3)
                _backupEnergy = Flower.Energy.Purple;
        }

        ColorSprite(_nextEnergy, _mainPotionSR);
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
        else if (nrg == Flower.Energy.Green)
            sr.color = _green;
        else if (nrg == Flower.Energy.Orange)
            sr.color = _orange;
        else if (nrg == Flower.Energy.Purple)
            sr.color = _purple;
    }
    #endregion
}
