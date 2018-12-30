using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Emitter for basic Entity effects
/// </summary>
[RequireComponent(typeof(Entity))]
public class EntityEffectEmmiter : MonoBehaviour
{
	[SerializeField] private Transform onDeathEffect;
	[SerializeField] private Transform onSpawnEffect;
	[SerializeField] private Transform onTeleportEffectStart;
	[SerializeField] private Transform onTeleportEffectEnd;
	[SerializeField] private Transform onCollideEffect;
	[SerializeField] private string globalShaderPropertyNameForPosition;//used to set global information about certain object's position
	private Entity entity;

	#region UnityMethods
	protected void Awake()
	{
		entity = GetComponent<Entity>();
		entity.onDeath += OnDeath;
		entity.onRespawn += OnRespawn;
		entity.onTeleport += OnTeleport;
		entity.onCollide += OnCollide;
		if(globalShaderPropertyNameForPosition.IsNotNullOrEmpty())
		{
			entity.onPositionChanged += OnPositionChanged;
		}
	}

	protected void Start()
	{
		OnRespawn(entity);
		PoolManager.AllocInPool(onTeleportEffectStart);
		PoolManager.AllocInPool(onTeleportEffectEnd);
		PoolManager.AllocInPool(onSpawnEffect);
		PoolManager.AllocInPool(onDeathEffect);
		PoolManager.AllocInPool(onCollideEffect);
	}
	#endregion

	private void OnTeleport(Entity entity, Vector3 before, Vector3 after)
	{
		if(onTeleportEffectStart != null)
		{
			PoolManager.CreateFromPool(onTeleportEffectStart, before, Quaternion.identity, PoolManager.Trans);
		}

		if(onTeleportEffectEnd != null)
		{
			PoolManager.CreateFromPool(onTeleportEffectEnd, after, Quaternion.identity, PoolManager.Trans);
		}
	}

	private void OnRespawn(Entity entity)
	{
		if(onSpawnEffect != null)
		{
			PoolManager.CreateFromPool(onSpawnEffect, entity.Position, Quaternion.identity, PoolManager.Trans);
		}
	}

	private void OnDeath(Entity entity)
	{
		if(onDeathEffect != null)
		{
			PoolManager.CreateFromPool(onDeathEffect, entity.Position, Quaternion.identity, PoolManager.Trans);
		}
	}

	private void OnCollide(Entity entity, Entity entityColliding)
	{
		if(onCollideEffect != null)
		{
			PoolManager.CreateFromPool(onCollideEffect, entity.Position, Quaternion.identity, PoolManager.Trans);
		}
	}

	private void OnPositionChanged(Entity entity, Vector3 before, Vector3 after)
	{
		if(globalShaderPropertyNameForPosition.IsNotNullOrEmpty())
		{
			Shader.SetGlobalVector(globalShaderPropertyNameForPosition, after);
		}
	}
}
