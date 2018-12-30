using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class StringExtensions
{
    public static bool IsNullOrEmpty(this string str)
    {
        return string.IsNullOrEmpty(str);
    }
    
    public static bool IsNotNullOrEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }
    
    public static string SetArgs(this string str, params object[] args)
    {
        return String.Format(str, args);
    }
    
    public static string ToStringWithSeparator(this string[] strList, string separator)
    {
        if(strList == null)
		{
			return "";
		}

		StringBuilder sb = new StringBuilder();
        for(int i = 0; i < strList.Length; i++)
        {
            sb.Append(strList[i]);
            if(i + i < strList.Length)
			{
				sb.Append(separator);
			}
		}
        return sb.ToString();
    }
    
    public static string ToStringWithSeparator(this List<string> strList, string separator)
    {
        if(strList == null)
            return "";
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < strList.Count; i++)
        {
            sb.Append(strList[i]);
            if(i + i < strList.Count)
			{
				sb.Append(separator);
			}
		}
        return sb.ToString();
    }
    
    public static string Lerp(this string str, string str2, float progress, bool startFromBegining = false)
    {
        int capacity = Mathf.Max(str.Length, str2.Length);
        StringBuilder sb = new StringBuilder(capacity);
        int endBasicSign = Mathf.RoundToInt(capacity * progress) - 1;
        int pos = 0;
        if(!startFromBegining)
        {
            for(; pos <= endBasicSign; pos++)
            {
                if(str.Length > pos)
                    sb.Append(str[pos]);
            }
        
            for(; pos <= capacity; pos++)
            {
                if(str2.Length > pos)
                    sb.Append(str2[pos]);
                else
                    break;
            }
        }
        else
        {
            for(; pos <= endBasicSign; pos++)
            {
                if(str2.Length > pos)
                    sb.Append(str2[pos]);
            }
        
            for(; pos <= capacity; pos++)
            {
                if(str.Length > pos)
                    sb.Append(str[pos]);
                else
                    break;
            }
        }

        return sb.ToString();
    }
    
    /// <returns>1 if current version is bigger, -1 if smaller, 0 if equal</returns>
    public static int CompareVersions(this string currentVersion, string otherVersion, int compareSteps = 3)
    {
        if(otherVersion == currentVersion)
            return 0;

        if(otherVersion.IsNullOrEmpty())
            return 1;

        if(currentVersion.IsNullOrEmpty())
            return -1;

        try
        {
            string[] currentSplit = currentVersion.Split('.');
            string[] otherSplit = otherVersion.Split('.');
            for(int i = 0; i < compareSteps; i++)
            {
                if(otherSplit.Length == i)
                {
                    if(currentSplit.Length == i)//in case of 2 arrays too short
                        return 0;
                    return 1;
                }

                if(currentSplit.Length == i)
                    return -1;
                
                if(currentSplit[i] == otherSplit[i])
                    continue;
                
                return int.Parse(currentSplit[i]).CompareTo(int.Parse(otherSplit[i]));
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
        return 0;
    }

}

