using UnityEngine;
using System.Collections;

/// <summary>
/// Self destructing poolable object
/// </summary>
public class AutoDestroy : Poolable
{
    public float lifeTime = 1f;
    public bool dontKeepInPool;
    
    private float startTime;

    void OnEnable()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if(startTime + lifeTime <= Time.time)
        {
            if(dontKeepInPool)
            {
                Destroy(gameObject);
            }
            else
            {
                Release();
            }
        }
    }
}
