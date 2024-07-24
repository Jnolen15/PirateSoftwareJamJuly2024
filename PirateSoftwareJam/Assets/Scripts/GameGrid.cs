using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    //============== Singleton ==============
    #region Singleton
    public static GameGrid Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    #endregion

    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private Grid _gameGrid;
    [SerializeField] private GameObject _gridTile;
    [SerializeField] private GameObject _gridWall;
    [SerializeField] private GameObject _flower;
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;

    private Dictionary<Vector2Int, Flower> _flowerDict = new();

    [SerializeField] private float _gameTickInterval;
    [SerializeField] private float _curGameTick;

    [SerializeField] private int _numGameTick;

    public delegate void OnGameTick();
    public static event OnGameTick GameTick;
    public static event OnGameTick EnemyGameTick;
    #endregion

    //============== Setup ==============
    #region Setup
    void Start()
    {
        // Tile floor
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                MakeNewTile(_gridTile, x - _gridSizeX / 2, y - _gridSizeY / 2);
            }
        }

        // Right wall
        for (int y = 0; y < _gridSizeY; y++)
        {
            MakeNewTile(_gridWall, _gridSizeX / 2, y - _gridSizeY / 2);
        }

        // Left wall
        for (int y = 0; y < _gridSizeY; y++)
        {
            MakeNewTile(_gridWall, -1 - _gridSizeX / 2, y - _gridSizeY / 2);
        }

        // Flowers
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = _gridSizeY / 2; y < _gridSizeY; y++)
            {
                MakeNewFlower(x - _gridSizeX / 2, y - _gridSizeY / 2);
            }
        }
    }

    private void MakeNewTile(GameObject obj, int x, int y)
    {
        Tile tile = Instantiate(obj, transform).GetComponent<Tile>();
        tile.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));
        Vector2Int tilePos = new Vector2Int(x, y);
        tile.Setup(tilePos, this);
    }

    private void MakeNewFlower(int x, int y)
    {
        Flower flower = Instantiate(_flower).GetComponent<Flower>();
        flower.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));

        Vector2Int tilePos = new Vector2Int(x, y);
        _flowerDict.Add(tilePos, flower);
        flower.Setup(tilePos, this);
    }
    #endregion

    //============== Function ==============
    #region Function
    private void Update()
    {
        if (_curGameTick >= _gameTickInterval)
        {
            _curGameTick = 0;
            //DoGameTick();
        }
        else
            _curGameTick += Time.deltaTime;
    }

    public void DoGameTick()
    {
        GameTick?.Invoke();

        _numGameTick++;

        //// enemy tick every 3
        //if(_numGameTick % 3 == 0)
        //{
        //    EnemyGameTick?.Invoke();

        //    // Spawn enemy
        //    int rand = Random.Range(0, 3);
        //    if (rand < 2)
        //    {
        //        int num = Random.Range(2, 5);
        //        for (int i = 0; i < num; i++)
        //        {
        //            MakeNewFlower(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
        //        }
        //    }
        //}
    }

    public void ColorFlower(Vector2Int flowerPos, Flower.Energy Catalyst)
    {
        Debug.Log($"{Catalyst} Potion burst at tile {flowerPos}");

        if (!_flowerDict.ContainsKey(flowerPos))
        {
            Debug.Log($"Tile {flowerPos} not found!");
            return;
        }

        Debug.Log($"Tile {flowerPos} found, painting...");
        _flowerDict[flowerPos].Paint(Catalyst);
    }
    #endregion

    //============== Helpers ==============
    #region Helpers
    public Vector3 GetGridSize()
    {
        return _gameGrid.cellSize;
    }

    public Flower.Energy GetFlowerEnergyAt(Vector2Int pos)
    {
        if (_flowerDict.ContainsKey(pos))
        {
            return _flowerDict[pos].GetEnergy();
        }
        else
        {
            Debug.Log($"Tile {pos} not found!");
            return Flower.Energy.White;
        }
    }

    public Vector2Int MoveDown(Vector2Int curPos, Transform trans)
    {
        Vector2Int newPos = new Vector2Int(curPos.x, curPos.y -1);

        //Vector3Int transCell = _gameGrid.WorldToCell(trans.position);
        trans.position = _gameGrid.GetCellCenterWorld(new Vector3Int(newPos.x, newPos.y, 0));

        return newPos;
    }

    public bool CheckMatchingTile(Vector2Int curPos, Flower.Energy liquid)
    {
        if (_flowerDict.ContainsKey(curPos))
            return (_flowerDict[curPos].GetEnergy() == liquid);
        else
            return false;
    }

    public void ClearTile(Vector2Int curPos)
    {
        _flowerDict[curPos].SetEnergy(Flower.Energy.White);
    }
    #endregion
}
