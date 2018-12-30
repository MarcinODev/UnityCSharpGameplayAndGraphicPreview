using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple bombs spawning mechanism which spawns bombs dependently on current game hardness
/// </summary>
public class BombSpawner
{
	private float lastSpawnTime = 0f;
	private MapConfig mapConfig;
	private GameplayManager gameplayManager;

	public BombSpawner(GameplayManager gameplayManager, MapConfig mapConfig)
	{
		this.mapConfig = mapConfig;
		this.gameplayManager = gameplayManager;
	}

	/// <summary>
	/// Spawns bomb basing on game time and current hardness
	/// </summary>
	public void Update(float gameTime)
	{
		float hardness = mapConfig.GetHardness(gameTime);
		float spawnInterval = Mathf.Lerp(mapConfig.maxBombSpawnInterval, mapConfig.minBombSpawnInterval, hardness);
		if(lastSpawnTime + spawnInterval > gameTime || gameplayManager.ShipEntity == null || gameplayManager.ShipEntity.IsDead)
		{
			return;
		}

		TerrainController terrainController = gameplayManager.TerrainController;
		float zPos = mapConfig.mapWorldBoundings.yMax + gameplayManager.bombPrefab.StartupMoveDir.z;

		float xPos = 0f;
		if(UnityEngine.Random.value < mapConfig.chanceBombSpawnXSameAsShip)
		{
			xPos = gameplayManager.ShipEntity.Position.x;
		}
		else
		{
			xPos = UnityEngine.Random.Range(mapConfig.mapWorldBoundings.xMin, mapConfig.mapWorldBoundings.xMax) + gameplayManager.bombPrefab.StartupMoveDir.x;
		}

		float yPos = 0f;
		if(UnityEngine.Random.value < mapConfig.chanceBombSpawnYSameAsShip)
		{
			yPos = gameplayManager.ShipEntity.Position.y;
		}
		else
		{
			float terrainYPos = terrainController.GetTerrainYPos(xPos, zPos) + gameplayManager.bombPrefab.Radius;
			float flyHeightLimit = mapConfig.GetFlyHeightLimit(gameplayManager.GameTime);
			yPos = UnityEngine.Random.Range(terrainYPos, flyHeightLimit + terrainController.BottomWorldY);
		}

		Vector3 position = new Vector3(xPos, yPos, zPos);

		Mover bomb = (Mover)EntityFactory.CreateOrPullEntity(gameplayManager.bombPrefab, GameplayManager.enemyTeam, position, mapConfig, gameplayManager);


		bomb.MoveSpeed = terrainController.CurrentScrollSpeed * mapConfig.terrainScrollSpeedToBombSpeed;
		bomb.Trans.parent = gameplayManager.transform;
		lastSpawnTime = gameTime;
	}
}
