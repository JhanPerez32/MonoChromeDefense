using System.Collections.Generic;
using UnityEngine;

public class BaseHealthUIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthChangedEvent healthChangedEvent;

    [Header("UI")]
    [SerializeField] private GameObject baseUIPrefab;
    [SerializeField] private Transform uiContainer;

    private readonly Dictionary<PlayerBase, PlayerBaseHealthUI> _uiMap = new();

    private void OnEnable()
    {
        RuntimeWorld.Bases.OnChanged += OnBaseListChanged;
        healthChangedEvent.Register(OnHealthChanged);
        
        CreateMissingUI(RuntimeWorld.Bases.GetAll());
    }

    private void OnDisable()
    {
        RuntimeWorld.Bases.OnChanged -= OnBaseListChanged;
        healthChangedEvent.Unregister(OnHealthChanged);
    }

    private void OnBaseListChanged(List<PlayerBase> bases)
    {
        CreateMissingUI(bases);
    }

    private void CreateMissingUI(List<PlayerBase> bases)
    {
        for (int i = 0; i < bases.Count; i++)
        {
            PlayerBase playerBase = bases[i];

            if (playerBase == null) continue;

            if (_uiMap.ContainsKey(playerBase))
                continue;

            GameObject gameObjectInstantiate = Instantiate(baseUIPrefab, uiContainer);
            PlayerBaseHealthUI playerBaseHealthUI = gameObjectInstantiate.GetComponent<PlayerBaseHealthUI>();

            playerBaseHealthUI.Initialize(playerBase, i);

            _uiMap.Add(playerBase, playerBaseHealthUI);
        }
    }

    private void OnHealthChanged(IDamageable target, float current, float max)
    {
        if (target is not PlayerBase baseRef) return;

        if (!_uiMap.TryGetValue(baseRef, out PlayerBaseHealthUI playerBaseHealthUI)) return;
        
        playerBaseHealthUI.UpdateHealth(current, max);
        
        playerBaseHealthUI.SetDeadVisual(baseRef.State != BaseStateEnum.Active);
    }
}
