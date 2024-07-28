using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSetter : MonoBehaviour
{
    //============== Refrences / Variables ==============
    [Header("Refrences")]
    [SerializeField] private SpriteRenderer _coreSR;
    [SerializeField] private GameObject _catalystObj;
    [SerializeField] private Sprite _catalystSprite;
    [SerializeField] private GameObject _orbObj;
    [SerializeField] private Sprite _orbSprite;
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

    //============== Function ==============
    public void Setup(Flower.Energy nrg, bool isCatalyst)
    {
        if (isCatalyst)
        {
            _catalystObj.SetActive(true);
            _orbObj.SetActive(false);
            _coreSR.sprite = _catalystSprite;
        }
        else
        {
            _catalystObj.SetActive(false);
            _orbObj.SetActive(true);
            _coreSR.sprite = _orbSprite;
        }

        ColorObj(nrg);
    }

    private void ColorObj(Flower.Energy _energy)
    {
        Color toColor = Color.white;
        switch (_energy)
        {
            case Flower.Energy.White:
                toColor = Color.white;
                break;
            case Flower.Energy.Red:
                toColor = _red;
                _symbol.sprite = _symbolRed;
                break;
            case Flower.Energy.Yellow:
                toColor = _yellow;
                _symbol.sprite = _symbolYellow;
                break;
            case Flower.Energy.Blue:
                toColor = _blue;
                _symbol.sprite = _symbolBlue;
                break;
            case Flower.Energy.Green:
                toColor = _green;
                _symbol.sprite = _symbolGreen;
                break;
            case Flower.Energy.Purple:
                toColor = _purple;
                _symbol.sprite = _symbolPurple;
                break;
            case Flower.Energy.Orange:
                toColor = _orange;
                _symbol.sprite = _symbolOrange;
                break;
            case Flower.Energy.Brown:
                break;
            default:
                break;
        }

        _coreSR.color = toColor;
    }
}
