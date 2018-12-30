using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class AnimationExtensions
{
	public static AnimationClip GetClip(this Animation animation, int clipId)
	{
		if(clipId < 0 || clipId >= animation.GetClipCount())
		{
			return null;
		}

		int i = 0;
		foreach(var clip in animation)
		{
			if(i++ == clipId)
			{
				return (AnimationClip)clip;
			}
		}

		return null;
	}
}

