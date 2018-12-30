using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class FloatExtensions
{
    public static string ToPercentageString(this float val)
    {
        return Mathf.FloorToInt(val * 100f) + "%";
    }

}

