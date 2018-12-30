using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic renderer script for decal objects. It self registers to DecalsRendererManager
/// </summary>
[ExecuteInEditMode]
public class DecalRenderer : MonoBehaviour
{
	public Material material;

	protected void Start()
	{
		DecalsRendererManager.Instance.AddDecal(this);
	}

	protected void OnEnable()
	{
		DecalsRendererManager.Instance.AddDecal(this);
	}

	protected void OnDisable()
	{
		DecalsRendererManager.Instance.RemoveDecal(this);
	}

	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawCube(Vector3.zero, Vector3.one);
	}

	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
	}

}
