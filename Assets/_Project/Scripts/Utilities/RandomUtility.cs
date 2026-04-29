using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class RandomUtility
{
    public static T GetWeighted<T>(List<T> list, Func<T, float> weightSelector)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("GetWeighted called with empty list!");
            return default;
        }

        float total = 0f;

        foreach (var item in list)
        {
            total += Mathf.Max(0f, weightSelector(item)); // prevent negative weights
        }

        if (total <= 0f)
        {
            Debug.LogWarning("All weights are zero!");
            return list[0];
        }

        float random = Random.Range(0f, total);
        float current = 0f;

        foreach (var item in list)
        {
            current += Mathf.Max(0f, weightSelector(item));
            if (random <= current)
            {
                return item;
            }
        }

        return list[0]; // fallback safety
    }

    public static T GetRandom<T>(List<T> list)
    {
        if (list != null && list.Count != 0)
        {
            return list[Random.Range(0, list.Count)];
        }
        
        Debug.LogWarning("GetRandom called with empty list!");
        return default;

    }
}
