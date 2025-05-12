using System;
using System.Globalization;
using UnityEngine;

namespace Librarium;

public static class Formatting
{
    /// <summary>
    ///     Creates a formatted string from a unity <see cref="Vector3" />.
    ///     If a unit is provided, the unit will be appended to each scalar.
    ///     Format: "(x[unit](separator)y[unit](separator)z[unit])"
    /// </summary>
    /// <param name="input"></param>
    /// <param name="roundDigits">To how many digits the scalars should be rounded</param>
    /// <param name="separator">Scalar separator</param>
    /// <param name="unit">Optional scalar unit</param>
    /// <returns></returns>
    public static string FormatVector(
        Vector3 input,
        int roundDigits = 1,
        string separator = "/",
        string unit = ""
    )
    {
        var x = roundDigits > -1 ? MathF.Round(input.x, roundDigits) : input.x;
        var y = roundDigits > -1 ? MathF.Round(input.y, roundDigits) : input.y;
        var z = roundDigits > -1 ? MathF.Round(input.z, roundDigits) : input.z;
        return $"({x}{unit}{separator}{y}{unit}{separator}{z}{unit})";
    }

    /// <summary>
    ///     Limits a float to 3 digits.
    ///     e.g. 100.01 => 100, 14.23 => 12.3, 1.22 => 1.22, 0.1 => 0.1
    ///     Note: This only works for positive numbers smaller than 999
    /// </summary>
    public static string FormatFloatToThreeDigits(float value) =>
        value switch
        {
            >= 100 => Mathf.RoundToInt(value).ToString(),
            >= 10 => MathF.Round(value, 1).ToString(CultureInfo.InvariantCulture),
            _ => MathF.Round(value, 2).ToString(CultureInfo.InvariantCulture)
        };

    /// <summary>
    ///     Formats a second value to minutes and seconds. E.g. 1m 15s or 45s or 5m
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    public static string FormatSecondsMinutes(float seconds)
    {
        if (seconds < 60) return $"{seconds:0}s";
        return seconds % 60 == 0 ? $"{seconds / 60:0}m" : $"{(int)seconds / 60}m {(int)seconds % 60}s";
    }
}