using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlls Holo terrain view and generation parameters dependingly of current game hardness level
/// </summary>
public class TerrainController
{
	private MapConfig mapConfig;
	private GameplayManager gameplayManager;
	private HoloTerrainGenerator terrainGenerator;
	private float terrainYOffset;

	public TerrainController(GameplayManager gameplayManager, MapConfig mapConfig, HoloTerrainGenerator terrainGenerator)
	{
		this.mapConfig = mapConfig;
		this.gameplayManager = gameplayManager;
		this.terrainGenerator = terrainGenerator;
		terrainYOffset = Random.value * 1000f;
	}

	/// <summary>
	/// Initializes basics of terrain generator parameters
	/// </summary>
	public void OnPreStartGame()
	{
		terrainGenerator.Initialize();
		terrainGenerator.Refresh(0f, terrainYOffset);
		terrainGenerator.SetHeightFactorToTerrainMaterial(0f);
	}

	public void Update(float gameTime, float deltaTime)
	{
		float hardness = mapConfig.GetHardness(gameTime);

		CurrentHeightFactor = hardness * mapConfig.maxTerrainHeightFactor;
		terrainGenerator.SetHeightFactorToTerrainMaterial(CurrentHeightFactor);

		CurrentScrollSpeed = Mathf.Lerp(mapConfig.minMapScrollSpeed, mapConfig.maxMapScrollSpeed, hardness);
		terrainYOffset += CurrentScrollSpeed * deltaTime;
		terrainGenerator.Refresh(0f, terrainYOffset);
	}

	public float GetTerrainYPos(float x, float z)
	{
		return BottomWorldY + terrainGenerator.GetTerrainHeight(x, z) * CurrentHeightFactor;
	}

	public float CurrentScrollSpeed { get; private set; }
	public float CurrentHeightFactor { get; private set; }
	public float BottomWorldY
	{
		get
		{
			return terrainGenerator.BottomTerrainY;
		}
	}
}
