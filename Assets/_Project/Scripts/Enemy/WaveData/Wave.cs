using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Wave
{
    [Header("Enemies")]
    public List<WeightedEnemy> enemies;

    [Header("Spawning")]
    public int spawnCount = 10;
    public float spawnInterval = 1f;
}
