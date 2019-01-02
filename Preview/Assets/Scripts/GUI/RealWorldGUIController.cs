using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple controller for RealWorld gui
/// </summary>
public class RealWorldGUIController : MonoBehaviour
{
	[SerializeField] private WorldSwaper worldSwaper;
	[SerializeField] private GameplayManager gameplayManager;
	private bool loadingGame = false;

	public void PlayGame()
	{
		if(loadingGame)
		{
			return;
		}
		worldSwaper.SwapRealToHoloWorld(OnFinishWorldSwap, gameplayManager.PreStartGame);
		loadingGame = true;
	}

	private void OnFinishWorldSwap()
	{
		gameplayManager.StartGame();
		loadingGame = false;
	}
}
