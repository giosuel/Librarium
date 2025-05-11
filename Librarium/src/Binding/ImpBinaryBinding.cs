#region

using System;
using System.Linq;

#endregion

namespace Librarium.Binding;

public class ImpBinaryBinding : ImpBinding<bool>
{
    public event Action OnTrue;
    public event Action OnFalse;

    public ImpBinaryBinding(
        bool currentValue,
        Action onTrue = null,
        Action onFalse = null
    ) : base(currentValue)
    {
        OnTrue += onTrue;
        OnFalse += onFalse;

        OnUpdate += OnBaseUpdate;
    }

    public void Toggle() => Set(!Value);
    public void SetTrue() => Set(true);
    public void SetFalse() => Set(false);

    private void OnBaseUpdate(bool updatedValue)
    {
        if (updatedValue)
        {
            OnTrue?.Invoke();
        }
        else
        {
            OnFalse?.Invoke();
        }
    }

    public static bool operator true(ImpBinaryBinding obj) => obj.Value;
    public static bool operator false(ImpBinaryBinding obj) => !obj.Value;
    public static bool operator !(ImpBinaryBinding obj) => !obj.Value;

    /// <summary>
    /// Creates a combined binary binding from two or more provided bindings.
    ///
    /// The new binding's value is true if any of the source bindings' values is true.
    /// </summary>
    /// <param name="bindingPairs">A list of bindings to combine</param>
    public static ImpBinaryBinding CreateAnd((IBinding<bool>, bool)[] bindingPairs)
    {
        var combinedBinding = new ImpBinaryBinding(GetCombinedAndValue(bindingPairs));
        foreach (var bindingPair in bindingPairs)
        {
            bindingPair.Item1.OnTrigger += () => combinedBinding.Set(GetCombinedAndValue(bindingPairs));
        }

        return combinedBinding;

        bool GetCombinedAndValue((IBinding<bool>, bool)[] pairs) => pairs.Aggregate(
            true, (combined, current) => combined && current.Item2 ^ current.Item1.Value
        );
    }

    /// <summary>
    /// Creates a combined binary binding from two or more provided bindings.
    ///
    /// The new binding's value is true if all the source bindings' values are true.
    /// </summary>
    /// <param name="bindingPairs">A list of bindings to combine</param>
    public static ImpBinaryBinding CreateOr((IBinding<bool>, bool)[] bindingPairs)
    {
        var combinedBinding = new ImpBinaryBinding(GetCombinedOrValue(bindingPairs));
        foreach (var bindingPair in bindingPairs)
        {
            bindingPair.Item1.OnTrigger += () => combinedBinding.Set(GetCombinedOrValue(bindingPairs));
        }

        return combinedBinding;

        bool GetCombinedOrValue((IBinding<bool>, bool)[] pairs) => pairs.Aggregate(
            false, (combined, current) => combined || current.Item2 ^ current.Item1.Value
        );
    }
}