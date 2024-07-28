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
    [SerializeField] private GameObject _burstFX;
    [SerializeField] private List<ParticleSystem> _shockFX;
    private Flower.Energy _energy;
    private Vector2Int _gridPos;
    private Color _thisColor;
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

    private void OnDestroy()
    {
        BurstFX fx = Instantiate(_burstFX, transform.position, transform.rotation).GetComponent<BurstFX>();
        fx.Setup(0, _thisColor, new Vector2Int(6, 12));
    }

    public void ShowFX(bool[] dir)
    {
        int index = 0;
        foreach (ParticleSystem fx in _shockFX)
        {
            if (dir[index])
            {
                ParticleSystem.MainModule ma = fx.main;
                _thisColor.a = 120;
                ma.startColor = _thisColor;
                fx.Play();
            }

            index++;
        }
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

        _thisColor = toColor;
        _sr.DOColor(_thisColor, 1f);
    }
    #endregion
}
