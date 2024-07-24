using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //if(_liquid == Liquid.Brown)
        //{
        //    _liquid = Liquid.White;
        //    ColorTile();
        //}
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
        switch (_liquid)
        {
            case Liquid.White:
                _sr.color = Color.white;
                break;
            case Liquid.Red:
                _sr.color = _red;
                break;
            case Liquid.Yellow:
                _sr.color = _yellow;
                break;
            case Liquid.Blue:
                _sr.color = _blue;
                break;
            case Liquid.Green:
                _sr.color = _green;
                break;
            case Liquid.Purple:
                _sr.color = _purple;
                break;
            case Liquid.Orange:
                _sr.color = _orange;
                break;
            case Liquid.Brown:
                _sr.color = _brown;
                break;
            default:
                break;
        }
    }
    #endregion
}
