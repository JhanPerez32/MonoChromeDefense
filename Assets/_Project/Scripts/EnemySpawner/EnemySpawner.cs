using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave System")]
    public WaveData waveData;

    [Header("Pool")]
    public PoolFactory poolFactory;

    [Header("Spawn")]
    public PathNode spawnNode;

    private int _currentWaveIndex;
    private int _spawnedInWave;
    private int _aliveEnemies;

    private float _timer;

    private Wave CurrentWave => waveData.waves[_currentWaveIndex];

    private void Start()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var wave in waveData.waves)
        {
            foreach (var enemy in wave.enemies)
            {
                poolFactory.Initialize(enemy.enemy, enemy.poolSize, transform);
            }
        }
    }

    private void Update()
    {
        var playerBasesList = RuntimeWorld.Bases.GetValidTargets();
        if (playerBasesList.Count == 0) return;

        if (_currentWaveIndex >= waveData.waves.Count) return;

        _timer += Time.deltaTime;
        
        if (_spawnedInWave < CurrentWave.spawnCount)
        {
            if (_timer < CurrentWave.spawnInterval) return;

            _timer = 0f;
            SpawnEnemy(playerBasesList);
        }
        else if (_aliveEnemies <= 0)
        {
            NextWave();
        }
    }

    private void SpawnEnemy(List<PlayerBase> bases)
    {
        PlayerBase targetBase = RandomUtility.GetRandom(bases);
        if (!targetBase) return;
        
        WeightedEnemy selected = RandomUtility.GetWeighted(CurrentWave.enemies, weightedEnemy => weightedEnemy.weight);

        if (selected == null || !selected.enemy)
        {
            Debug.LogWarning("No valid enemy selected!");
            return;
        }

        EnemyBehaviour enemy = poolFactory.Get(selected.enemy);

        if (!enemy)
        {
            Debug.LogWarning("Pool exhausted!");
            return;
        }

        enemy.SetPoolFactory(poolFactory);
        enemy.SetSpawner(this);
        enemy.Initialize(targetBase, spawnNode);

        _aliveEnemies++;
        _spawnedInWave++;
    }
    
    public void OnEnemyReturned()
    {
        _aliveEnemies--;
    }

    private void NextWave()
    {
        _currentWaveIndex++;
        _spawnedInWave = 0;
        _aliveEnemies = 0;
        _timer = 0f;

        Debug.Log("Next Wave: " + _currentWaveIndex);
    }
}
