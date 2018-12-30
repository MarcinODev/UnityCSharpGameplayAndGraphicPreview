using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity class with extended movement parameters
/// </summary>
public class Mover : Entity
{
	[SerializeField] private float startupMoveSpeed = 1f;
	[SerializeField] private Vector3 startupMoveDir = Vector3.zero;
	[SerializeField] private float maxZRotationOnHorizontalMove = 60f;
	[SerializeField] private bool lockMovementWithinMapBorder = false;
	[SerializeField] private bool damageOnHitTerrain = false;
	[SerializeField] private bool keepAlwaysAboveTerrain = false;

	public override void Initialize(EntityController controller, int team)
	{
		base.Initialize(controller, team);
		MoveSpeed = startupMoveSpeed;
		MoveDir = startupMoveDir;
	}


	public float MoveSpeed { get; set; }
	public Vector3 MoveDir { get; set; }//doesnt need to be normalised 
	public float MaxZRotation
	{
		get
		{
			return maxZRotationOnHorizontalMove;
		}
	}

	public bool LockMovementWithinMapBorder
	{
		get
		{
			return lockMovementWithinMapBorder;
		}
	}

	public bool KeepAlwaysAboveTerrain
	{
		get
		{
			return keepAlwaysAboveTerrain;
		}
	}

	public bool DamageOnHitTerrain
	{
		get
		{
			return damageOnHitTerrain;
		}
	}

	public Vector3 StartupMoveDir
	{
		get
		{
			return startupMoveDir;
		}
	}
}
