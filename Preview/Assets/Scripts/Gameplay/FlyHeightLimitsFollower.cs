using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// After attaching this script object will follow current fly limit (y position) for ship
/// </summary>
public class FlyHeightLimitsFollower : MonoBehaviour
{
	[SerializeField] private GameplayManager gameplayManager;
	[SerializeField] private MapConfig mapConfig;

	protected void Awake()
	{
		Trans = transform;
	}

	protected void Update()
	{
		if(gameplayManager && gameplayManager.IsGameRunning)
		{
			float flyLimit = mapConfig.GetFlyHeightLimit(gameplayManager.GameTime);
			float maxFlyY = gameplayManager.TerrainController.BottomWorldY + flyLimit;
			Trans.position = new Vector3(Trans.position.x, maxFlyY, Trans.position.z);
		}
	}

	public Transform Trans { get; private set; }
}
