using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //============== Refrences / Variables ==============
    #region R/V
    [SerializeField] private Color _red;
    [SerializeField] private Color _yellow;
    [SerializeField] private Color _blue;
    [SerializeField] private Color _green;
    [SerializeField] private Color _purple;
    [SerializeField] private Color _orange;
    [SerializeField] private Color _brown;

    [SerializeField] private Flower.Energy _weakness;
    [SerializeField] private Vector2Int _gridPos;

    [SerializeField] private GameGrid _grid;
    [SerializeField] private SpriteRenderer _sr;
    #endregion

    //============== Setup ==============
    #region Setup
    //private void Start()
    //{
    //    GameGrid.GameTick += CheckKill;
    //    GameGrid.EnemyGameTick += Move;
    //}

    //private void OnDestroy()
    //{
    //    GameGrid.GameTick -= CheckKill;
    //    GameGrid.EnemyGameTick -= Move;
    //}

    public void Setup(Vector2Int gridPos, GameGrid grid)
    {
        _sr = this.GetComponent<SpriteRenderer>();

        _grid = grid;
        _gridPos = gridPos;

        transform.localScale = _grid.GetGridCellSize() * 0.9f;

        // Assign weakness
        int rand = Random.Range(1, 4);
        //if (rand == 1)
        //    _weakness = Tile.Liquid.Red;
        //else if (rand == 2)
        //    _weakness = Tile.Liquid.Yellow;
        //else if (rand == 3)
        //    _weakness = Tile.Liquid.Blue;
        //if (rand == 1)
        //    _weakness = Tile.Liquid.Green;
        //else if (rand == 2)
        //    _weakness = Tile.Liquid.Purple;
        //else if (rand == 3)
        //    _weakness = Tile.Liquid.Orange;

        //ColorEnemy();
    }
    #endregion

    //============== Function ==============
    #region Function
    //private void ColorEnemy()
    //{
    //    switch (_weakness)
    //    {
    //        case Tile.Liquid.White:
    //            _sr.color = Color.white;
    //            break;
    //        case Tile.Liquid.Red:
    //            _sr.color = _red;
    //            break;
    //        case Tile.Liquid.Yellow:
    //            _sr.color = _yellow;
    //            break;
    //        case Tile.Liquid.Blue:
    //            _sr.color = _blue;
    //            break;
    //        case Tile.Liquid.Green:
    //            _sr.color = _green;
    //            break;
    //        case Tile.Liquid.Purple:
    //            _sr.color = _purple;
    //            break;
    //        case Tile.Liquid.Orange:
    //            _sr.color = _orange;
    //            break;
    //        case Tile.Liquid.Brown:
    //            _sr.color = _brown;
    //            break;
    //        default:
    //            break;
    //    }
    //}

    private void Move()
    {
        Debug.Log("Moving enemy, cur pos " + _gridPos);

        //_gridPos = GameGrid.Instance.MoveDown(_gridPos, transform);

        Debug.Log("Moved to" + _gridPos);

        if (GameGrid.Instance.CheckMatchingTile(_gridPos, _weakness))
            StartCoroutine(EnemyDeath());
    }
    
    private void CheckKill()
    {
        if (GameGrid.Instance.CheckMatchingTile(_gridPos, _weakness))
            StartCoroutine(EnemyDeath());
            
    }

    private IEnumerator EnemyDeath()
    {
        yield return new WaitForSeconds(0.2f);

        GameGrid.Instance.ClearTile(_gridPos);
        Destroy(gameObject);
    }
    #endregion
}
