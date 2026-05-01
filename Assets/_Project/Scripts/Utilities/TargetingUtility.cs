using System.Collections.Generic;
using UnityEngine;

public static class TargetingUtility
{
    public static T GetClosest<T>(List<T> list, Vector3 fromPosition) where T : MonoBehaviour
    {
        if (list == null || list.Count == 0)
            return null;

        T best = null;
        float bestDistance = float.MaxValue;

        foreach (var item in list)
        {
            if (!item) continue;

            float distance = (item.transform.position - fromPosition).sqrMagnitude;

            if (!(distance < bestDistance)) continue;
            
            bestDistance = distance;
            best = item;
        }

        return best;
    }
}
