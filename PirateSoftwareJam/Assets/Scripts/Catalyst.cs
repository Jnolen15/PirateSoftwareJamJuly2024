using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Catalyst : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private Flower.Energy _energy;

    [SerializeField] private Color _orange;
    [SerializeField] private Color _purple;
    [SerializeField] private Color _green;

    [SerializeField] private Vector2Int _gridPos;

    [SerializeField] private GameGrid _grid;
    [SerializeField] private SpriteRenderer _sr;

    [SerializeField] private TextMeshProUGUI _coords;
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

        ColorCatalyst();

        _coords.text = $"({_gridPos.x},{_gridPos.y})";
    }
    #endregion

    //============== Function ==============
    #region Function
    private void GameTick()
    {
        //Destroy(gameObject, 0.2f);
    }

    public void Break()
    {
        Destroy(gameObject, 0.2f);
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
                break;
            case Flower.Energy.Purple:
                toColor = _purple;
                break;
            case Flower.Energy.Green:
                toColor = _green;
                break;
            default:
                break;
        }

        _sr.DOColor(toColor, 1f);
    }
    #endregion
}
