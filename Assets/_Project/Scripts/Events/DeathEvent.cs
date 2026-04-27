using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New DeathEvent", menuName = "ScriptableObjects/Events/Combat/DeathEvent")]
public class DeathEvent : ScriptableObject
{
    private Action<IDamageable> _listeners;

    public void Register(Action<IDamageable> listener)
    {
        _listeners += listener;
    }

    public void Unregister(Action<IDamageable> listener)
    {
        _listeners -= listener;
    }

    public void Raise(IDamageable target)
    {
        _listeners?.Invoke(target);
    }
}
