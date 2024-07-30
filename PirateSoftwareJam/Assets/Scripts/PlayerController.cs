using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [Header("Refrences")]
    [SerializeField] private Grid _gameGrid;
    [SerializeField] private VisualSetter _mainPotion;
    [SerializeField] private VisualSetter _backupPotion;
    [SerializeField] private SpriteRenderer _broom;
    [SerializeField] private Sprite _broomCharged;
    [SerializeField] private Sprite _broomDepleted;
    private PlayerAnimator _animator;
    private SoundPlayer _soundPlayer;

    [Header("Variables")]
    [SerializeField] private GameObject _bullet;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private int _catalystCD;
    private Vector3Int _gridPos;
    private Flower.Energy _nextEnergy;
    private Flower.Energy _backupEnergy;
    private bool _shotLoaded;
    private float _moveCD;
    [SerializeField] int _xBoundRight;
    [SerializeField] int _xBoundLeft;
    #endregion

    //============== Setup ==============
    #region Setup
    private void Start()
    {
        _animator = this.GetComponent<PlayerAnimator>();
        _soundPlayer = this.GetComponent<SoundPlayer>();

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

        _xBoundRight = (0 - GameGrid.Instance.GetGridScaleX() / 2);
        _xBoundLeft = (0 + GameGrid.Instance.GetGridScaleX() / 2);
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
        if (GameGrid.Instance.IsGameOver())
            return;

        if (Input.GetKeyDown(KeyCode.S) && _shotLoaded)
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
        Vector3Int cellPos = _gameGrid.WorldToCell(transform.position);
        if (cellPos.x <= _xBoundRight && dir < 0)
            return;
        if (cellPos.x >= _xBoundLeft && dir > 0)
            return;

        _gridPos.x += dir;
        transform.position = _gameGrid.GetCellCenterWorld(_gridPos);
        _moveCD = 0.2f;

        _animator.Move(dir);
    }

    private void GameTick()
    {
        _shotLoaded = true;
        _broom.sprite = _broomCharged;
    }

    private void SpawnBullet()
    {
        _shotLoaded = false;
        _broom.sprite = _broomDepleted;

        bool isCatalyst = false;
        if (_nextEnergy == Flower.Energy.Orange || _nextEnergy == Flower.Energy.Green || _nextEnergy == Flower.Energy.Purple)
            isCatalyst = true;

        Bullet bullet = Instantiate(_bullet, transform.position, transform.rotation).GetComponent<Bullet>();
        bullet.Setup(Vector3.down, _bulletSpeed, _nextEnergy, isCatalyst);

        ColorSprites(bullet.GetComponent<VisualSetter>(), _nextEnergy);

        GetNewLiquid();
    }

    private void SwapPotions()
    {
        _soundPlayer.PlayRandom(true);

        Flower.Energy temp = _nextEnergy;
        _nextEnergy = _backupEnergy;
        _backupEnergy = temp;

        ColorSprites(_mainPotion, _nextEnergy);
        ColorSprites(_backupPotion, _backupEnergy);
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

        ColorSprites(_mainPotion, _nextEnergy);
        ColorSprites(_backupPotion, _backupEnergy);
    }

    private void ColorSprites(VisualSetter vs, Flower.Energy nrg)
    {
        bool isCatalyst = false;

        if (nrg == Flower.Energy.Red)
            isCatalyst = false;
        else if (nrg == Flower.Energy.Yellow)
            isCatalyst = false;
        else if (nrg == Flower.Energy.Blue)
            isCatalyst = false;
        else if (nrg == Flower.Energy.Green)
            isCatalyst = true;
        else if (nrg == Flower.Energy.Orange)
            isCatalyst = true;
        else if (nrg == Flower.Energy.Purple)
            isCatalyst = true;

        vs.Setup(nrg, isCatalyst);
    }
    #endregion
}
