using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRetargeter : MonoBehaviour
{
    public event Action OnNoTargetsRemaining;

    public PlayerBase TryGetNewTarget(Vector3 fromPosition)
    {
        List<PlayerBase> bases = RuntimeWorld.Bases.GetValidTargets();

        if (bases != null && bases.Count != 0)
        {
            return TargetingUtility.GetClosest(bases, fromPosition);
        }
        
        OnNoTargetsRemaining?.Invoke();
        return null;

    }
}
