using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthChangedEvent", menuName = "ScriptableObjects/Events/Combat/HealthChangedEvent")]
public class HealthChangedEvent : ScriptableObject
{
    private Action<IDamageable, float, float> _listeners;

    public void Register(Action<IDamageable, float, float> listener)
    {
        _listeners += listener;
    }

    public void Unregister(Action<IDamageable, float, float> listener)
    {
        _listeners -= listener;
    }

    public void Raise(IDamageable target, float current, float max)
    {
        _listeners?.Invoke(target, current, max);
    }
}
