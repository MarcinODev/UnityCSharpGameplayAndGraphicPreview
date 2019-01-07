using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Supports camera movement to follow its target
/// </summary>
public class CameraFollower : MonoBehaviour
{
	[SerializeField] private Vector3 offset;
	[SerializeField] private float followSpeed;
	[SerializeField] private float minYDistanceToJump = 100;

	private Transform target;

	#region UnityMethods
	protected void Awake()
	{
		Trans = transform;
	}

	protected void FixedUpdate()
	{
		if(target == null)
		{
			return;
		}

		Vector3 targetPos = target.position + offset + ForceOffset;

		float lerpVal = Mathf.Abs(targetPos.y - Trans.position.y) < minYDistanceToJump
						? Time.fixedDeltaTime * followSpeed
						: 1f;

		Trans.position = Vector3.Lerp(Trans.position, targetPos, lerpVal);

		Quaternion targetRot = Quaternion.LookRotation((target.position - Trans.position).normalized, Vector3.up);
		Trans.rotation = Quaternion.Lerp(Trans.rotation, targetRot, lerpVal);

	}
	#endregion

	public void SetTargetToFollow(Transform target)
	{
		this.target = target;
	}

	public Transform Trans { get; private set; }
	public Vector3 ForceOffset { get; set; }
}
