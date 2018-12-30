using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class IntExtensions
{
    /// <summary>
    /// Changes value to string range with spliting by step
    /// </summary>
    /// <returns>string looking like "10-20", "100-150". With square brackets: [10-20]</returns>
    public static string ToRangeString(this int val, int step, bool useSquareBrackets = false)
    {
        int minVal = (val / step) * step;
        int maxVal = minVal + step;
        if(useSquareBrackets)
            return "[{0}-{1}]".SetArgs(minVal, maxVal);
        else
            return "{0}-{1}".SetArgs(minVal, maxVal);
    }

}

