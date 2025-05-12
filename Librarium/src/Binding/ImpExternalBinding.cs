#region

using System;

#endregion

namespace Librarium.Binding;

/// <summary>
///     Intermediate binding for when the value can't be easily bound due to external changes
///     Whenever the refresher binding is updated, the child refreshes it's value based on the provided getter function.
/// </summary>
/// <typeparam name="T">Type of the binding and getter return value</typeparam>
/// <typeparam name="R">Type of the parent binding</typeparam>
public class ImpExternalBinding<T, R> : ImpBinding<T>
{
    private readonly Func<T> valueGetter;

    /// <param name="valueGetter">Getter function that returns the value</param>
    /// <param name="refresher">ImpBinding that the binder is listening to</param>
    /// <param name="onPrimaryUpdate">
    ///     <see cref="ImpBinding{T}.OnUpdate" />
    /// </param>
    /// <param name="secondaryUpdate">
    ///     <see cref="ImpBinding{T}.OnUpdateSecondary" />
    /// </param>
    public ImpExternalBinding(
        Func<T> valueGetter,
        IBinding<R> refresher = null,
        Action<T> onPrimaryUpdate = null,
        Action<T> secondaryUpdate = null
    ) : base(Utils.InvokeOrDefault(valueGetter), primaryUpdate: onPrimaryUpdate, onUpdateSecondary: secondaryUpdate)
    {
        this.valueGetter = valueGetter;

        if (refresher != null) refresher.OnUpdate += _ => Set(Utils.InvokeOrDefault(valueGetter));
    }

    public override void Refresh()
    {
        base.Set(Utils.InvokeOrDefault(valueGetter));
    }
}