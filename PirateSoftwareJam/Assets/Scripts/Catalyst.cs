using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Catalyst : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    private GameGrid _grid;
    private SpriteRenderer _sr;

    [SerializeField] private SpriteRenderer _symbol;
    [SerializeField] private Color _green;
    [SerializeField] private Sprite _symbolGreen;
    [SerializeField] private Color _purple;
    [SerializeField] private Sprite _symbolPurple;
    [SerializeField] private Color _orange;
    [SerializeField] private Sprite _symbolOrange;
    private Flower.Energy _energy;
    private Vector2Int _gridPos;
    #endregion

    //============== Setup ==============
    #region Setup
    public void Setup(Vector2Int gridPos, GameGrid grid, Flower.Energy nrg)
    {
        _sr = this.GetComponent<SpriteRenderer>();

        _grid = grid;
        _gridPos = gridPos;

        transform.localScale = _grid.GetGridCellSize();

        _energy = nrg;

        ColorCatalyst();
    }
    #endregion

    //============== Function ==============
    #region Function
    public void Break()
    {
        Destroy(gameObject, 0.6f);
    }

    private void ColorCatalyst()
    {
        Color toColor = Color.white;
        switch (_energy)
        {
            case Flower.Energy.White:
                toColor = Color.white;
                break;
            case Flower.Energy.Orange:
                toColor = _orange;
                _symbol.sprite = _symbolOrange;
                break;
            case Flower.Energy.Purple:
                toColor = _purple;
                _symbol.sprite = _symbolPurple;
                break;
            case Flower.Energy.Green:
                toColor = _green;
                _symbol.sprite = _symbolGreen;
                break;
            default:
                break;
        }

        _sr.DOColor(toColor, 1f);
    }
    #endregion
}
