using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyPathAgent : MonoBehaviour
{
    public float moveSpeed = 3f;

    [Header("Events")]
    [SerializeField] private DeathEvent playerBaseDeathEvent;

    [Header("Components")]
    public EnemyBehaviour enemyBehaviour;
    
    [Header("AI Preferences")]
    [SerializeField] private List<PathPreferenceScriptable> preferences;

    private PathPreferenceScriptable _currentPreference;

    private PathNode _currentNode;
    private PlayerBase _target;
    private List<PathNode> _path;
    private int _index;
    
    public PathNode CurrentNode => _currentNode;
    
    private void OnEnable()
    {
        playerBaseDeathEvent?.Register(OnAnyDeath);
    }

    private void OnDisable()
    {
        playerBaseDeathEvent?.Unregister(OnAnyDeath);
    }
    
    private void Update()
    {
        if (!HasValidPath()) return;

        MoveAlongPath();
    }
    
     public void InitializePreference()
    {
        _currentPreference = ChoosePreference();
    }

    private PathPreferenceScriptable ChoosePreference()
    {
        if (preferences == null || preferences.Count == 0)
        {
            return null;
        }

        float totalWeight = 0f;

        foreach (var pathPreferenceScriptable in preferences)
        {
            totalWeight += pathPreferenceScriptable.selectionWeight;
        }

        float roll = Random.Range(0, totalWeight);

        float cumulative = 0f;

        foreach (var pathPreferenceScriptable in preferences)
        {
            cumulative += pathPreferenceScriptable.selectionWeight;

            if (roll <= cumulative)
            {
                return pathPreferenceScriptable;
            }
        }

        return preferences[0];
    }

    private void MoveAlongPath()
    {
        PathNode targetNode = _path[_index];
        if (!targetNode) return;

        MoveTo(targetNode);

        if (!Reached(targetNode)) return;

        OnNodeReached(targetNode);
    }

    private void MoveTo(PathNode node)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            node.transform.position,
            moveSpeed * Time.deltaTime
        );
    }

    private bool Reached(PathNode node)
    {
        return (transform.position - node.transform.position).sqrMagnitude <= 0.01f;
    }

    private void OnNodeReached(PathNode node)
    {
        transform.position = node.transform.position;
        _currentNode = node;

        if (TryHandleBlockingBase(node)) return;

        if (IsLastNode())
        {
            AttackTarget();
            return;
        }

        _index++;
    }

    private bool TryHandleBlockingBase(PathNode node)
    {
        PlayerBase nodeBase = node.attachedBase;

        if (!nodeBase || !nodeBase.IsActive())
        {
            return false;
        }

        if (nodeBase == _target)
        {
            return false;
        }

        Debug.Log($"{name}: Blocked by {nodeBase.name}");

        SetTargetInternal(nodeBase);

        StopPath();
        AttackTarget();

        return true;
    }

    public void SetTarget(PlayerBase target)
    {
        if (!target)
        {
            target = GetPreferredTarget();
        }

        SetTargetInternal(target);

        if (!_target)
        {
            Debug.LogWarning($"{name}: Target is null");
            return;
        }

        RecalculatePath();
    }

    private PlayerBase GetPreferredTarget()
    {
        List<PlayerBase> bases = RuntimeWorld.Bases.GetValidTargets();

        if (bases == null || bases.Count == 0)
        {
            return null;
        }

        if (!_currentPreference)
        {
            return bases[Random.Range(0, bases.Count)];
        }

        switch (_currentPreference.targetPriority)
        {
            case TargetPriorityType.Closest:
            {
                return GetClosest(bases);
            }

            case TargetPriorityType.LowestHealth:
            {
                return GetLowestHealth(bases);
            }

            case TargetPriorityType.HighestHealth:
            {
                return GetHighestHealth(bases);
            }

            case TargetPriorityType.Random:
            {
                return bases[Random.Range(0, bases.Count)];
            }
        }

        return null;
    }

    private PlayerBase GetClosest(List<PlayerBase> bases)
    {
        PlayerBase best = null;
        float bestDist = float.MaxValue;

        foreach (var playerBase in bases)
        {
            float distance = Vector3.Distance(transform.position, playerBase.transform.position);

            if (!(distance < bestDist)) continue;
            
            bestDist = distance;
            best = playerBase;
        }

        return best;
    }

    private PlayerBase GetLowestHealth(List<PlayerBase> bases)
    {
        PlayerBase best = null;
        float lowestPlayerBaseHealth = float.MaxValue;

        foreach (var playerBase in bases)
        {
            if (!playerBase.IsActive()) continue;

            float playerBaseCurrentHealth = playerBase.GetCurrentHealth();

            if (!(playerBaseCurrentHealth < lowestPlayerBaseHealth)) continue;
            
            lowestPlayerBaseHealth = playerBaseCurrentHealth;
            best = playerBase;
        }

        return best;
    }

    private PlayerBase GetHighestHealth(List<PlayerBase> bases)
    {
        PlayerBase best = null;
        float highestPlayerBaseHealth = float.MinValue;

        foreach (var playerBase in bases)
        {
            float playerBaseMaxHealth = playerBase.GetMaxHealth();

            if (!(playerBaseMaxHealth > highestPlayerBaseHealth)) continue;
            
            highestPlayerBaseHealth = playerBaseMaxHealth;
            best = playerBase;
        }

        return best;
    }

    private void SetTargetInternal(PlayerBase target)
    {
        _target = target;

        if (enemyBehaviour)
        {
            enemyBehaviour.SetTarget(target);
        }
    }
    

    private void RecalculatePath()
    {
        if (!IsValidForPath()) return;

        PathNode startNode = GetRepathStartNode();

        _path = PathResolver.FindPath(
            startNode,
            _target.entryNode,
            _currentPreference ? _currentPreference.pathPreference : PathPreferenceTypeEnum.Random
        );

        if (_path == null || _path.Count == 0)
        {
            Debug.LogWarning($"{name}: No valid path found");
            return;
        }

        _index = 0;
    }

    private bool IsValidForPath()
    {
        return _target && _currentNode && _target.entryNode;
    }

    private bool HasValidPath()
    {
        return _path != null && _index < _path.Count;
    }

    private bool IsLastNode()
    {
        return _index >= _path.Count - 1;
    }

    private void StopPath()
    {
        _path = null;
    }

    private void AttackTarget()
    {
        StopPath();
        enemyBehaviour.DealDamage();
    }

    private void OnAnyDeath(IDamageable dead)
    {
        if (dead is not PlayerBase deadBase) return;
        if (_target != deadBase) return;

        StartCoroutine(RepathRoutine());
    }

    private IEnumerator RepathRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (!_target || !_currentNode) yield break;

        _currentPreference = ChoosePreference();

        RecalculatePath();
    }

    private PathNode GetRepathStartNode()
    {
        if (_path != null && _index < _path.Count)
        {
            return _path[_index];
        }

        return _currentNode;
    }

    public void ForceSetStartNode(PathNode node)
    {
        _currentNode = node;
    }

    public void ResetAgent()
    {
        _path = null;
        _index = 0;
        _target = null;
    }
}
