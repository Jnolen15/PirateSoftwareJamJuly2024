using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

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
    [Header("Refrences")]
    [SerializeField] private Grid _gameGrid;
    [SerializeField] private TextMeshProUGUI _totalScore;
    [SerializeField] private TextMeshProUGUI _greenBurst;
    [SerializeField] private TextMeshProUGUI _orangeBurst;
    [SerializeField] private TextMeshProUGUI _purpleBurst;
    [SerializeField] private TextMeshProUGUI _roundNum;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private TextMeshProUGUI _gameOverText;

    [Header("Prefabs")]
    [SerializeField] private GameObject _gridTile;
    [SerializeField] private GameObject _gridWall;
    [SerializeField] private GameObject _flower;
    [SerializeField] private GameObject _catalyst;

    [Header("Rules")]
    [SerializeField] private int _gridSizeX;
    [SerializeField] private int _gridSizeY;
    [SerializeField] private int _minComboSize;
    [SerializeField] private int _newRowSize;
    [SerializeField] private List<Vector2Int> _newRowInterval;

    private Dictionary<Vector2Int, FlowerEntry> _flowerDict = new();
    private bool _inGameTick;
    private int _round;
    private int _newRowCD;
    private float _score;
    private float _purpleScore;
    private float _greenScore;
    private float _orangeScore;
    private bool _gameIsOver;
    private bool _comboScored;

    private WaitForSeconds _colorPause;
    private WaitForSeconds _scorePause;
    private WaitForSeconds _dropPause;

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
        _colorPause = new WaitForSeconds(0.8f);
        _scorePause = new WaitForSeconds(0.05f);
        _dropPause = new WaitForSeconds(0.5f);

        _newRowCD = _newRowInterval[0].y;

        // Tile floor
        for (int x = 0; x <= _gridSizeX; x++)
        {
            for (int y = 0; y <= _gridSizeY; y++)
            {
                MakeNewTile(_gridTile, x - _gridSizeX / 2, y - _gridSizeY / 2);
            }
        }

        // Right wall
        for (int y = 0; y < _gridSizeY; y++)
            MakeNewTile(_gridWall, 1 + _gridSizeX / 2, y - _gridSizeY / 2);

        // Left wall
        for (int y = 0; y < _gridSizeY; y++)
            MakeNewTile(_gridWall, -1 - _gridSizeX / 2, y - _gridSizeY / 2);

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
    
    private Catalyst MakeNewCatalyst(int x, int y, Flower.Energy energy)
    {
        Catalyst catalyst = Instantiate(_catalyst).GetComponent<Catalyst>();
        catalyst.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(x, y, 0));
        Vector2Int tilePos = new Vector2Int(x, y);
        catalyst.Setup(tilePos, this, energy);

        return catalyst;
    }
    #endregion

    //============== UI ==============
    #region UI
    public void ShowGameEnd()
    {
        if (_gameIsOver)
            return;

        _gameIsOver = true;
        _gameOver.SetActive(true);
        _gameOverText.text = "Final Score: " + _score;
    }

    private void UpdateScoreUI()
    {
        _score = _greenScore + _orangeScore + _purpleScore;

        _totalScore.text = "Score: " + _score;
        _greenBurst.text = "Green Burst: " + _greenScore;
        _orangeBurst.text = "Orange Burst: " + _orangeScore;
        _purpleBurst.text = "Purple Burst: " + _purpleScore;
    }
    #endregion

    //============== Game Management ==============
    #region Game Management
    public void DoGameTick()
    {
        if (_inGameTick)
            return;

        StartCoroutine(GameTickCoroutine());
        IncreaseDifficulty();
    }

    private IEnumerator GameTickCoroutine()
    {
        _inGameTick = true;

        ResetComboGroups();

        yield return _colorPause; // Wait for color

        // Search and score combos
        yield return StartCoroutine(SearchForCombos());
        while (_comboScored)
        {
            yield return _dropPause;
            Debug.Log("Combo scored, dropping and checking");
            MoveDown();

            ResetComboGroups();

            yield return StartCoroutine(SearchForCombos());
        }
        
        // Make new rows
        if (_newRowCD <= 0)
        {
            _newRowCD = GetNewRowCDBasedOnDifficulty();
            MakeNewRows();
        }
        else
            _newRowCD--;

        GameTick?.Invoke();

        _inGameTick = false;
    }

    private void MakeNewRows()
    {
        Debug.Log("Making new rows");

        // Move all up X
        List<FlowerEntry> dictEntries = _flowerDict.Values.ToList();
        _flowerDict.Clear();
        foreach (FlowerEntry item in dictEntries)
        {
            Vector2Int newPos = new Vector2Int(item.GridPos.x, item.GridPos.y + _newRowSize);
            item.GridPos = newPos;
            item.Flower.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(newPos.x, newPos.y, 0));
            item.Flower.SetGridPos(item.GridPos);
            _flowerDict.Add(newPos, item);
        }

        // Spawn X new rows
        for (int x = 0; x <= _gridSizeX; x++)
        {
            for (int y = 0; y < _newRowSize; y++)
            {
                MakeNewFlower(x - _gridSizeX / 2, y - _gridSizeY / 2);
            }
        }
    }

    private void IncreaseDifficulty()
    {
        _round++;
        _roundNum.text = _round.ToString();
    }

    private int GetNewRowCDBasedOnDifficulty()
    {
        int newRowSize = _newRowInterval[0].y;
        foreach (Vector2Int item in _newRowInterval)
        {
            if (_round >= item.x)
                newRowSize = item.y;
        }
        return newRowSize;
    }
    #endregion

    //============== Objects ==============
    #region Objects
    public void MakeFlower(Vector3 bulletPos, Flower.Energy Catalyst)
    {
        Vector3Int pos = _gameGrid.WorldToCell(bulletPos);
        MakeNewFlower(pos.x, pos.y, Catalyst);
    }

    public void MakeCatalyst(Vector3 bulletPos, Flower.Energy catalystColor)
    {
        Vector3Int pos = _gameGrid.WorldToCell(bulletPos);
        Catalyst catalyst = MakeNewCatalyst(pos.x, pos.y, catalystColor);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x + 1, pos.y), catalystColor);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x - 1, pos.y), catalystColor);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x, pos.y + 1), catalystColor);
        GameGrid.Instance.ColorFlower(new Vector2Int(pos.x, pos.y - 1), catalystColor);
        catalyst.Break();
    }

    public void ColorFlower(Vector2Int flowerPos, Flower.Energy Catalyst)
    {
        if (!_flowerDict.ContainsKey(flowerPos))
        {
            Debug.Log($"Tile {flowerPos} not found!");
            return;
        }

        _flowerDict[flowerPos].Flower.Paint(Catalyst);
    }
    #endregion

    //============== Grid ==============
    #region Grid
    private IEnumerator SearchForCombos()
    {
        _comboScored = false;
        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector2Int checkingPos = new Vector2Int(x - _gridSizeX / 2, y - _gridSizeY / 2);
                if (_flowerDict.ContainsKey(checkingPos))
                {
                    if (_flowerDict[checkingPos] != null && !_flowerDict[checkingPos].CheckedForCombo)
                    {
                        // If flower there, check if it makes a combo
                        int comboID = Random.Range(0, 100000);
                        List<FlowerEntry> flowerList = new();
                        flowerList.Add(_flowerDict[checkingPos]);

                        _flowerDict[checkingPos].ComboID = comboID;
                        _flowerDict[checkingPos].CheckedForCombo = true;
                        Flower flower = _flowerDict[checkingPos].Flower;
                        flower.SetComboID(comboID);

                        CheckFlowerCombo(flowerList, flower, comboID);

                        if (flowerList.Count >= _minComboSize && (flower.GetEnergy() == Flower.Energy.Green || flower.GetEnergy() == Flower.Energy.Orange || flower.GetEnergy() == Flower.Energy.Purple))
                        {
                            _comboScored = true;
                            yield return ScoreCombo(flowerList);
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
        if (prevFlower.GetEnergy() == flowerEntry.Flower.GetEnergy() && !flowerList.Contains(flowerEntry))
        {
            flowerList.Add(flowerEntry);
            flowerEntry.ComboID = comboID;
            flowerEntry.CheckedForCombo = true;
            flowerEntry.Flower.SetComboID(comboID);
            CheckFlowerCombo(flowerList, flowerEntry.Flower, comboID);
        }
    }

    private IEnumerator ScoreCombo(List<FlowerEntry> flowerList)
    {
        int count = 1;
        foreach (FlowerEntry fe in flowerList)
        {
            if (_flowerDict.ContainsKey(fe.GridPos))
            {
                float value = 10 * (1 + (count * 0.1f));

                if (fe.Flower.GetEnergy() == Flower.Energy.Green)
                    _greenScore += value;
                else if (fe.Flower.GetEnergy() == Flower.Energy.Purple)
                    _purpleScore += value;
                else if (fe.Flower.GetEnergy() == Flower.Energy.Orange)
                    _orangeScore += value;

                _flowerDict.Remove(fe.GridPos);
                fe.Flower.Score(value);

                count++;

                yield return _scorePause;
            }
            else
                Debug.Log("Flower can not be scored! not found in dictionary");
        }

        UpdateScoreUI();
    }

    private void ResetComboGroups()
    {
        foreach (FlowerEntry entry in _flowerDict.Values)
        {
            entry.ComboID = -1;
            entry.CheckedForCombo = false;
        }
    }

    private void MoveDown()
    {
        // Cycle through from bottom up, dropping flowers if can
        for (int x = 0; x <= _gridSizeX; x++)
        {
            bool dropped = false;
            for (int y = _gridSizeY; y >= 1; y--)
            {
                Vector2Int checkingPos = new Vector2Int(x - _gridSizeX / 2, y - _gridSizeY / 2);
                if (_flowerDict.ContainsKey(checkingPos))
                {
                    FlowerEntry flowerEntry = _flowerDict[checkingPos];
                    Vector2Int lowerPos = new Vector2Int(flowerEntry.GridPos.x, flowerEntry.GridPos.y - 1);
                    if (flowerEntry != null && !GetIsFlowerAt(lowerPos))
                    {
                        dropped = true;
                        flowerEntry.GridPos = lowerPos;
                        _flowerDict.Remove(checkingPos);
                        _flowerDict.Add(flowerEntry.GridPos, flowerEntry);
                        flowerEntry.Flower.transform.position = _gameGrid.GetCellCenterWorld(new Vector3Int(lowerPos.x, lowerPos.y, 0));
                        flowerEntry.Flower.SetGridPos(flowerEntry.GridPos);
                    }
                }
            }

            if (dropped) // If something dropped, go over column again
                x--;
        }
    }
    #endregion

    //============== Helpers ==============
    #region Helpers
    public int GetGridScaleY()
    {
        return _gridSizeY;
    }

    public Vector3 GetGridCellSize()
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
            return _flowerDict[pos].Flower.GetEnergy();
        else
            return Flower.Energy.White;
    }
    #endregion
}
