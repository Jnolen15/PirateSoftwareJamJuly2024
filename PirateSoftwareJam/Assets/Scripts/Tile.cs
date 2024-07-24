using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tile : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    public enum Liquid
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
    [SerializeField] private Liquid _liquid;

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

    private int _brownEvaporationTime;
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
    }
    #endregion

    //============== Function ==============
    #region Function
    public Liquid GetLiquid()
    {
        return _liquid;
    }

    public void SetLiquid(Liquid liquid)
    {
        _liquid = liquid;
        ColorTile();
    }

    private void GameTick()
    {
        if(_liquid == Liquid.Brown)
        {
            if(_brownEvaporationTime >= 1)
            {
                _brownEvaporationTime = 0;
                _liquid = Liquid.White;
                ColorTile();
            }
            else
                _brownEvaporationTime++;
        }
    }

    public void Paint(Liquid liquid)
    {
        string message = ($"Mixing {_liquid} with {liquid}");

        switch (_liquid)
        {
            case Liquid.White:
                _liquid = liquid;
                break;
            case Liquid.Red:
                if (liquid == Liquid.Red)
                    _liquid = Liquid.Red;
                else if (liquid == Liquid.Yellow)
                    _liquid = Liquid.Orange;
                else if (liquid == Liquid.Blue)
                    _liquid = Liquid.Purple;
                else
                    _liquid = Liquid.Brown;
                break;
            case Liquid.Yellow:
                if (liquid == Liquid.Red)
                    _liquid = Liquid.Orange;
                else if (liquid == Liquid.Yellow)
                    _liquid = Liquid.Yellow;
                else if (liquid == Liquid.Blue)
                    _liquid = Liquid.Green;
                else
                    _liquid = Liquid.Brown;
                break;
            case Liquid.Blue:
                if (liquid == Liquid.Red)
                    _liquid = Liquid.Purple;
                else if (liquid == Liquid.Yellow)
                    _liquid = Liquid.Green;
                else if (liquid == Liquid.Blue)
                    _liquid = Liquid.Blue;
                else
                    _liquid = Liquid.Brown;
                break;
            case Liquid.Green:
                _liquid = Liquid.Brown;
                break;
            case Liquid.Purple:
                _liquid = Liquid.Brown;
                break;
            case Liquid.Orange:
                _liquid = Liquid.Brown;
                break;
            case Liquid.Brown:
                _liquid = Liquid.Brown;
                break;
            default:
                break;
        }

        message += ($" made {_liquid}");
        Debug.Log(message);

        ColorTile();
    }

    private void ColorTile()
    {
        Color toColor = Color.white;
        switch (_liquid)
        {
            case Liquid.White:
                toColor = Color.white;
                break;
            case Liquid.Red:
                toColor = _red;
                break;
            case Liquid.Yellow:
                toColor = _yellow;
                break;
            case Liquid.Blue:
                toColor = _blue;
                break;
            case Liquid.Green:
                toColor = _green;
                break;
            case Liquid.Purple:
                toColor = _purple;
                break;
            case Liquid.Orange:
                toColor = _orange;
                break;
            case Liquid.Brown:
                toColor = _brown;
                break;
            default:
                break;
        }

        _sr.DOColor(toColor, 1f);
    }
    #endregion
}
