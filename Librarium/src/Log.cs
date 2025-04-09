using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Librarium;

public static class Log
{
    public static string LogBlock(List<string> lines, string title)
    {
        title = "< " + title + " >";
        // Limit width to 518 so lines can be clamped to 512 and an ellipse can be added
        var width = Mathf.Min(lines.Max(line => line.Length) + 4, 520);
        var fullWidth = string.Concat(Enumerable.Repeat("\u2550", width - 2));
        var titlePaddingCount = (width - title.Length) / 2 - 1;
        if ((width - title.Length) / 2 % 2 == 0) titlePaddingCount++;

        var titlePadding = string.Concat(Enumerable.Repeat(" ", titlePaddingCount));

        var output = "\u2552" + fullWidth + "\u2555\n";
        output += "\u2502" + titlePadding + title + titlePadding + "\u2502\n";
        output += "\u255e" + fullWidth + "\u2561\n";
        output = lines.Aggregate(
            output,
            (current, line) =>
            {
                var clampedLine = line.Length > 512 ? line[..512] + "..." : line;
                return current + $"\u2502 {clampedLine}".PadRight(width - 2) + " \u2502\n";
            }
        );
        output += "\u2558" + fullWidth + "\u255b";

        return output.Split("\n").Aggregate("", (current, se) => current + se.Trim());
    }
}