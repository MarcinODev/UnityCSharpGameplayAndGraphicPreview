using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class HoloPostprocess : MonoBehaviour
{
	[SerializeField] private Material holoPostprocessMat;
	private Coroutine fadeCoroutine;

	#region UnityMethods
	protected void Awake()
	{
		if(Application.isPlaying)
		{
		//	holoPostprocessMat = new Material(holoPostprocessMat);//copy to make it unchangeable for repository
		}
	}

	protected void OnEnable()
	{
		Cam = GetComponent<Camera>();
		if(Cam.depthTextureMode == DepthTextureMode.None)
		{
			Cam.depthTextureMode = DepthTextureMode.Depth;
		}
	}

	protected void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if(holoPostprocessMat == null)
		{
			Graphics.Blit(src, dest);
			return;
		}

		Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(Cam.projectionMatrix, false);//because Y can be flipped dependingly on platform
		projectionMatrix[2, 3] = projectionMatrix[3, 2] = 0.0f;
		projectionMatrix[3, 3] = 1.0f;
		Matrix4x4 clipToWorld = Matrix4x4.Inverse(projectionMatrix * Cam.worldToCameraMatrix) 
								* Matrix4x4.TRS(new Vector3(0, 0, -projectionMatrix[2, 2]), Quaternion.identity, Vector3.one);
		holoPostprocessMat.SetMatrix("_ClipToWorld", clipToWorld);
		Graphics.Blit(src, dest, holoPostprocessMat);
	}
	#endregion

	public void FadeToColor(Color color, float fadeTime)
	{
		if(fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
		}
		fadeCoroutine = StartCoroutine(FadeToColorCoroutine(color, fadeTime));
	}

	private IEnumerator FadeToColorCoroutine(Color color, float fadeTime)
	{
		Color startColor = holoPostprocessMat.GetColor("_Color");
		var wait = new WaitForEndOfFrame();
		float startTime = Time.time;
		float endTime = startTime + fadeTime;
		while(Time.time < endTime)
		{
			Color lerpColor = Color.Lerp(startColor, color, (Time.time - startTime) / fadeTime);
			holoPostprocessMat.SetColor("_Color", lerpColor);
			yield return wait;
		}
		holoPostprocessMat.SetColor("_Color", color);
		fadeCoroutine = null;
	}

	public Camera Cam { get; private set; }
}
