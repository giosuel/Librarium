#region

using System;
using System.Collections.Generic;

#endregion

namespace Librarium.Binding;

/// <summary>
///     Binds a value and allows subscribers to register on change listeners.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ImpBinding<T> : IBinding<T>
{
    public event Action<T> OnUpdate;
    public event Action<T> OnUpdateSecondary;
    public event Action OnTrigger;
    public event Action OnTriggerSecondary;

    /// <summary>
    /// If this is set to true, calls to <see cref="Refresh"/> won't invoke any events.
    /// </summary>
    private readonly bool ignoreRefresh;

    public T DefaultValue { get; }

    public T Value { get; protected set; }

    public ImpBinding()
    {
    }

    public ImpBinding(
        T currentValue = default,
        T defaultValue = default,
        Action<T> primaryUpdate = null,
        Action<T> onUpdateSecondary = null,
        bool ignoreRefresh = false
    )
    {
        Value = currentValue;
        DefaultValue = !EqualityComparer<T>.Default.Equals(defaultValue, default)
            ? defaultValue
            : currentValue;

        this.ignoreRefresh = ignoreRefresh;

        OnUpdate += primaryUpdate;
        OnUpdateSecondary += onUpdateSecondary;
    }

    public virtual void Set(T updatedValue, bool invokePrimary = true, bool invokeSecondary = true)
    {
        var isSame = EqualityComparer<T>.Default.Equals(updatedValue, Value);
        Value = updatedValue;

        if (invokePrimary)
        {
            OnUpdate?.Invoke(Value);
            OnTrigger?.Invoke();
        }

        if (invokeSecondary && !isSame)
        {
            OnUpdateSecondary?.Invoke(updatedValue);
            OnTriggerSecondary?.Invoke();
        }
    }

    public virtual void Refresh()
    {
        if (!ignoreRefresh) Set(Value);
    }

    public void Reset(bool invokePrimary = true, bool invokeSecondary = true)
    {
        Set(DefaultValue, invokePrimary, invokeSecondary);
    }
}