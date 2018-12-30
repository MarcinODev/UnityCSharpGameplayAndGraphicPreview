using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T obj = go.GetComponent<T>();
        if(obj == null)
            obj = go.AddComponent<T>();
        return obj;
    }
}
