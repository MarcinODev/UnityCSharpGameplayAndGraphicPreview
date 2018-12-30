using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main gameplay controller. Manages game end, game score and updates for non-MonoBehaviour gameplay related controllers objects
/// </summary>
public class GameplayManager : MonoBehaviour
{
	public const int allyTeam = 0;//teams are numbers in case of need more than 1 team
	public const int enemyTeam = 1;

	public event Action onGameStarted;
	public event Action onGameFinished;

	public Mover bombPrefab;
	public Mover shipPrefab;

	[SerializeField] private float spawnShipBorderOffset = 5f;
	[SerializeField] private HoloTerrainGenerator terrainGenerator;
	[SerializeField] private MapConfig mapConfig;
	[SerializeField] private ShipInputController shipInput;
	[SerializeField] private CameraShipFollower cameraShipFollower;

	private BombSpawner bombSpawner;
	private TerrainController terrainController;

	#region UnityMethods
	protected void Awake()
	{
		bombSpawner = new BombSpawner(this, mapConfig);
		terrainController = new TerrainController(this, mapConfig, terrainGenerator);
		shipInput.Initialize(this);
	}
	
	protected void Update()
	{
		if(!IsGameRunning)
		{
			return;
		}

		terrainController.Update(GameTime, Time.deltaTime);
		bombSpawner.Update(GameTime);
	}
	#endregion

	/// <summary>
	/// Initialisation of gameplay (like spawning ship or calling terrainController.OnPreStartGame)
	/// </summary>
	public void PreStartGame()
	{
		float xPos = mapConfig.mapWorldBoundings.center.x;
		float yPos = mapConfig.maxFlyHeight * 0.5f + terrainGenerator.BottomTerrainY;
		float zPos = mapConfig.mapWorldBoundings.yMin + spawnShipBorderOffset;
		Vector3 position = new Vector3(xPos, yPos, zPos);
		ShipEntity = (Mover)EntityFactory.CreateOrPullEntity(shipPrefab, allyTeam, position, mapConfig, this);
		ShipEntity.Trans.parent = transform;
		ShipEntity.onDeath += OnShipDie;
		shipInput.AssignShip(ShipEntity);
		cameraShipFollower.SetTargetToFollow(ShipEntity);

		terrainController.OnPreStartGame();
	}

	/// <summary>
	/// Starts the game (since now arr actions will be enabled)
	/// </summary>
	public void StartGame()
	{
		StartTime = Time.time;
		if(onGameStarted != null)
		{
			onGameStarted();
		}
	}

	private void OnShipDie(Entity entity)
	{
		FinishGame();
	}

	private void FinishGame()
	{
		if(onGameFinished != null)
		{
			onGameFinished();
		}

		int lastBestScore = DataSaveController.GetIntData(SavedDataType.BestScore);
		if(lastBestScore < GameTime)
		{
			DataSaveController.SaveData(SavedDataType.BestScore, Mathf.FloorToInt(GameTime));
		}

		//cleanups
		ShipEntity.onDeath -= OnShipDie;
		StartTime = -1f;

		Debug.Log("Game finished");
	}

	public float GameTime
	{
		get
		{
			return Time.time - StartTime;
		}
	}

	/// <summary>
	/// Game Start Time, if it's <= 0 than game is not started
	/// </summary>
	public float StartTime { get; private set; }

	public Mover ShipEntity { get; private set; }

	public bool IsGameRunning
	{
		get
		{
			return StartTime > 0f;
		}
	}

	public TerrainController TerrainController
	{
		get
		{
			return terrainController;
		}
	}
}
