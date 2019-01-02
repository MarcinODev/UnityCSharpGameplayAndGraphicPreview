using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic postprocess effect script
/// </summary>
[ExecuteInEditMode]
public class PostprocessEffect : MonoBehaviour
{
	[SerializeField] private Material material;
	[SerializeField] private bool requiresDepthTexture;
	[SerializeField] private bool copyMaterialInstance;

	#region UnityMethods
	protected void Awake()
	{
		if(Application.isPlaying && copyMaterialInstance)
		{
			material = Instantiate(material);//copy to make it unchangeable for repository
			material.name += gameObject.name;
		}
	}

	protected void OnEnable()
	{
		Cam = GetComponent<Camera>();
		if(requiresDepthTexture && Cam.depthTextureMode == DepthTextureMode.None)
		{
			Cam.depthTextureMode = DepthTextureMode.Depth;
		}
	}

	protected void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if(material == null)
		{
			Graphics.Blit(src, dest);
			return;
		}
		
		Graphics.Blit(src, dest, material);
	}
	#endregion
	
	public Camera Cam { get; private set; }
	public Material Material
	{
		get
		{
			return material;
		}
	}
}
