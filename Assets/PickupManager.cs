 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{

	private GameObject[] pickups;
	private GameObject currentTarget;
	private bool collectedItem;

	// Start is called before the first frame update
	void Start()
	{
		pickups = GameObject.FindGameObjectsWithTag("Pickup");
		currentTarget = null;
		collectedItem = true;
	}


	// Update is called once per frame
	void Update()
	{
		if (collectedItem)
		{
			StartCoroutine(PickItem());
		}
		else { 
			
		}
	}

	IEnumerator PickItem() {
		collectedItem = false;
		yield return new WaitForSeconds(5f);
		int i = Random.Range(0, pickups.Length - 1);
		currentTarget = pickups[i];
		Debug.Log("Current Target: " + currentTarget.name);
		yield return null;
	}
}
