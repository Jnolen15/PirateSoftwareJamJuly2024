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
    [SerializeField] private GameObject _catalyst;
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;
    [SerializeField] private int _minComboSize;

    private Dictionary<Vector2Int, FlowerEntry> _flowerDict = new();

    [SerializeField] private float _curGameTick;
    [SerializeField] private bool _inGameTick;
    [SerializeField] private int _purpleScore;
    [SerializeField] private int _greenScore;
    [SerializeField] private int _orangeScore;

    [SerializeField] private GameObject _checker;

    public delegate void OnGameTick();
    public static event OnGameTick GameTick;

    public class FlowerEntry
    {
        public Flower Flower;
        public Vector2Int GridPos;
        public int ComboID;
        public bool CheckedForCombo;

        public FlowerEntry(Flower flower, Vector2Int gridPos, int comboID)
        {
            Flower = flower;
            GridPos = gridPos;
            ComboID = comboID;
            CheckedForCombo = false;
        }
    }
    #endregion

    //============== Setup ==============
    #region Setup
    void Start()
    {
        // Tile floor
        for (int x = 0; x <= _gridSizeX; x++)
        {
            for (int y = 0; y <= _gridSizeY; y++)
            {
                MakeNewTile(_gridTile, x - _gridSizeX / 2, y - _gridSizeY / 2);
            }
        }

        //// Right wall
        //for (int y = 0; y < _gridSizeY; y++)
        //{
        //    MakeNewTile(_gridWall, _gridSizeX / 2, y - _gridSizeY / 2);
        //}

        //// Left wall
        //for (int y = 0; y < _gridSizeY; y++)
        //{
        //    MakeNewTile(_gridWall, -1 - _gridSizeX / 2, y - _gridSizeY / 2);
        //}

        // bottom wall
        for (int x = 0; x <= _gridSizeX; x++)
        {
            MakeNewTile(_gridWall, x - _gridSizeX / 2, _gridSizeX / 2 - _gridSizeX - 1);
        }

        // Flowers
        for (int x = 0; x <= _gridSizeX; x++)
        {
            for (int y = 0; y <= _gridSizeY / 2; y++)
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

    private void MakeNewFlower(int x, int y, Flower.Energy energy = Flower.Energy.White)
    {
        Flower flower = Instantiate(_flower).GetComponent<Flower>();
        flower.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));

        Vector2Int tilePos = new Vector2Int(x, y);
        _flowerDict.Add(tilePos, new FlowerEntry(flower, tilePos, - 1));

        if (energy == Flower.Energy.White)
        {
            // Assign color
            Flower.Energy nrg = Flower.Energy.White;
            int rand = Random.Range(1, 4);
            if (rand == 1)
                nrg = Flower.Energy.Red;
            else if (rand == 2)
                nrg = Flower.Energy.Yellow;
            else if (rand == 3)
                nrg = Flower.Energy.Blue;

            flower.Setup(tilePos, this, nrg);
        }
        else
        {
            flower.Setup(tilePos, this, energy);
        }
    }
    
    private void MakeNewCatalyst(int x, int y, Flower.Energy energy)
    {
        Catalyst catalyst = Instantiate(_catalyst).GetComponent<Catalyst>();
        catalyst.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));

        Vector2Int tilePos = new Vector2Int(x, y);
        catalyst.Setup(tilePos, this, energy);
    }
    #endregion

    //============== Function ==============
    #region Function
    public void DoGameTick()
    {
        if (_inGameTick)
            return;

        StartCoroutine(GameTickCoroutine());
    }

    private IEnumerator GameTickCoroutine()
    {
        _inGameTick = true;

        // Reset combo groups
        foreach (FlowerEntry entry in _flowerDict.Values)
        {
            entry.ComboID = -1;
            entry.CheckedForCombo = false;
        }

        yield return new WaitForSeconds(0.4f);

        // Search and score combos
        SearchForCombos();

        yield return new WaitForSeconds(0.1f);

        //StartCoroutine(MoveDown());
        MoveDown();

        yield return new WaitForSeconds(0.1f);

        // Reset combo groups
        foreach (FlowerEntry entry in _flowerDict.Values)
        {
            entry.ComboID = -1;
            entry.CheckedForCombo = false;
        }

        SearchForCombos();

        GameTick?.Invoke();

        _inGameTick = false;
    }

    private void SearchForCombos()
    {
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector2Int checkingPos = new Vector2Int(x - _gridSizeX / 2, y - _gridSizeY / 2);
                if (_flowerDict.ContainsKey(checkingPos))
                {
                    if (_flowerDict[checkingPos] != null)
                    {
                        if (!_flowerDict[checkingPos].CheckedForCombo)
                        {
                            // If flower there, check if it makes a combo
                            int comboID = Random.Range(0, 100000);
                            List<FlowerEntry> flowerList = new();
                            Flower flower = _flowerDict[checkingPos].Flower;
                            flowerList.Add(_flowerDict[checkingPos]);
                            _flowerDict[checkingPos].ComboID = comboID;
                            _flowerDict[checkingPos].CheckedForCombo = true;
                            flower.SetComboID(comboID);
                            CheckFlowerCombo(flowerList, flower, comboID);

                            //Debug.Log($"{flower.GetEnergy()} Flower combo list was {flowerList.Count}");
                            if (flowerList.Count >= _minComboSize && (flower.GetEnergy() == Flower.Energy.Green || flower.GetEnergy() == Flower.Energy.Orange || flower.GetEnergy() == Flower.Energy.Purple))
                                ScoreCombo(flowerList);
                        }
                    }
                }
            }
        }
    }

    private void CheckFlowerCombo(List<FlowerEntry> flowerList, Flower prevFlower, int comboID)
    {
        Vector2Int pos = prevFlower.GetGridPosition();
        CheckFlowerNeighbors(flowerList, prevFlower, new Vector2Int(pos.x + 1, pos.y), comboID);
        CheckFlowerNeighbors(flowerList, prevFlower, new Vector2Int(pos.x - 1, pos.y), comboID);
        CheckFlowerNeighbors(flowerList, prevFlower, new Vector2Int(pos.x, pos.y + 1), comboID);
        CheckFlowerNeighbors(flowerList, prevFlower, new Vector2Int(pos.x, pos.y - 1), comboID);
    }

    private void CheckFlowerNeighbors(List<FlowerEntry> flowerList, Flower prevFlower, Vector2Int flowerPos, int comboID)
    {
        if (!_flowerDict.ContainsKey(flowerPos))
            return;

        if (_flowerDict[flowerPos] == null)
            return;

        // If flower there, check if it matches
        FlowerEntry flowerEntry = _flowerDict[flowerPos];
        if (prevFlower.GetEnergy() == flowerEntry.Flower.GetEnergy())
        {
            if (!flowerList.Contains(flowerEntry))
            {
                flowerList.Add(flowerEntry);
                flowerEntry.ComboID = comboID;
                flowerEntry.CheckedForCombo = true;
                flowerEntry.Flower.SetComboID(comboID);
                CheckFlowerCombo(flowerList, flowerEntry.Flower, comboID);
            }
        }
    }

    private void ScoreCombo(List<FlowerEntry> flowerList)
    {
        foreach (FlowerEntry fe in flowerList)
        {
            if (_flowerDict.ContainsKey(fe.GridPos))
            {
                if (fe.Flower.GetEnergy() == Flower.Energy.Green)
                    _greenScore++;
                else if (fe.Flower.GetEnergy() == Flower.Energy.Purple)
                    _purpleScore++;
                else if (fe.Flower.GetEnergy() == Flower.Energy.Orange)
                    _orangeScore++;

                _flowerDict.Remove(fe.GridPos);
                Destroy(fe.Flower.gameObject);
            }
            else
            {
                Debug.Log("Flower can not be scored! not found in dictionary");
            }
        }
    }

    public void MakeFlower(Vector3 bulletPos, Flower.Energy Catalyst)
    {
        Vector3Int pos = _gameGrid.WorldToCell(bulletPos);
        MakeNewFlower(pos.x, pos.y, Catalyst);
    }

    public void MakeCatalyst(Vector3 bulletPos, Flower.Energy Catalyst)
    {
        Vector3Int pos = _gameGrid.WorldToCell(bulletPos);
        MakeNewCatalyst(pos.x, pos.y, Catalyst);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x + 1, pos.y), Catalyst);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x - 1, pos.y), Catalyst);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x, pos.y + 1), Catalyst);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x, pos.y - 1), Catalyst);
    }

    public void ColorFlower(Vector2Int flowerPos, Flower.Energy Catalyst)
    {
        //Debug.Log($"{Catalyst} Potion burst at tile {flowerPos}");

        if (!_flowerDict.ContainsKey(flowerPos))
        {
            Debug.Log($"Tile {flowerPos} not found!");
            return;
        }

        //Debug.Log($"Tile {flowerPos} found, painting...");
        _flowerDict[flowerPos].Flower.Paint(Catalyst);
    }

    //private IEnumerator MoveDown()
    private void MoveDown()
    {
        // Cycle through from bottom up, dropping flowers if can
        for (int x = 0; x <= _gridSizeX; x++)
        {
            bool dropped = false;
            for (int y = _gridSizeY; y >= 1; y--)
            {

                Vector2Int checkingPos = new Vector2Int(x - _gridSizeX / 2, y - _gridSizeY / 2);
                //yield return new WaitForSeconds(0.1f);

                _checker.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(checkingPos.x, checkingPos.y, 0));

                Debug.Log($"Checking to drop at {checkingPos.x}, {checkingPos.y}");
                if (_flowerDict.ContainsKey(checkingPos))
                {
                    FlowerEntry flowerEntry = _flowerDict[checkingPos];
                    if (flowerEntry != null)
                    {
                        Vector2Int lowerPos = new Vector2Int(flowerEntry.GridPos.x, flowerEntry.GridPos.y - 1);

                        // Move flower
                        if (!GetIsFlowerAt(lowerPos))
                        {
                            dropped = true;

                            Debug.Log($"Dropping flower {checkingPos.x}, {checkingPos.y} to {lowerPos.x}, {lowerPos.y}");

                            flowerEntry.GridPos = lowerPos;
                            _flowerDict.Remove(checkingPos);
                            _flowerDict.Add(flowerEntry.GridPos, flowerEntry);

                            flowerEntry.Flower.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(lowerPos.x, lowerPos.y, 0));

                            flowerEntry.Flower.SetGridPos(flowerEntry.GridPos);
                        }
                    }
                }
            }
            if (dropped) // If something dropped, go over again
                x--;
        }
    }
    #endregion

    //============== Helpers ==============
    #region Helpers
    public Vector3 GetGridSize()
    {
        return _gameGrid.cellSize;
    }

    public bool GetIsFlowerAt(Vector2Int pos)
    {
        if (_flowerDict.ContainsKey(pos))
            return true;
        else
            return false;
    }

    public Flower.Energy GetFlowerEnergyAt(Vector2Int pos)
    {
        if (_flowerDict.ContainsKey(pos))
        {
            return _flowerDict[pos].Flower.GetEnergy();
        }
        else
        {
            Debug.Log($"Tile {pos} not found!");
            return Flower.Energy.White;
        }
    }

    public bool CheckMatchingTile(Vector2Int curPos, Flower.Energy liquid)
    {
        if (_flowerDict.ContainsKey(curPos))
            return (_flowerDict[curPos].Flower.GetEnergy() == liquid);
        else
            return false;
    }

    public void ClearTile(Vector2Int curPos)
    {
        _flowerDict[curPos].Flower.SetEnergy(Flower.Energy.White);
    }
    #endregion
}
