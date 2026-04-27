using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRetargeter : MonoBehaviour
{
    public event Action OnNoTargetsRemaining;

    public PlayerBase TryGetNewTarget(Vector3 fromPosition)
    {
        List<PlayerBase> bases = RuntimeWorld.Bases.GetValidTargets();

        if (bases == null || bases.Count == 0)
        {
            OnNoTargetsRemaining?.Invoke();
            return null;
        }

        PlayerBase best = null;
        float bestDistance = float.MaxValue;

        foreach (var playerBase in bases)
        {
            float distance = Vector3.Distance(fromPosition, playerBase.transform.position);

            if (!(distance < bestDistance)) continue;
            
            bestDistance = distance;
            best = playerBase;
        }

        return best;
    }
}
