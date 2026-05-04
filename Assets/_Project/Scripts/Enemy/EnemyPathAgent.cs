using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPathAgent : MonoBehaviour
{
    public float moveSpeed = 3f;

    [Header("Events")]
    [SerializeField] private DeathEvent playerBaseDeathEvent;

    public EnemyBehaviour enemyBehaviour;
    public PathPreferenceTypeEnum preference;

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

        // Blocking logic
        if (TryHandleBlockingBase(node)) return;

        // End of path
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
        SetTargetInternal(target);

        if (!_target)
        {
            Debug.LogWarning($"{name}: Target is null");
            return;
        }

        RecalculatePath();
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

        _path = PathResolver.FindPath(startNode, _target.entryNode, preference);

        if (_path == null || _path.Count == 0)
        {
            Debug.LogWarning($"{name}: No valid path found");
            return;
        }

        _index = 0;

        Debug.Log($"{name}: Path created with {_path.Count} nodes");
    }
    
    private bool IsValidForPath()
    {
        if (!_target)
        {
            Debug.LogWarning($"{name}: No target");
            return false;
        }

        if (!_currentNode)
        {
            Debug.LogWarning($"{name}: No start node");
            return false;
        }

        if (!_target.entryNode)
        {
            Debug.LogWarning($"{name}: Target has no entry node");
            return false;
        }

        return true;
    }

    private bool HasValidPath()
    {
        return _path != null && _path.Count > 0 && _index < _path.Count;
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

    //Repath
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

        RecalculatePath();
    }

    private PathNode GetRepathStartNode()
    {
        if (_path != null && _index < _path.Count)
            return _path[_index];

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
        SetTargetInternal(null);
    }
}
