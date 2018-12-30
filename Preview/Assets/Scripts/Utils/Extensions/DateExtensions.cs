using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public static class DateExtensions
{
	private const string beginingOfTime = "01/01/1970 00:00:00";
	private const string isoFormat = @"yyyy-MM-dd\THH:mm:ss.fff\Z";

	public static long ToTimestamp(this DateTime val)
    {
        long ticks = val.Ticks - DateTime.Parse(beginingOfTime).Ticks;
        ticks /= 10000;//to ms
        return ticks;
    }
    
    public static DateTime TimestampToDateTime(this long val)
    {
        val *= 10000;
        val += DateTime.Parse(beginingOfTime).Ticks;
        return new DateTime(val);
    }

    public static string ToHumanString(this TimeSpan span)
    {
        string formatted = string.Format("{0}{1}{2}{3}",
            span.Duration().Days > 0 ? span.Days + " d " : string.Empty,
            span.Duration().Hours > 0 ? span.Hours + " h " : string.Empty,
            span.Duration().Minutes > 0 ? span.Minutes + " m " : string.Empty,
            span.Duration().Seconds > 0 ? span.Seconds + " s" : string.Empty);

        if (formatted.EndsWith(" ")) formatted = formatted.Substring(0, formatted.Length - 1);

        if (string.IsNullOrEmpty(formatted)) formatted = "0 s";

        return formatted;
    }

    public static string ToISOString(this DateTime date)
    {
        return date.ToString(isoFormat);
    }
	
    public static DateTime? ToUTCTimeFromISO(this string str, DateTime? defaultVal = null)
    {
        if(str.IsNullOrEmpty())
        {
            return defaultVal;
        }

        DateTime date;
        if(!DateTime.TryParseExact(str, isoFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date))
        {
            Debug.LogError("Unable to parse ISO string: " + str);
            return defaultVal;
        }

        return date.ToUniversalTime();
    }
}

