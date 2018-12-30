using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows in text current best game score
/// </summary>
public class GUIBestScoreViewer : MonoBehaviour
{
	[SerializeField] private TextMeshPro scoreText;

	protected void OnEnable()
	{
		scoreText.text = "Best Score: " + DataSaveController.GetIntData(SavedDataType.BestScore, 0);//this can be replaced by localization in the future
	}
}
