#region

using System;

#endregion

namespace Librarium.Binding;

public class ImpEvent
{
    public event Action OnTrigger;

    public void Trigger() => OnTrigger?.Invoke();
}