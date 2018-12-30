using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

/// <summary>
/// Simple controller for HoloWorld gui
/// </summary>
public class HoloWorldGUIController : MonoBehaviour
{
	[SerializeField] private WorldSwaper worldSwaper;
	[SerializeField] private GameplayManager gameplayManager;
	[SerializeField] private TextMeshProUGUI scoreAndLifeText;
	[SerializeField] private Button goToMainMenuButton;
	[SerializeField] private GameObject youDieMessage;

	#region UnityMethods
	protected void Start()
	{
		gameplayManager.onGameStarted += OnGameStart;
		gameplayManager.onGameFinished += OnGameFinish;
	}

	protected void OnDestroy()
	{
		if(gameplayManager != null)
		{
			gameplayManager.onGameStarted -= OnGameStart;
			gameplayManager.onGameFinished -= OnGameFinish;
		}
	}
	#endregion

	public void GoToMainMenu()
	{
		worldSwaper.SwapHoloToRealWorld(null, null);
		goToMainMenuButton.gameObject.SetActive(false);
		scoreAndLifeText.gameObject.SetActive(false);
		youDieMessage.SetActive(false);
	}

	private void OnGameStart()
	{
		scoreAndLifeText.gameObject.SetActive(true);
		StartCoroutine(UpdateScoreCoroutine());
	}

	private void OnGameFinish()
	{
		StopAllCoroutines();
		goToMainMenuButton.gameObject.SetActive(true);
		youDieMessage.SetActive(true);
		UpdateScore();
	}

	private IEnumerator UpdateScoreCoroutine()
	{
		while(gameplayManager.IsGameRunning)
		{
			UpdateScore();
			yield return new WaitForSeconds(1f);
		}
	}

	private void UpdateScore()
	{
		string text = String.Format("Score: {0}\nLifes: {1}", Mathf.FloorToInt(gameplayManager.GameTime), gameplayManager.ShipEntity.Life);//this can be replaced by localization in the future
		scoreAndLifeText.text = text;
	}
}
