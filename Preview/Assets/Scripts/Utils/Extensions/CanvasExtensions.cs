using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class CanvasExtensions
{
    public static Vector2 WorldToCanvasAnchorPos(this Canvas canvas, Vector3 pos, Camera worldCamera)
    {
        Vector2 viewportPoint = worldCamera.WorldToViewportPoint(pos);  
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
 
        return new Vector2(viewportPoint.x * canvasRect.sizeDelta.x - canvasRect.sizeDelta.x * 0.5f,
                            viewportPoint.y * canvasRect.sizeDelta.y - canvasRect.sizeDelta.y * 0.5f);
    }
    
    public static Vector3 CanvasAnchorPointToWorld(this Canvas canvas, Vector2 anchorPos, Camera worldCamera)
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 viewportPoint = new Vector2((anchorPos.x + canvasRect.sizeDelta.x * 0.5f) / canvasRect.sizeDelta.x,
                                            (anchorPos.y + canvasRect.sizeDelta.y * 0.5f) / canvasRect.sizeDelta.y);
        Vector3 pos = worldCamera.ViewportToWorldPoint(viewportPoint);  
        return pos;
    }
}

