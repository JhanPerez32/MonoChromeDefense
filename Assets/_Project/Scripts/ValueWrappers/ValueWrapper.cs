using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValueWrapper<T> : ScriptableObject
{
    [Header("Default Value")]
    [SerializeField] protected T value;

    private T _runtimeValue;
    private bool _isInitialized;

    public event Action<T> OnValueChanged;

    public T Value
    {
        get
        {
            if (_isInitialized)
            {
                return _runtimeValue;
            }
            
            _runtimeValue = value;
            _isInitialized = true;

            return _runtimeValue;
        }
        set
        {
            if (!_isInitialized)
            {
                _runtimeValue = this.value;
                _isInitialized = true;
            }

            // Safe equality check
            if (EqualityComparer<T>.Default.Equals(_runtimeValue, value)) return;

            _runtimeValue = value;
            OnValueChanged?.Invoke(_runtimeValue);
        }
    }

    /// <summary>
    /// Direct access to serialized value (Editor-safe, no runtime logic)
    /// </summary>
    public T GetRawValue()
    {
        return value;
    }

    /// <summary>
    /// Explicit setter (cleaner usage)
    /// </summary>
    public void SetValue(T newValue)
    {
        Value = newValue;
    }

    /// <summary>
    /// Force event trigger without changing value
    /// </summary>
    public void ForceNotify()
    {
        OnValueChanged?.Invoke(Value);
    }

    /// <summary>
    /// Reset runtime value back to default
    /// </summary>
    public void ResetValue()
    {
        _runtimeValue = value;
        _isInitialized = true;
        OnValueChanged?.Invoke(_runtimeValue);
    }

    /// <summary>
    /// Allows setting value via object (useful for generic systems)
    /// </summary>
    public void SetObjectValue(object newValue)
    {
        if (newValue is T castValue)
        {
            Value = castValue;
        }
        else
        {
            Debug.LogWarning($"[ValueWrapper<{typeof(T).Name}>] Tried to assign {newValue?.GetType()} to {typeof(T)}");
        }
    }

    protected virtual void OnEnable()
    {
        //Reset on entering play mode / domain reload
        _isInitialized = false;
    }
}
