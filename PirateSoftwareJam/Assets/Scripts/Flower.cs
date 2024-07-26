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

    [Header("Refrences")]
    [SerializeField] private Color _red;
    [SerializeField] private Color _yellow;
    [SerializeField] private Color _blue;
    [SerializeField] private Color _green;
    [SerializeField] private Color _purple;
    [SerializeField] private Color _orange;
    [SerializeField] private Color _brown;
    [SerializeField] private Energy _energy;
    private Vector2Int _gridPos;
    private GameGrid _grid;
    private SpriteRenderer _sr;

    [Header("Testing")]
    [SerializeField] private TextMeshProUGUI _coords;
    [SerializeField] private TextMeshProUGUI _comboID;
    [SerializeField] private TextMeshProUGUI _points;
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

    public void Setup(Vector2Int gridPos, GameGrid grid, Flower.Energy nrg)
    {
        _sr = this.GetComponent<SpriteRenderer>();

        _grid = grid;
        _gridPos = gridPos;

        transform.localScale = _grid.GetGridCellSize();

        _energy = nrg;

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

    public void SetGridPos(Vector2Int gridPos)
    {
        _gridPos = gridPos;
        _coords.text = $"({_gridPos.x},{_gridPos.y})";
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

    public void SetComboID(int comboID)
    {
        _comboID.text = comboID.ToString();
    }

    private void GameTick()
    {
        float gridSizeY = _grid.GetGridScaleY();

        if (_gridPos.y > (gridSizeY - gridSizeY/2))
            _grid.ShowGameEnd();
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
                else if (nrg == Energy.Orange)
                    _energy = Energy.Orange;
                else if (nrg == Energy.Purple)
                    _energy = Energy.Purple;
                break;
            case Energy.Yellow:
                if (nrg == Energy.Red)
                    _energy = Energy.Orange;
                else if (nrg == Energy.Yellow)
                    _energy = Energy.Yellow;
                else if (nrg == Energy.Blue)
                    _energy = Energy.Green;
                else if(nrg == Energy.Orange)
                    _energy = Energy.Orange;
                else if (nrg == Energy.Green)
                    _energy = Energy.Green;
                break;
            case Energy.Blue:
                if (nrg == Energy.Red)
                    _energy = Energy.Purple;
                else if (nrg == Energy.Yellow)
                    _energy = Energy.Green;
                else if (nrg == Energy.Blue)
                    _energy = Energy.Blue;
                else if (nrg == Energy.Purple)
                    _energy = Energy.Purple;
                else if (nrg == Energy.Green)
                    _energy = Energy.Green;
                break;
            case Energy.Green:
                break;
            case Energy.Purple:
                break;
            case Energy.Orange:
                break;
            case Energy.Brown:
                break;
            default:
                break;
        }
        message += ($" made {_energy}");
        //Debug.Log(message);

        if (prevNrg == _energy)
            return;

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

        _sr.DOColor(toColor, 0.6f);
    }

    public void Score(float value)
    {
        _coords.gameObject.SetActive(false);
        _comboID.gameObject.SetActive(false);
        _points.gameObject.SetActive(true);
        _points.text = value.ToString();
        Destroy(gameObject, 0.5f);
    }
    #endregion
}
