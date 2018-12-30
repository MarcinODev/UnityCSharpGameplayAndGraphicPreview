using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates RenderTexture for gui camera and attachest it to decal material
/// </summary>
public class GUICameraToRTForDecal : MonoBehaviour
{
	[SerializeField] private DecalRenderer targetDecal;
	[SerializeField] private Camera guiCamera;

	protected void Awake()
	{
		RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
		guiCamera.targetTexture = rt;
		targetDecal.material.SetTexture("_MainTex", rt);
	}
}
