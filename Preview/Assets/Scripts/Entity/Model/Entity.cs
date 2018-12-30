using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic entity class. Contains information about object position, life, radius, etc.
/// </summary>
public class Entity : MonoBehaviour
{
	public delegate void OnEntityCollide(Entity entity, Entity entityColliding);
	public delegate void OnPositionChange(Entity entity, Vector3 before, Vector3 after);

	public event Action<Entity> onDeath;//self clears on destroy
	public event Action<Entity> onRespawn;//self clears on destroy
	public event OnEntityCollide onCollide;//self clears on destroy
	public event OnPositionChange onTeleport;//self clears on destroy
	public event OnPositionChange onPositionChanged;//self clears on destroy

	[SerializeField] private float maxLife = 1;
	[SerializeField] private float damageOnColide = 1;
	[SerializeField] private CollisionHandler collisionHandler;
	[SerializeField] private Rigidbody rootRigidbody;
	[SerializeField] private float timeToHideAfterDeath = 1f;
	[SerializeField] private float radius = 1f;
	private float lostLife;

	#region UnityMethods
	protected void Awake()
	{
		Trans = transform;
	}

	protected void OnDestroy()
	{
		if(collisionHandler)
		{
			collisionHandler.onTrigger -= OnTriggerCollision;
		}

		onDeath = null;
		onCollide = null;
		onTeleport = null;
		onRespawn = null;
		onPositionChanged = null;
	}
	#endregion

	public virtual void Initialize(EntityController controller, int team)
	{
		Team = team;
		LostLife = 0;
		Controller = controller;
		if(collisionHandler)
		{
			collisionHandler.onTrigger += OnTriggerCollision;
		}
	}

	public virtual void Respawn(int team)
	{
		LostLife = 0;
		IsDead = false;
		Team = team;

		if(onRespawn != null)
		{
			onRespawn(this);
		}
	}

	public void Teleport(Vector3 pos)
	{
		if(onTeleport != null)
		{
			onTeleport(this, Position, pos);
		}

		Position = pos;
	}

	/// <summary>
	/// Only controller can mark entity as dead. Life = 0 doesn't mean that entity is dead
	/// </summary>
	public void Die()
	{
		IsDead = true;
		LostLife = maxLife;
		if(onDeath != null)
		{
			onDeath(this);
		}

		StartCoroutine(PostDeathCoroutine());
	}

	private IEnumerator PostDeathCoroutine()
	{
		yield return new WaitForSeconds(timeToHideAfterDeath);

		EntityFactory.AddEntityToPool(this);//this will also disable current gameObject
	}

	protected void OnTriggerCollision(Collider collider, CollisionHandler coliHandler)
	{
		Entity collidingEntity = collider.GetComponentInParent<Entity>();
		if(collidingEntity == null || collidingEntity.IsDead)
		{
			return;
		}

		if(onCollide != null)
		{
			onCollide(this, collidingEntity);
		}
	}

	public EntityController Controller { get; private set; }
	public Transform Trans { get; private set; }
	public int Team { get; private set; }
	public bool IsDead { get; private set; }
	public string PrefabName { get; set; }

	public float Life
	{
		get
		{
			return maxLife - LostLife;
		}
	}

	public float LostLife
	{
		get
		{
			return lostLife;
		}

		set
		{
			if(lostLife == value)
			{
				return;
			}

			lostLife = value;
			if(lostLife > maxLife)
			{
				lostLife = maxLife;
			}
		}
	}

	public Vector3 Position
	{
		get
		{
			return Trans.position;
		}
		set
		{
			Vector3 oldPos = Trans.position;
			Trans.position = value;
			if(onPositionChanged != null)
			{
				onPositionChanged(this, oldPos, value);
			}
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return Trans.rotation;
		}
		set
		{
			Trans.rotation = value;
		}
	}

	public float DamageOnColide
	{
		get
		{
			return damageOnColide;
		}
	}

	public Rigidbody RootRigidbody
	{
		get
		{
			return rootRigidbody;
		}
	}

	public float Radius
	{
		get
		{
			return radius;
		}
	}
}
