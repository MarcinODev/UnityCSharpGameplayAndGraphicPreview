using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Manager for spawning effects and other poolable (or just transform) objects
/// </summary>
public class PoolManager : MonoBehaviour
{
	private Dictionary<string, Queue<Transform>> pool = new Dictionary<string, Queue<Transform>>();

	protected void Awake()
	{
		if(Instance != null)
		{
			DestroyImmediate(Instance.gameObject);
		}
		DontDestroyOnLoad(gameObject);
		Instance = this;
		Trans = transform;
    }
    
    public static void ReleaseIntoPool(Transform obj)
    {
        if(Instance == null)
        {
            Destroy(obj.gameObject);
            return;
        }

        obj.name = obj.name.Replace("(Clone)", "");
        Queue<Transform> queue = GetPoolQueue(obj.name, true);
        obj.gameObject.SetActive(false);
        obj.parent = Trans;
        queue.Enqueue(obj);
    }

	/// <summary>
	/// Good for preloading effects to get rid of load lag
	/// </summary>
	public static void AllocInPool(Transform prefab)
	{
		if(prefab == null)
		{
			return;
		}

		Queue<Transform> queue = GetPoolQueue(prefab.name);
		if(queue == null || queue.Count == 0)
		{
			Transform obj = CreateFromPool(prefab, Trans.position, Trans.rotation, Trans);
			ReleaseIntoPool(obj);
		}
	}
	
	/// <summary>
	/// Instantiates or enables and returns cached object which was made from selected prefab
	/// </summary>
    public static Transform CreateFromPool(Transform prefab, Vector3 pos, Quaternion rotation, Transform parent)
    {
        Transform output = DequeuePooledPrefab(prefab);
        if(output == null)
        {
            output = GameObject.Instantiate(prefab, pos, rotation, parent) as Transform;
        }
        else
        {
            output.position = pos;
            output.rotation = rotation;
            output.parent = parent;
            output.gameObject.SetActive(true);
        }
        return output;
    }

	/// <summary>
	/// Instantiates or enables and returns cached object which was made from selected prefab
	/// </summary>
	public static T CreateFromPool<T>(T prefab, Vector3 pos, Quaternion rotation, Transform parent) where T : MonoBehaviour
    {
        Transform oldTransform = DequeuePooledPrefab(prefab.transform);
        T output = null;
        if(oldTransform == null)
        {
            output = GameObject.Instantiate(prefab, pos, rotation, parent) as T;
        }
        else
        {
            oldTransform.position = pos;
            oldTransform.rotation = rotation;
            oldTransform.parent = parent;
            oldTransform.gameObject.SetActive(true);
            output = oldTransform.GetComponent<T>();
        }
        return output;
	}

	private static Queue<Transform> GetPoolQueue(string name, bool addMissingQueue = false)
	{
		Queue<Transform> queue = null;
		if(!Instance.pool.TryGetValue(name, out queue))
		{
			if(addMissingQueue)
			{
				Instance.pool[name] = queue = new Queue<Transform>();
			}
			else
			{
				return null;
			}
		}
		return queue;
	}

	private static Transform DequeuePooledPrefab(Transform prefab)
	{
		Queue<Transform> queue = GetPoolQueue(prefab.name);
		if(queue == null || queue.Count == 0)
			return null;
		return queue.Dequeue().transform;
	}

	public static Transform Trans { get; private set; }
	public static PoolManager Instance { get; private set; }
}
