using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class TransformExtensions
{
    public static T GetOrAddComponent<T>(this Transform trans) where T : Component
    {
        GameObject go = trans.gameObject;
        T obj = go.GetComponent<T>();
        if(obj == null)
		{
			obj = go.AddComponent<T>();
		}

		return obj;
    }

    public static Rect ToScreenRect(this RectTransform trans, Camera cam)
    {
        Vector3[] v = new Vector3[4];
        trans.GetWorldCorners(v);
        Vector2 leftBot = cam.WorldToScreenPoint(v[0]).ToVec2XY();
        Vector2 rightTop = cam.WorldToScreenPoint(v[2]).ToVec2XY();
        Rect finalRect = new Rect(leftBot, rightTop - leftBot);
        return finalRect;
    }
    
    public static void ResetLocals(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
    
    public static void CopyParams(this Transform trans, Transform copyFrom, bool withLocalScale = true)
    {
        trans.position = copyFrom.position;
        trans.rotation = copyFrom.rotation;
        if(withLocalScale)
		{
			trans.localScale = copyFrom.localScale;
		}
	}
}
