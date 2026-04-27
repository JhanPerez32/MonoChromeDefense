using UnityEngine;

public class PlayerBase : MonoBehaviour, ITargetable, IDamageable
{
    public PathNode entryNode;
    
    [Header("Events")]
    [SerializeField] private HealthChangedEvent healthChangedEvent;
    [SerializeField] private DeathEvent playerBaseDeathEvent;
    
    [Header("Stats")]
    [SerializeField] private StatsUnit statsUnit;
    private RuntimeStats _runtimeStats;

    [Header("Faction")]
    [SerializeField] private FactionScriptable playerFaction;
    
    public BaseStateEnum State { get; private set; } = BaseStateEnum.Active;

    private float _currentHealth;
    private float _maxHealthRuntime;

    private bool IsAlive => State == BaseStateEnum.Active;

    private void Awake()
    {
        _runtimeStats = new RuntimeStats(statsUnit);

        _maxHealthRuntime = _runtimeStats.maxHealth;
        _currentHealth = _maxHealthRuntime;
    }

    private void OnEnable()
    {
        RuntimeWorld.Bases.Register(this);
    }

    private void OnDisable()
    {
        RuntimeWorld.Bases.Unregister(this);
    }
    
    private void Start()
    {
        healthChangedEvent.Raise(this, _currentHealth, _maxHealthRuntime);
    }

    public void TakeDamage(float damageAmount)
    {
        if (State != BaseStateEnum.Active) return;
        
        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Max(_currentHealth, 0);
        
        Debug.Log($"{name} HP: {_currentHealth}");

        // Notify UI / systems
        healthChangedEvent.Raise(this, _currentHealth, _maxHealthRuntime);

        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        if (State == BaseStateEnum.Destroyed) return;

        State = BaseStateEnum.Destroyed;
        
        playerBaseDeathEvent?.Raise(this);
        
        gameObject.SetActive(false);
    }
    
    public void Rebuild(float restoreHealthPercent = 1f)
    {
        Debug.Log($"{name} rebuilt!");

        State = BaseStateEnum.Active;

        _currentHealth = _maxHealthRuntime * Mathf.Clamp01(restoreHealthPercent);

        gameObject.SetActive(true);

        healthChangedEvent.Raise(this, _currentHealth, _maxHealthRuntime);
    }
    
    public bool IsActive() => State == BaseStateEnum.Active;
    public Transform GetTransform() => transform;
    public FactionScriptable GetFaction() => playerFaction;
}
