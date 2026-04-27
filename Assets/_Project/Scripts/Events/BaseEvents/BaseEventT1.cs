using System;
using UnityEngine;

public class BaseEventT1<T> : ScriptableObject
{
    private Action<T> _listeners;

    public void Register(Action<T> listener)
    {
        _listeners += listener;
    }

    public void Unregister(Action<T> listener)
    {
        _listeners -= listener;
    }

    public void Raise(T value)
    {
        _listeners?.Invoke(value);
    }
}
