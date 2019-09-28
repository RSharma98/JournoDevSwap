using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

	public string missionText;

	private Vector3 startPosition;
	private Quaternion startRotation;

	private void Awake()
	{
		startPosition = transform.position;
		startRotation = transform.rotation;
	}

	public void Reset()
	{
		transform.position = startPosition;
		transform.rotation = startRotation;
	}

	public string GetMissionText => missionText;
}
