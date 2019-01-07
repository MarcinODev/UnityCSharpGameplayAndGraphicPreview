using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Supports camera movement to follow its ship target
/// </summary>
public class CameraShipFollower : CameraFollower
{
	[SerializeField] private float shakeAmptitudeOnCollide = 1f;
	[SerializeField] private float shakeTimeOnCollide = 1f;
	[SerializeField] private float shakeIntervalsOnCollide = 0.1f;

	private Mover shipTarget;
	private Coroutine shakeCoroutine;

	public void SetTargetToFollow(Mover target)
	{
		SetTargetToFollow(target.Trans);
		shipTarget = target;
		shipTarget.onCollide += OnShipCollide;
	}

	public void Shake()
	{
		if(shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
		}

		shakeCoroutine = StartCoroutine(ShakeCoroutine());
	}

	private void OnShipCollide(Entity entity, Entity entityColliding)
	{
		Shake();
	}

	private IEnumerator ShakeCoroutine()
	{
		float startTime = Time.time;
		float endTime = startTime + shakeTimeOnCollide;
		var wait = new WaitForSeconds(shakeIntervalsOnCollide);

		while(endTime > Time.time)
		{
			ForceOffset = (UnityEngine.Random.insideUnitCircle * shakeAmptitudeOnCollide).ToVec3XY();
			yield return wait;
		}

		ForceOffset = Vector3.zero;
		shakeCoroutine = null;
	}
}
