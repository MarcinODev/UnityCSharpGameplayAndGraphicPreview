using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Postprocess effect script with handing fade in/out effects
/// </summary>
[ExecuteInEditMode]
public class ProgressivePostprocessEffect : PostprocessEffect
{
	[SerializeField] private string progressPropertyName = "_Progress";
	private Coroutine progressCoroutine;

	public void SetProgress(float progress)
	{
		Material.SetFloat(progressPropertyName, progress);
	}

	public void SetProgressOverTime(float progress, float time, float forceStartProgress = -1f)
	{
		if(progressCoroutine != null)
		{
			StopCoroutine(progressCoroutine);
		}
		progressCoroutine = StartCoroutine(SetProgressOverTimeCoroutine(progress, time, forceStartProgress));
	}
	
	private IEnumerator SetProgressOverTimeCoroutine(float targetProgress, float fadeTime, float forceStartProgress = -1f)
	{
		float startProgress = forceStartProgress >= 0f ? forceStartProgress : Material.GetFloat(progressPropertyName);
		var wait = new WaitForEndOfFrame();
		float startTime = Time.time;
		float endTime = startTime + fadeTime;
		while(Time.time < endTime)
		{
			float lerpFloat = Mathf.Lerp(startProgress, targetProgress, (Time.time - startTime) / fadeTime);
			Material.SetFloat(progressPropertyName, lerpFloat);
			yield return wait;
		}
		Material.SetFloat(progressPropertyName, targetProgress);
		progressCoroutine = null;
	}
}
