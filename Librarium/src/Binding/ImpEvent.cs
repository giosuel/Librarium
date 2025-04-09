#region

using System;

#endregion

namespace Librarium.Binding;

public class ImpEvent
{
    public event Action onTrigger;

    public void Trigger() => onTrigger?.Invoke();
}