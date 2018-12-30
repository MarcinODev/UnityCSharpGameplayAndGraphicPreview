using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic Entity controller.
/// Used for hit and collide reactions
/// </summary>
public class EntityController : MonoBehaviour
{
	private Entity entity;
	protected MapConfig mapConfig;
	protected GameplayManager gameplayManager;

	#region UnityMethods
	protected virtual void OnDestroy()
	{
		if(entity)
		{
			entity.onCollide -= OnCollide;
		}
	}
	#endregion

	public virtual void Initialize(Entity entity, MapConfig mapConfig, GameplayManager gameplayManager)
	{
		this.entity = entity;
		this.mapConfig = mapConfig;
		this.gameplayManager = gameplayManager;
		entity.onCollide += OnCollide;
	}

	public void HandleHit(float damage, Entity attacker)
	{
		if(entity.IsDead)
		{
			return;
		}

		entity.LostLife += damage;
		if(entity.Life <= 0f)
		{
			entity.Die();
		}
	}

	private void OnCollide(Entity entity, Entity entityColliding)
	{
		if(entityColliding.Team != entity.Team)
		{
			entityColliding.Controller.HandleHit(entity.DamageOnColide, entity);
		}
	}
}
