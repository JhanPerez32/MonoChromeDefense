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
        if (_path == null || _path.Count == 0) return;
        if (_index >= _path.Count) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        PathNode targetNode = _path[_index];
        if (!targetNode) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetNode.transform.position,
            moveSpeed * Time.deltaTime
        );
        
        if ((transform.position - targetNode.transform.position).sqrMagnitude > 0.01f) return;
        
        transform.position = targetNode.transform.position;
        _currentNode = targetNode;
        
        if (targetNode.attachedBase && targetNode.attachedBase.IsActive())
        {
            // If this base is not our current target > it blocks us
            if (targetNode.attachedBase != _target)
            {
                Debug.Log($"{name}: Blocked by {targetNode.attachedBase.name}");

                // Switch target to blocking base
                _target = targetNode.attachedBase;
                
                if (enemyBehaviour)
                {
                    enemyBehaviour.SetTarget(_target);
                }

                // Stop movement path
                _path = null;

                // Attack immediately
                enemyBehaviour.DealDamage();
                return;
            }
        }

        bool isLastNode = (_index >= _path.Count - 1);

        if (isLastNode)
        {
            _path = null;
            enemyBehaviour.DealDamage();
        }

        _index++;
    }
    
    public void SetTarget(PlayerBase target)
    {
        _target = target;
        
        if (enemyBehaviour)
        {
            enemyBehaviour.SetTarget(target);
        }

        if (!_target)
        {
            Debug.LogWarning($"{name}: Target is null");
            return;
        }

        RecalculatePath();
    }

    public void ForceSetStartNode(PathNode node)
    {
        _currentNode = node;
    }

    public void ResetAgent()
    {
        _path = null;
        _target = null;
        _index = 0;
    }
    
    private void RecalculatePath()
    {
        if (!_target)
        {
            Debug.LogWarning($"{name}: No target");
            return;
        }

        if (!_currentNode)
        {
            Debug.LogWarning($"{name}: No start node");
            return;
        }

        if (!_target.entryNode)
        {
            Debug.LogWarning($"{name}: Target has no entry node");
            return;
        }

        PathNode startNode = GetRepathStartNode();

        _path = PathResolver.FindPath(startNode, _target.entryNode);

        if (_path == null || _path.Count == 0)
        {
            Debug.LogWarning($"{name}: No valid path found");
            return;
        }

        _index = 0;

        Debug.Log($"{name}: Path created with {_path.Count} nodes");
    }
    
    private void OnAnyDeath(IDamageable dead)
    {
        PlayerBase deadBase = dead as PlayerBase;
        if (!deadBase) return;

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
        {
            return _path[_index];
        }

        return _currentNode;
    }
}
