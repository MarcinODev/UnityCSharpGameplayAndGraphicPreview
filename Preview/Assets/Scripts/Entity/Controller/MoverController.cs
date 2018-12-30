using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mover controller, support movement, in-bounds movement and terrain collisions
/// </summary>
public class MoverController : EntityController
{
	private const float bounceMoveDirFromTerrain = 1f;//to change move dir after hit the terrain as a temporary bounce
	private static readonly Vector3 zeroVector = Vector3.zero;//because Vector3.zero always create new object
	private Mover mover;

	#region UnityMethods
	protected void FixedUpdate()//to match physics updates of moves
	{
		if(mover.IsDead)
		{
			return;
		}

		UpdateMovement();

		UpdateMapBoundingsColisions();

		if(mover.IsDead)
		{
			return;
		}

		if(mover.DamageOnHitTerrain)
		{
			UpdateHitTerrain();
		}

		if(mover.KeepAlwaysAboveTerrain)
		{
			UpdateKeepAboveTerrain();
		}
	}
	#endregion

	public override void Initialize(Entity entity, MapConfig mapConfig, GameplayManager gameplayManager)
	{
		base.Initialize(entity, mapConfig, gameplayManager);
		mover = (Mover)entity;
	}

	private void UpdateMovement()
	{
		if(mover.MoveDir == zeroVector)
		{
			Vector3 rotation = mover.Rotation.eulerAngles;
			if(rotation.z != 0f)
			{
				rotation.z = 0;
				mover.Rotation = Quaternion.Euler(rotation);
			}
			return;
		}

		float moveSpeed = mover.MoveSpeed * Time.fixedDeltaTime;
		Vector3 moveVec = moveSpeed * mover.MoveDir;
		mover.Position += moveVec;		

		Vector3 eulerRot = mover.Rotation.eulerAngles;
		eulerRot.z = mover.MoveDir.x * -mover.MaxZRotation;
		mover.Rotation = Quaternion.Euler(eulerRot);
	}

	private void UpdateMapBoundingsColisions()
	{
		if(!mapConfig.mapWorldBoundings.Contains(mover.Position.ToVec2XZ())
			|| mover.Position.y > LimitForTopYPos)//is object out of game view
		{
			if(mover.LockMovementWithinMapBorder)
			{
				LockMoverToMapBoundings();
			}
			else
			{
				mover.Die();
			}
		}
	}

	private void LockMoverToMapBoundings()
	{
		Rect boundings = mapConfig.mapWorldBoundings;
		Vector3 position = mover.Position;
		if(boundings.xMin > position.x)
		{
			position.x = boundings.xMin;
		}
		if(boundings.yMin > position.z)
		{
			position.z = boundings.yMin;
		}
		if(boundings.xMax < position.x)
		{
			position.x = boundings.xMax;
		}
		if(boundings.yMax < position.z)
		{
			position.z = boundings.yMax;
		}

		if(LimitForTopYPos < position.y)
		{
			position.y = LimitForTopYPos;
		}

		mover.Position = position;
	}

	private void UpdateHitTerrain()
	{
		float terrainY = gameplayManager.TerrainController.GetTerrainYPos(mover.Position.x, mover.Position.z);
		//Debug.DrawLine(new Vector3(mover.Position.x, terrainY, mover.Position.z), mover.Position, Color.green, 0.1f);
		if(terrainY + mover.Radius > mover.Position.y)
		{
			HandleHit(mover.DamageOnColide, mover);
			if(!mover.IsDead)
			{
				mover.MoveDir = new Vector3(mover.MoveDir.x, bounceMoveDirFromTerrain, mover.MoveDir.z);
				mover.Teleport(new Vector3(mover.Position.x, LimitForTopYPos, mover.Position.z));
			}
		}
	}

	private void UpdateKeepAboveTerrain()
	{
		float terrainY = gameplayManager.TerrainController.GetTerrainYPos(mover.Position.x, mover.Position.z);
		if(terrainY + mover.Radius > mover.Position.y)
		{
			mover.Position = new Vector3(mover.Position.x, terrainY + mover.Radius, mover.Position.z);
		}

	}

	public float LimitForTopYPos
	{
		get
		{
			float flyLimit = mapConfig.GetFlyHeightLimit(gameplayManager.GameTime);
			return gameplayManager.TerrainController.BottomWorldY + flyLimit;
		}
	}
}
