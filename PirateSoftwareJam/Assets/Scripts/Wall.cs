using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private Vector2Int _gridPos;
    [SerializeField] private GameGrid _grid;
    #endregion

    //============== Setup ==============
    #region Setup
    public void Setup(Vector2Int gridPos, GameGrid grid)
    {
        _grid = grid;
        _gridPos = gridPos;

        transform.localScale = _grid.GetGridSize();
    }
    #endregion
}
