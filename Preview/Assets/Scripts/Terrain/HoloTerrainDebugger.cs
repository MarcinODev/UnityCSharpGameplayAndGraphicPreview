using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloTerrainDebugger : MonoBehaviour
{
	public HoloTerrainGenerator holoTerrainGenerator;

	protected void OnDrawGizmosSelected()
	{
		if(holoTerrainGenerator == null)
		{
			return;
		}
		float terrainY = holoTerrainGenerator.GetTerrainHeight(transform.position.x, transform.position.z) * holoTerrainGenerator.LastHeightFactor
						+ holoTerrainGenerator.BottomTerrainY;

		Gizmos.color = Color.green;
		Gizmos.DrawLine(new Vector3(transform.position.x, terrainY, transform.position.z), transform.position);
	}
}
