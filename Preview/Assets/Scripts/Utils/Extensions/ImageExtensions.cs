using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class ImageExtensions
{
    public static void ReinstantiateMaterial(this Image img)
    {
        img.material = new Material(img.material);
    }
}

