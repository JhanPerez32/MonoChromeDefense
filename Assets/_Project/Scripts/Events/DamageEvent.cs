using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New DamageEvent", menuName = "ScriptableObjects/Events/Combat/DamageEvent")]
public class DamageEvent : ScriptableObject
{
    private Action<IDamageable, float> _listeners;

    public void Register(Action<IDamageable, float> listener)
    {
        _listeners += listener;
    }

    public void Unregister(Action<IDamageable, float> listener)
    {
        _listeners -= listener;
    }

    public void Raise(IDamageable target, float damage)
    {
        _listeners?.Invoke(target, damage);
    }
}
