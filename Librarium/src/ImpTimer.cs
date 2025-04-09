#region

using UnityEngine;

#endregion

namespace Librarium;

public class ImpTimer
{
    private float initialTime;
    private float countdown;

    private ImpTimer()
    {
    }

    public static ImpTimer ForInterval(float seconds)
    {
        var timer = new ImpTimer
        {
            initialTime = seconds,
            countdown = seconds
        };
        return timer;
    }

    public void Set(float newTime)
    {
        initialTime = newTime;
    }

    public bool Tick()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0)
        {
            countdown = initialTime;
            return true;
        }

        return false;
    }

    public void Reset() => countdown = initialTime;
}