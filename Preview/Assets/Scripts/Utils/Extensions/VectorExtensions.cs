using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class VectorExtensions
{
	#region VEC2
	public static Vector3 ToVec3XZ(this Vector2 vec)
    {
        return new Vector3(vec.x, 0f, vec.y);
    }

    public static Vector3 ToVec3XY(this Vector2 vec)
    {
        return new Vector3(vec.x, vec.y, 0f);
    }

    public static Vector3 ToVec3XZ(this Vector2 vec, float yToSet)
    {
        return new Vector3(vec.x, yToSet, vec.y);
    }

    public static Vector2 Div(this Vector2 vec, Vector2 div)
    {
        vec.x /= div.x;
        vec.y /= div.y;
        return vec;
    }    

    public static Vector2 Mul(this Vector2 vec, Vector2 multiplier)
    {
        vec.x *= multiplier.x;
        vec.y *= multiplier.y;
        return vec;
    }    

    public static Vector2 Max(this Vector2 vec, Vector2 compVec)
    {
        if(vec.x < compVec.x)
            vec.x = compVec.x;
        if(vec.y < compVec.y)
            vec.y = compVec.y;        
        return vec;
    }    

    public static Vector2 Min(this Vector2 vec, Vector2 compVec)
    {
        if(vec.x > compVec.x)
            vec.x = compVec.x;
        if(vec.y > compVec.y)
            vec.y = compVec.y;        
        return vec;
    }
	#endregion

	#region VEC3
	public static float SqrMagnitudeXZ(this Vector3 vec)
    {
        return vec.x * vec.x + vec.z * vec.z; 
    }

    public static float SqrMagnitudeXY(this Vector3 vec)
    {
        return vec.x * vec.x + vec.y * vec.y; 
    }

    public static float MagnitudeXZ(this Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.z * vec.z); 
    }

    public static float MagnitudeXY(this Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y); 
    }
    
    public static Vector2 ToVec2XZ(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.z);
    }
    
    public static Vector2 ToVec2XY(this Vector3 vec)
    {
        return new Vector2(vec.x, vec.y);
    }
    
    public static Vector3 ToVec3SetY(this Vector3 vec, float y)
    {
        vec.y = y;
        return vec;
    }
    
    public static Vector3 ToVec3SetZ(this Vector3 vec, float z)
    {
        vec.z = z;
        return vec;
    }

    public static Vector3 Mul(this Vector3 vec, Vector3 mul)
    {
        vec.x *= mul.x;
        vec.y *= mul.y;
        vec.z *= mul.z;
        return vec;
    }
    
    public static Vector3 Div(this Vector3 vec, Vector3 div)
    {
        vec.x /= div.x;
        vec.y /= div.y;
        vec.z /= div.z;
        return vec;
    }

    public static Vector3 WorldPosToAnotherCamWorldPos(this Vector3 pos, Camera oldCam, Camera newCam)
    {
        Vector3 screenPos = oldCam.WorldToScreenPoint(pos);
        return newCam.ScreenToWorldPoint(screenPos);
    }
	#endregion

	#region VEC4
	public static Color ToColor(this Vector4 vec)
    {
        return new Color(vec.x, vec.y, vec.z, vec.w);
	}
	#endregion

	#region GUI
	public static Vector2 WorldToGUIAnchoredPosition(this Vector3 worldPos, Camera worldCamera, RectTransform canvasRect)
    {
        Vector2 viewportPosition = worldCamera.WorldToViewportPoint(worldPos);
        Vector2 anchoredPos = new Vector2(
                                (viewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f),
                                (viewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
        return anchoredPos;
    }
    
    public static Vector2 WorldToGUIAnchoredPosition(this Vector3 worldPos, Camera worldCamera, Canvas canvas)
    {
        return worldPos.WorldToGUIAnchoredPosition(worldCamera, canvas.GetComponent<RectTransform>());
    }
    
    public static Vector3 GUIAnchoredPositionToWorld(this Vector2 canvasPos, Camera worldCamera, Canvas canvas)
    {
        return canvasPos.GUIAnchoredPositionToWorld(worldCamera, canvas.GetComponent<RectTransform>());
    }

    public static Vector3 GUIAnchoredPositionToWorld(this Vector2 canvasPos, Camera worldCamera, RectTransform canvasRect)
    {
        canvasPos.x = (canvasPos.x + canvasRect.sizeDelta.x * 0.5f) / canvasRect.sizeDelta.x;
        canvasPos.y = (canvasPos.y + canvasRect.sizeDelta.y * 0.5f) / canvasRect.sizeDelta.y;
        return worldCamera.ViewportToWorldPoint(canvasPos);
    }

    public static Vector2 GUIAnchoredPositionToCanvas(this Vector2 canvasPos, Canvas oldCanvas, Canvas newCanvas)
    {
        return canvasPos.GUIAnchoredPositionToCanvas(oldCanvas.GetComponent<RectTransform>(), newCanvas.GetComponent<RectTransform>());
    }

    public static Vector2 GUIAnchoredPositionToCanvas(this Vector2 canvasPos, RectTransform oldCanvasRect, RectTransform newCanvasRect)
    {
        canvasPos.x = (canvasPos.x + oldCanvasRect.sizeDelta.x * 0.5f) / oldCanvasRect.sizeDelta.x;
        canvasPos.y = (canvasPos.y + oldCanvasRect.sizeDelta.y * 0.5f) / oldCanvasRect.sizeDelta.y;
        
        canvasPos.x = (canvasPos.x * newCanvasRect.sizeDelta.x) - (newCanvasRect.sizeDelta.x * 0.5f);
        canvasPos.y = (canvasPos.y * newCanvasRect.sizeDelta.y) - (newCanvasRect.sizeDelta.y * 0.5f);
        return canvasPos;
	}
	#endregion
}

