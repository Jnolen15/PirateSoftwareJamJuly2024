using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblesFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fx;

    private void Start()
    {
        GameGrid.PointsScored += EmitBubbles;
    }

    private void OnDestroy()
    {
        GameGrid.PointsScored -= EmitBubbles;
    }

    private void EmitBubbles(Color color)
    {
        _fx.Emit(10);
    }
}
