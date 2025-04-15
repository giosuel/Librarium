#region

using System;
using System.Collections.Generic;

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

    public override T Value
    {
        get => Utils.InvokeDefaultOnNull(valueGetter);
        protected set => this.value = value;
    }

    /// <param name="valueGetter">Getter function that returns the value</param>
    /// <param name="refresher">ImpBinding that the binder is listening to</param>
    /// <param name="onPrimaryUpdate">
    ///     <see cref="ImpBinding{T}.onUpdate" />
    /// </param>
    /// <param name="secondaryUpdate">
    ///     <see cref="ImpBinding{T}.onUpdateSecondary" />
    /// </param>
    public ImpExternalBinding(
        Func<T> valueGetter,
        IBinding<R> refresher = null,
        Action<T> onPrimaryUpdate = null,
        Action<T> secondaryUpdate = null
    ) : base(Utils.InvokeDefaultOnNull(valueGetter), primaryUpdate: onPrimaryUpdate, onUpdateSecondary: secondaryUpdate)
    {
        this.valueGetter = valueGetter;
        if (refresher != null) refresher.onUpdate += _ => Set(Utils.InvokeDefaultOnNull(valueGetter));
    }

    public override void Set(T updatedValue, bool invokePrimary = true, bool invokeSecondary = true)
    {
        base.Set(Utils.InvokeDefaultOnNull(valueGetter), invokePrimary, invokeSecondary);
    }
}