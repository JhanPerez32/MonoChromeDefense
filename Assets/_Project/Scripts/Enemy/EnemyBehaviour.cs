using UnityEngine;
using Object = System.Object;

public class EnemyBehaviour : MonoBehaviour, IPoolable
{
    [Header("Damage")]
    [SerializeField] private float damage = 10f;

    private PoolFactory _poolFactory;
    private EnemySpawner _spawner;

    public EnemyPathAgent pathAgent;
    public EnemyRetargeter retargeter;

    private IDamageable _target;

    private bool _isActive;
    private bool _isFinished;

    public Object OriginalPrefab { get; set; }

    public void SetPoolFactory(PoolFactory factory)
    {
        _poolFactory = factory;
    }

    public void SetSpawner(EnemySpawner spawner)
    {
        _spawner = spawner;
    }
    
    public void Initialize(IDamageable target, PathNode spawnNode)
    {
        _target = target;

        _isActive = true;
        _isFinished = false;
        
        transform.position = spawnNode.transform.position;
        transform.rotation = Quaternion.identity;
        
        pathAgent.ResetAgent();
        pathAgent.ForceSetStartNode(spawnNode);
        
        pathAgent.SetTarget(target as PlayerBase);

        gameObject.SetActive(true);
    }
    
    private void Update()
    {
        if (!_isActive || _isFinished) return;

        if (_target == null || !_target.IsActive())
        {
            TryRetarget();
        }
    }
    
    private void TryRetarget()
    {
        PlayerBase newTarget = retargeter.TryGetNewTarget(transform.position);

        if (!newTarget)
        {
            ReturnToPool();
            return;
        }

        _target = newTarget;

        pathAgent.SetTarget(newTarget);
    }

    private void HandleNoTargets()
    {
        ReturnToPool();
    }
    
    public void DealDamage()
    {
        if (_isFinished) return;

        if (_target != null && _target.IsActive())
        {
            _target.TakeDamage(damage);
        }

        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        if (_isFinished) return;

        _isFinished = true;
        _isActive = false;
        
        if (retargeter)
        {
            retargeter.OnNoTargetsRemaining -= HandleNoTargets;
        }

        _poolFactory?.Return(this);
        _spawner?.OnEnemyReturned();
    }
    
    public void OnSpawn()
    {
        gameObject.SetActive(true);

        _isActive = false;
        _isFinished = false;

        _target = null;

        if (pathAgent)
        {
            pathAgent.ResetAgent();
        }
    }

    public void OnDespawn()
    {
        _isActive = false;
        _isFinished = true;

        _target = null;

        if (retargeter)
        {
            retargeter.OnNoTargetsRemaining -= HandleNoTargets;
        }

        gameObject.SetActive(false);
    }
}