using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Manager responsible for rendering all DecalRenderers based on custom Deffered Decals in CameraEvent.BeforeLighting buffer. 
/// Note that it should be attached to MeshRenderer because uses OnWillRenderObject to attach to Camera buffers.
/// </summary>
[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer))]
public class DecalsRendererManager : MonoBehaviour
{
	[SerializeField]private Mesh decalMesh;
	private Dictionary<Camera, CommandBuffer> cameras = new Dictionary<Camera, CommandBuffer>();
	private HashSet<DecalRenderer> decals = new HashSet<DecalRenderer>();
	private static DecalsRendererManager instance;

	#region UnityMethods
	protected void Awake()
	{
		if(Instance != this && Instance != null)
		{
			DestroyImmediate(gameObject);
			return;
		}
		instance = this;
	}

	protected void OnDisable()
	{
		foreach(var cam in cameras)
		{
			if(cam.Key)
			{
				cam.Key.RemoveCommandBuffer(CameraEvent.BeforeLighting, cam.Value);
			}
		}
		cameras.Clear();
	}

	protected void OnWillRenderObject()
	{
		var isEnabled = gameObject.activeInHierarchy && enabled;
		if(!isEnabled)
		{
			OnDisable();
			return;
		}

		var currentCam = Camera.current;
		if(!currentCam)
		{
			return;
		}

		RenderDecals(currentCam);
	}
	#endregion

	public void AddDecal(DecalRenderer decalRenderer)
	{
		if(!decals.Contains(decalRenderer))
		{
			decals.Add(decalRenderer);
		}
	}

	public void RemoveDecal(DecalRenderer decalRenderer)
	{
		decals.Remove(decalRenderer);
	}

	private void RenderDecals(Camera currentCam)
	{
		CommandBuffer buffer = GetBufferForRendering(currentCam);

		int normalsID = GetBufferNormals(buffer);

		SetBufferRenderTargets(buffer);

		foreach(var decal in decals)
		{
			buffer.DrawMesh(decalMesh, decal.transform.localToWorldMatrix, decal.material);//can be extended with culling if used in wider range
		}
		buffer.ReleaseTemporaryRT(normalsID);
	}

	private CommandBuffer GetBufferForRendering(Camera currentCam)
	{
		CommandBuffer buffer = null;
		if(cameras.ContainsKey(currentCam))
		{
			buffer = cameras[currentCam];
			buffer.Clear();
		}
		else
		{
			buffer = new CommandBuffer();
			buffer.name = "Decals";
			cameras[currentCam] = buffer;
			currentCam.AddCommandBuffer(CameraEvent.BeforeLighting, buffer);
		}

		return buffer;
	}

	/// <summary>
	/// Initializes _GBufferNormals from GBuffer
	/// </summary>
	/// <returns>PropertyToID of _GBufferNormals texture</returns>
	private static int GetBufferNormals(CommandBuffer buffer)
	{
		var normalsID = Shader.PropertyToID("_GBufferNormals");
		buffer.GetTemporaryRT(normalsID, -1, -1);
		buffer.Blit(BuiltinRenderTextureType.GBuffer2, normalsID);
		return normalsID;
	}

	private static void SetBufferRenderTargets(CommandBuffer buffer)
	{
		RenderTargetIdentifier[] mrt = { BuiltinRenderTextureType.GBuffer0, BuiltinRenderTextureType.GBuffer2, BuiltinRenderTextureType.CameraTarget };//diffuse, normals, emission
		buffer.SetRenderTarget(mrt, BuiltinRenderTextureType.CameraTarget);
	}

	public static DecalsRendererManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType<DecalsRendererManager>();
			}
			return instance;
		}
	}
}
