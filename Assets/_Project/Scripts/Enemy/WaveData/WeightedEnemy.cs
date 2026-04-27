using UnityEngine;
using System;

[Serializable]
public class WeightedEnemy
{
    public EnemyBehaviour enemy;

    [Range(0f, 1f)]
    public float weight = 1f;
    
    public int poolSize = 10;
}
