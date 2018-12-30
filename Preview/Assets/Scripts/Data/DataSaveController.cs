using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SavedDataType
{
	BestScore
}

/// <summary>
/// DataSaveController supports simple save process.
/// </summary>
public static class DataSaveController
{
	private static Dictionary<string, object> valuesCache = new Dictionary<string, object>();

	public static void SaveData(SavedDataType dataType, int value)
	{
		string key = DataTypeToKey(dataType);
		PlayerPrefs.SetInt(key, value);
		valuesCache[key] = value;
		PlayerPrefs.Save();
	}

	public static void SaveData(SavedDataType dataType, string value)
	{
		string key = DataTypeToKey(dataType);
		PlayerPrefs.SetString(key, value);
		valuesCache[key] = value;
		PlayerPrefs.Save();
	}

	public static int GetIntData(SavedDataType dataType, int defaultVal = -1)
	{
		string key = DataTypeToKey(dataType);
		object objValue;
		if(!valuesCache.TryGetValue(key, out objValue))
		{
			int value = PlayerPrefs.GetInt(key, defaultVal);
			valuesCache[key] = value;
			return value;
		}

		return (int)objValue;
	}

	public static string GetStringData(SavedDataType dataType)
	{
		string key = DataTypeToKey(dataType);
		object objValue;
		if(!valuesCache.TryGetValue(key, out objValue))
		{
			string value = PlayerPrefs.GetString(key);
			valuesCache[key] = value;
			return value;
		}

		return (string)objValue;
	}

	private static string DataTypeToKey(SavedDataType dataType)
	{
		return "SAVE_" + dataType;
	}
}
