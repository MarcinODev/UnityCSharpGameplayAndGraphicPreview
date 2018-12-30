using UnityEngine;
using System.Collections;

/// <summary>
/// Class for using in PoolManager - best for poolable effects, animations and other objects
/// </summary>
public class Poolable : MonoBehaviour 
{
    public void Release()
    {
        PoolManager.ReleaseIntoPool(transform);
    }

    public void ReleaseAfterTime(float time)
    {
        StartCoroutine(ReleaseAfterTimeCoroutine(time));
    }

    protected virtual IEnumerator ReleaseAfterTimeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Release();
    }

    public void Release(object obj)
    {
        Release();
    }

}
