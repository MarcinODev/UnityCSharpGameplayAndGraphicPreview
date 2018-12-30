using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Factory of entities with pool mechanism
/// </summary>
public class EntityFactory
{
	private static Dictionary<string, Queue<Entity>> entityPool = new Dictionary<string, Queue<Entity>>();//<Prefab name, Queue of entities in pool>

	/// <summary>
	/// Instantiates or takes from pool the new Entity with set team and position
	/// </summary>
	/// <param name="prefab">Preafab of entity</param>
	/// <param name="team">Team Id</param>
	/// <param name="position">Position of spawn</param>
	/// <param name="mapConfig">MapConfig</param>
	/// <param name="gameplayManager">GameplayManager</param>
	/// <returns></returns>
	public static Entity CreateOrPullEntity(Entity prefab, int team, Vector3 position, MapConfig mapConfig, GameplayManager gameplayManager)
	{
		Entity entityFromPool = TryGetEntityFromPool(prefab, team, position);
		if(entityFromPool)
		{
			return entityFromPool;
		}

		Entity entity = GameObject.Instantiate<Entity>(prefab, position, Quaternion.identity);
		EntityController controller = AttachControllerToEntity(entity);
		controller.Initialize(entity, mapConfig, gameplayManager);
		entity.Initialize(controller, team);
		entity.PrefabName = prefab.name;
		
		return entity;
	}

	/// <summary>
	/// Should be executed when theres no longer need of keepeng old Entities 
	/// (like load some special scene without chance of quick going to gameplay, etc)
	/// </summary>
	public static void ClearPool()
	{
		foreach(var pair in entityPool)
		{
			foreach(Entity entity in pair.Value)
			{
				GameObject.DestroyImmediate(entity.gameObject);
			}
		}

		entityPool.Clear();
	}

	/// <summary>
	/// Addds Entity to pool and disables it. It's self called by entity on its death.
	/// </summary>
	/// <param name="entity">Entity which should be added to pool</param>
	public static void AddEntityToPool(Entity entity)
	{
		Queue<Entity> queue = null;
		if(!entityPool.TryGetValue(entity.PrefabName, out queue))
		{
			queue = new Queue<Entity>();
			entityPool[entity.PrefabName] = queue;
		}

		queue.Enqueue(entity);
		entity.gameObject.SetActive(false);
	}

	private static EntityController AttachControllerToEntity(Entity entity)
	{
		Type entityType = entity.GetType();
		if(entityType == typeof(Entity))
		{
			return entity.gameObject.AddComponent<EntityController>();
		}
		if(entityType == typeof(Mover))
		{
			return entity.gameObject.AddComponent<MoverController>();
		}

		return entity.gameObject.AddComponent<EntityController>();
	}

	private static Entity TryGetEntityFromPool(Entity prefab, int team, Vector3 position)
	{
		Queue<Entity> queue = null;
		if(!entityPool.TryGetValue(prefab.name, out queue) || queue.Count == 0)
		{
			return null;
		}

		Entity entity = queue.Dequeue();
		entity.Position = position;
		entity.gameObject.SetActive(true);
		entity.Respawn(team);
		return entity;
	}
}
