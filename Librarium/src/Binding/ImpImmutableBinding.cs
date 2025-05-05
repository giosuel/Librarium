#region

using System;

#endregion

namespace Librarium.Binding;

/// <summary>
///     A ImpBinding that can eb subscribed to, but cannot be updated.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ImpImmutableBinding<T> : IBinding<T>
{
    private readonly IBinding<T> parentWritableBinding;

    public T Value => parentWritableBinding.Value;
    public T DefaultValue => parentWritableBinding.DefaultValue;

    public event Action<T> OnUpdate;
    public event Action OnTrigger;

    public event Action<T> OnUpdateSecondary;
    public event Action OnTriggerSecondary;

    public static ImpImmutableBinding<T> Wrap(IBinding<T> parent) => new(parent);

    private ImpImmutableBinding(IBinding<T> parentWritableBinding)
    {
        this.parentWritableBinding = parentWritableBinding;
        parentWritableBinding.OnUpdate += value => OnUpdate?.Invoke(value);
        parentWritableBinding.OnTrigger += () => OnTrigger?.Invoke();

        parentWritableBinding.OnUpdateSecondary += value => OnUpdateSecondary?.Invoke(value);
        parentWritableBinding.OnTriggerSecondary += () => OnTriggerSecondary?.Invoke();
    }

    public void Set(T updatedValue, bool invokePrimary = true, bool invokeSecondary = true)
    {
    }

    public void Refresh()
    {
    }

    public void Reset(bool invokePrimary, bool invokeSecondary)
    {
    }
}