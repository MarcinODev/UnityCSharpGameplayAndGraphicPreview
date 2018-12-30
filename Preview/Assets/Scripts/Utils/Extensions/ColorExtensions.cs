using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class ColorExtensions
{
    public static Vector3 ToRGBVec(this Color col)
    {
        return new Vector3(col.r, col.g, col.b);
    }

    public static Vector4 ToRGBAVec(this Color col)
    {
        return new Vector4(col.r, col.g, col.b, col.a);
    }

    public static Color ChangeAlpha(this Color col, float alpha)
    {
        col.a = alpha;
        return col;
    }
}

