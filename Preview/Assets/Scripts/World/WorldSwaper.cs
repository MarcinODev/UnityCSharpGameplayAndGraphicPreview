using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class allowing swaping HoloWorld to RealWorld and backward. 
/// </summary>
/// <remarks>Can be replaced partially by load scene mechanims but for higher readability of effects whole game is packed into single scene.</remarks>
public class WorldSwaper : MonoBehaviour
{
	[SerializeField] private float fadeInTime = 2f;
	[SerializeField] private float fadeOutTime = 2f;
	[SerializeField] private ProgressivePostprocessEffect fadeRealWorldEffect;
	[SerializeField] private ProgressivePostprocessEffect fadeHoloWorldEffect;
	[SerializeField] private GameObject realWorldRoot;
	[SerializeField] private GameObject holoWorldRoot;
	
	public void SwapRealToHoloWorld(Action onFinish, Action onFadeOut)
	{
		StartCoroutine(SwapRealToHoloWorldCoroutine(onFinish, onFadeOut));
	}

	public void SwapHoloToRealWorld(Action onFinish, Action onFadeOut)
	{
		StartCoroutine(SwapHoloToRealWorldCoroutine(onFinish, onFadeOut));
	}

	private IEnumerator SwapRealToHoloWorldCoroutine(Action onFinish, Action onFadeOut)
	{
		fadeRealWorldEffect.enabled = true;
		fadeRealWorldEffect.SetProgressOverTime(1f, fadeOutTime, 0f);
		yield return new WaitForSeconds(fadeOutTime);
		fadeRealWorldEffect.enabled = false;

		realWorldRoot.SetActive(false);

		fadeHoloWorldEffect.enabled = true;
		holoWorldRoot.SetActive(true);
		
		fadeHoloWorldEffect.SetProgressOverTime(0f, fadeInTime, 1f);

		if(onFadeOut != null)
		{
			onFadeOut();
		}

		yield return new WaitForSeconds(fadeInTime);
		fadeHoloWorldEffect.enabled = false;

		if(onFinish != null)
			onFinish();
	}

	private IEnumerator SwapHoloToRealWorldCoroutine(Action onFinish, Action onFadeOut)
	{
		fadeHoloWorldEffect.enabled = true;
		fadeHoloWorldEffect.SetProgressOverTime(1f, fadeOutTime, 0f);
		yield return new WaitForSeconds(fadeOutTime);
		fadeHoloWorldEffect.enabled = false;

		holoWorldRoot.SetActive(false);

		fadeRealWorldEffect.enabled = true;
		realWorldRoot.SetActive(true);
		
		fadeRealWorldEffect.SetProgressOverTime(0f, fadeInTime, 1f);

		if(onFadeOut != null)
		{
			onFadeOut();
		}

		yield return new WaitForSeconds(fadeInTime);
		fadeRealWorldEffect.enabled = false;

		if(onFinish != null)
			onFinish();
	}

}
