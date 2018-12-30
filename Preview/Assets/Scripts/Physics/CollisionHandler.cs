using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handler of collisions
/// </summary>
public class CollisionHandler : MonoBehaviour
{
	public event Action<Collision, CollisionHandler> onColide;
	public event Action<Collider, CollisionHandler> onTrigger;
	
	protected void OnCollisionEnter(Collision collision)
	{
		if(onColide != null)
			onColide(collision, this);
	}

	protected void OnTriggerEnter(Collider collider)
	{
		if(onTrigger != null)
			onTrigger(collider, this);
	}
}
