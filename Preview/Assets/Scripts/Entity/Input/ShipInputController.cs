using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Local player ship input controller
/// </summary>
public class ShipInputController : MonoBehaviour
{
	[SerializeField] private float directionChangeSpeed = 1f;

	private Mover ship;
	private GameplayManager gameplayManager;

	public void Initialize(GameplayManager gameplayManager)
	{
		this.gameplayManager = gameplayManager;
	}

	public void AssignShip(Mover ship)
	{
		this.ship = ship;
	}

	protected void Update()
	{
		if(ship == null)
		{
			return;
		}

		float horizontalInput = Input.GetAxis("Horizontal");
		float bumpUpInput = Input.GetAxis("Vertical");
		Vector3 inputDir = new Vector3(horizontalInput, bumpUpInput, 0f);

		//secure player from wrong decision system (if we are too close to terrain, input will force got up, even if player tries to die)
		float moveSpeed = ship.MoveSpeed * Time.deltaTime;
		Vector3 moveVec = moveSpeed * inputDir;
		float terrainY = gameplayManager.TerrainController.GetTerrainYPos(ship.Position.x, ship.Position.z);
		if(ship.Position.y + moveVec.y <= terrainY)
		{
			inputDir.y = 1f;
		}

		Vector3 moveDir = Vector3.Lerp(ship.MoveDir, inputDir, directionChangeSpeed * Time.deltaTime);
		ship.MoveDir = moveDir;

	}
}
