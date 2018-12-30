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

	public void PlayGame()
	{
		worldSwaper.SwapRealToHoloWorld(OnFinishWorldSwap, gameplayManager.PreStartGame);
	}

	private void OnFinishWorldSwap()
	{
		gameplayManager.StartGame();
	}
}
