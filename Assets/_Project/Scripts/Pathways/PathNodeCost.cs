using UnityEngine;

public class PathNodeCost : MonoBehaviour
{
    [Header("Dynamic Costs")]
    public float baseCost = 0f;

    [Tooltip("Higher = more dangerous (towers, enemies, etc.)")]
    public float dangerCost = 0f;

    [Tooltip("Slow terrain, mud, etc.")]
    public float movementPenalty = 0f;

    public float GetTotalCost()
    {
        return baseCost + dangerCost + movementPenalty;
    }
}
