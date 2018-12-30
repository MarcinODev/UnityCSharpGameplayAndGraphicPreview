using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data container for map configuration for gameplay.
/// </summary>
public class MapConfig : MonoBehaviour
{
	public Rect mapWorldBoundings;
	public float minFlyHeight = 5f;
	public float maxFlyHeight = 20f;
	public float minMapScrollSpeed = 1f;
	public float maxMapScrollSpeed = 1f;
	public float terrainScrollSpeedToBombSpeed = 10f;
	public float minBombSpawnInterval = 1f;
	public float maxBombSpawnInterval = 20f;
	public float chanceBombSpawnYSameAsShip = 0.5f;
	public float chanceBombSpawnXSameAsShip = 0.2f;
	public float maxTerrainHeightFactor = 1f;
	public AnimationCurve hardnessCurve;
	public float timeToMaxHardnessCurve = 60f;

	#region UnityMethods
	protected void Awake()
	{
		MapMinY = transform.position.y;
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(new Vector3(mapWorldBoundings.center.x, 
										transform.position.y + maxFlyHeight * 0.5f,
										mapWorldBoundings.center.y), 
							new Vector3(mapWorldBoundings.width, maxFlyHeight, mapWorldBoundings.height));
	}
	#endregion

	/// <summary>
	/// Gets hardness of gameplay in certain moment of gameTime. Values are in range [0,1]
	/// </summary>
	public float GetHardness(float gameTime)
	{
		if(gameTime == 0f)
		{
			return 0f;
		}

		if(gameTime > timeToMaxHardnessCurve)
		{
			gameTime = timeToMaxHardnessCurve;
		}

		return hardnessCurve.Evaluate(gameTime / timeToMaxHardnessCurve);
	}

	/// <summary>
	/// Gets fly height limit based on hardness which bases on gameTime
	/// </summary>
	public float GetFlyHeightLimit(float gameTime)
	{
		float flyLimit = Mathf.Lerp(minFlyHeight, maxFlyHeight, GetHardness(gameTime));
		return flyLimit;
	}

	public float MapMinY { get; private set; }
}
