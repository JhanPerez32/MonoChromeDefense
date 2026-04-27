using System;
using UnityEngine;

[Serializable]
public class RuntimeStats
{
    public float maxHealth;

    public RuntimeStats(StatsUnit baseStats)
    {
        maxHealth = baseStats.maxHealth;
    }
}
