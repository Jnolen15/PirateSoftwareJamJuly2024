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
    [SerializeField] private GameObject _burstFX;
    [SerializeField] private SpriteRenderer _symbol;
    [SerializeField] private SpriteRenderer _swirl;
    [SerializeField] private Color _red;
    [SerializeField] private Sprite _symbolRed;
    [SerializeField] private Color _yellow;
    [SerializeField] private Sprite _symbolYellow;
    [SerializeField] private Color _blue;
    [SerializeField] private Sprite _symbolBlue;
    [SerializeField] private Color _green;
    [SerializeField] private Sprite _symbolGreen;
    [SerializeField] private Color _purple;
    [SerializeField] private Sprite _symbolPurple;
    [SerializeField] private Color _orange;
    [SerializeField] private Sprite _symbolOrange;
    [SerializeField] private Energy _energy;
    private Vector2Int _gridPos;
    private GameGrid _grid;
    private SpriteRenderer _sr;
    private SoundPlayer _soundPlayer;
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

        _swirl.transform.DOKill();
        _sr.DOKill();
    }

    public void Setup(Vector2Int gridPos, GameGrid grid, Flower.Energy nrg, bool playSound)
    {
        _sr = this.GetComponent<SpriteRenderer>();

        _grid = grid;
        _gridPos = gridPos;

        transform.localScale = _grid.GetGridCellSize();

        _energy = nrg;

        ColorFlower();

        _soundPlayer = this.GetComponent<SoundPlayer>();
        if (playSound)
            _soundPlayer.PlayRandom(true);
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

    public Color GetColor()
    {
        return PickColorOnEnergy();
    }

    public void ShakeOrb()
    {
        transform.DOPunchPosition(new Vector3(0, 0.1f, 0), 0.2f, 20);
    }

    private void GameTick()
    {
        float gridSizeY = _grid.GetGridScaleY();

        if (_gridPos.y > (gridSizeY - gridSizeY/2))
            _grid.ShowGameEnd();
    }

    public void MoveTo(Vector3 newPos, bool animate)
    {
        transform.DOKill();

        if (animate)
            transform.DOMove(newPos, 0.1f).SetEase(Ease.InSine).OnComplete(() => { ShakeOrb(); });
        else
            transform.position = newPos;
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
        Color toColor = PickColorOnEnergy();

        _swirl.transform.rotation = Quaternion.Euler(0, 0, 0);
        _swirl.gameObject.SetActive(true);
        _swirl.color = toColor;
        _swirl.transform.DORotateQuaternion(Quaternion.Euler(0, 0, -180), 0.9f).OnComplete(() => { _swirl.gameObject.SetActive(false); });
        _sr.DOColor(toColor, 0.9f).SetEase(Ease.InSine);
    }

    private Color PickColorOnEnergy()
    {
        Color toColor = Color.white;
        switch (_energy)
        {
            case Energy.White:
                toColor = Color.white;
                break;
            case Energy.Red:
                toColor = _red;
                _symbol.sprite = _symbolRed;
                break;
            case Energy.Yellow:
                toColor = _yellow;
                _symbol.sprite = _symbolYellow;
                break;
            case Energy.Blue:
                toColor = _blue;
                _symbol.sprite = _symbolBlue;
                break;
            case Energy.Green:
                toColor = _green;
                _symbol.sprite = _symbolGreen;
                break;
            case Energy.Purple:
                toColor = _purple;
                _symbol.sprite = _symbolPurple;
                break;
            case Energy.Orange:
                toColor = _orange;
                _symbol.sprite = _symbolOrange;
                break;
            case Energy.Brown:
                break;
            default:
                break;
        }

        return toColor;
    }

    public void Glow()
    {
        _symbol.DOColor(Color.white, 0.2f);
    }

    public void Score(int value)
    {
        BurstFX fx = Instantiate(_burstFX, transform.position, transform.rotation).GetComponent<BurstFX>();
        fx.Setup(value, _sr.color, new Vector2Int(3, 5));
        Destroy(gameObject);
    }
    #endregion
}
