using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class CameraExtensions
{
    public static bool IsPointVisible(this Camera camera, Vector3 worldPos)
    {
		Vector3 viewportPoint = camera.WorldToViewportPoint(worldPos);
		return viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
	}
}

