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
    [SerializeField] private GameObject _enemy;
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;

    private Dictionary<Vector2Int, Tile> _tileDict = new();

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
                MakeNewTile(x - _gridSizeX / 2, y - _gridSizeY / 2);
            }
        }

        // Right wall
        for (int y = 0; y < _gridSizeY; y++)
        {
            MakeNewWall(_gridSizeX / 2, y - _gridSizeY / 2);
        }

        // Left wall
        for (int y = 0; y < _gridSizeY; y++)
        {
            MakeNewWall(-1 - _gridSizeX / 2, y - _gridSizeY / 2);
        }

        // Enemy
        MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
        MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
        MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
        MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
        MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
        MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
    }

    private void MakeNewTile(int x, int y)
    {
        Tile tile = Instantiate(_gridTile).GetComponent<Tile>();
        tile.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));

        Vector2Int tilePos = new Vector2Int(x, y);
        _tileDict.Add(tilePos, tile);
        tile.Setup(tilePos, this);
    }

    private void MakeNewWall(int x, int y)
    {
        Wall wall = Instantiate(_gridWall).GetComponent<Wall>();
        wall.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));
        Vector2Int tilePos = new Vector2Int(x, y);
        wall.Setup(tilePos, this);
    }

    private void MakeNewEnemy(int x, int y)
    {
        Enemy enemy = Instantiate(_enemy).GetComponent<Enemy>();
        enemy.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));
        Vector2Int enemyPos = new Vector2Int(x, y);
        enemy.Setup(enemyPos, this);
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

        // enemy tick every 3
        if(_numGameTick % 3 == 0)
        {
            EnemyGameTick?.Invoke();

            // Spawn enemy
            int rand = Random.Range(0, 3);
            if (rand < 2)
            {
                int num = Random.Range(2, 5);
                for (int i = 0; i < num; i++)
                {
                    MakeNewEnemy(Random.Range(0 - _gridSizeX / 2, _gridSizeX / 2), _gridSizeY / 2 - 1);
                }
            }
        }
    }

    public void PaintGridSpace(Vector2 worldPos, Tile.Liquid liquid)
    {
        Vector3Int cellPosV3 = _gameGrid.WorldToCell(worldPos);
        Vector2Int cellPosV2 = new Vector2Int(cellPosV3.x, cellPosV3.y);

        Debug.Log($"{liquid} Potion burst at world pos {worldPos}, tile {cellPosV2}");

        if (_tileDict.ContainsKey(cellPosV2))
        {
            Debug.Log($"Tile {cellPosV2} found, painting...");
            _tileDict[cellPosV2].Paint(liquid);
        }
        else
            Debug.Log($"Tile {cellPosV2} not found!");
    }
    #endregion

    //============== Helpers ==============
    #region Helpers
    public Vector3 GetGridSize()
    {
        return _gameGrid.cellSize;
    }

    public Vector2Int MoveDown(Vector2Int curPos, Transform trans)
    {
        Vector2Int newPos = new Vector2Int(curPos.x, curPos.y -1);

        //Vector3Int transCell = _gameGrid.WorldToCell(trans.position);
        trans.position = _gameGrid.GetCellCenterWorld(new Vector3Int(newPos.x, newPos.y, 0));

        return newPos;
    }

    public bool CheckMatchingTile(Vector2Int curPos, Tile.Liquid liquid)
    {
        if (_tileDict.ContainsKey(curPos))
            return (_tileDict[curPos].GetLiquid() == liquid);
        else
            return false;
    }

    public void ClearTile(Vector2Int curPos)
    {
        _tileDict[curPos].SetLiquid(Tile.Liquid.White);
    }
    #endregion
}
