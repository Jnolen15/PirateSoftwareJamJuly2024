using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Flower : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    public enum Energy
    {
        White,
        Red,
        Yellow,
        Blue,
        Green,
        Purple,
        Orange,
        Brown
    }
    [SerializeField] private Energy _energy;

    [SerializeField] private Color _red;
    [SerializeField] private Color _yellow;
    [SerializeField] private Color _blue;
    [SerializeField] private Color _green;
    [SerializeField] private Color _purple;
    [SerializeField] private Color _orange;
    [SerializeField] private Color _brown;

    [SerializeField] private Vector2Int _gridPos;

    [SerializeField] private GameGrid _grid;
    [SerializeField] private SpriteRenderer _sr;

    [SerializeField] private TextMeshProUGUI _coords;

    private int _brownWiltTime;
    #endregion

    //============== Setup ==============
    #region Setup
    private void Start()
    {
        GameGrid.GameTick += GameTick;
    }

    private void OnDestroy()
    {
        GameGrid.GameTick -= GameTick;
    }

    public void Setup(Vector2Int gridPos, GameGrid grid)
    {
        _sr = this.GetComponent<SpriteRenderer>();

        _grid = grid;
        _gridPos = gridPos;

        transform.localScale = _grid.GetGridSize();

        // Assign color
        int rand = Random.Range(1, 4);
        if (rand == 1)
            _energy = Flower.Energy.Red;
        else if (rand == 2)
            _energy = Flower.Energy.Yellow;
        else if (rand == 3)
            _energy = Flower.Energy.Blue;

        ColorFlower();

        _coords.text = $"({_gridPos.x},{_gridPos.y})";
    }
    #endregion

    //============== Function ==============
    #region Function
    public Vector2Int GetGridPosition()
    {
        return _gridPos;
    }

    public Energy GetEnergy()
    {
        return _energy;
    }

    public void SetEnergy(Energy nrg)
    {
        _energy = nrg;
        ColorFlower();
    }

    private void GameTick()
    {
        if(_energy == Energy.Brown)
        {
            if(_brownWiltTime >= 1)
            {
                _brownWiltTime = 0;
                _energy = Energy.White;
                ColorFlower();
            }
            else
                _brownWiltTime++;
        }
    }

    public void Paint(Energy nrg)
    {
        Energy prevNrg = _energy;

        // Paint this flower
        string message = ($"Mixing {_energy} with {nrg}");
        switch (_energy)
        {
            case Energy.White:
                _energy = nrg;
                break;
            case Energy.Red:
                if (nrg == Energy.Red)
                    _energy = Energy.Red;
                else if (nrg == Energy.Yellow)
                    _energy = Energy.Orange;
                else if (nrg == Energy.Blue)
                    _energy = Energy.Purple;
                else
                    _energy = Energy.Brown;
                break;
            case Energy.Yellow:
                if (nrg == Energy.Red)
                    _energy = Energy.Orange;
                else if (nrg == Energy.Yellow)
                    _energy = Energy.Yellow;
                else if (nrg == Energy.Blue)
                    _energy = Energy.Green;
                else
                    _energy = Energy.Brown;
                break;
            case Energy.Blue:
                if (nrg == Energy.Red)
                    _energy = Energy.Purple;
                else if (nrg == Energy.Yellow)
                    _energy = Energy.Green;
                else if (nrg == Energy.Blue)
                    _energy = Energy.Blue;
                else
                    _energy = Energy.Brown;
                break;
            case Energy.Green:
                _energy = Energy.Brown;
                break;
            case Energy.Purple:
                _energy = Energy.Brown;
                break;
            case Energy.Orange:
                _energy = Energy.Brown;
                break;
            case Energy.Brown:
                _energy = Energy.Brown;
                break;
            default:
                break;
        }
        message += ($" made {_energy}");
        Debug.Log(message);

        ColorFlower();

        // Search adjacent flowers for same color, if so paint them too
        CheckAndPaintAdjacentFlower(new Vector2Int(_gridPos.x + 1, _gridPos.y), prevNrg, nrg);
        CheckAndPaintAdjacentFlower(new Vector2Int(_gridPos.x - 1, _gridPos.y), prevNrg, nrg);
        CheckAndPaintAdjacentFlower(new Vector2Int(_gridPos.x, _gridPos.y + 1), prevNrg, nrg);
        CheckAndPaintAdjacentFlower(new Vector2Int(_gridPos.x, _gridPos.y - 1), prevNrg, nrg);
    }

    private void CheckAndPaintAdjacentFlower(Vector2Int pos, Energy prevNrg, Energy nrg)
    {
        if (prevNrg == _grid.GetFlowerEnergyAt(pos))
            _grid.ColorFlower(pos, nrg);
    }

    private void ColorFlower()
    {
        Color toColor = Color.white;
        switch (_energy)
        {
            case Energy.White:
                toColor = Color.white;
                break;
            case Energy.Red:
                toColor = _red;
                break;
            case Energy.Yellow:
                toColor = _yellow;
                break;
            case Energy.Blue:
                toColor = _blue;
                break;
            case Energy.Green:
                toColor = _green;
                break;
            case Energy.Purple:
                toColor = _purple;
                break;
            case Energy.Orange:
                toColor = _orange;
                break;
            case Energy.Brown:
                toColor = _brown;
                break;
            default:
                break;
        }

        _sr.DOColor(toColor, 1f);
    }
    #endregion
}
