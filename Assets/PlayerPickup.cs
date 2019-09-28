using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{

	[Header("Pickup stuff")]
	public float pickupRange;		//The range within which pickups are detected
	public Transform pickupOffset;	//The offset for pickup detection
	public LayerMask pickupLayer;   //The layer of the  pickups

	private GameObject currentPickup = null;    //The current object being carried

	void Update()
    {
		//TODO: GET HELP FROM BYRON ON ROTATION
		if (Input.GetMouseButtonDown(0)) {	//If the left mouse button is pressed
			if (currentPickup == null)		//If there is no object currently picked
			{
				//Store all the pickup objects in range
				Collider[] pickupObjectsInRange = Physics.OverlapSphere(transform.position - (transform.position - pickupOffset.position), pickupRange, pickupLayer);
				if (pickupObjectsInRange.Length > 0)
				{
					//Set the current pickup to the closest pickup and make it a child
					currentPickup = ClosestObject(pickupObjectsInRange).gameObject;	
					currentPickup.transform.parent = this.transform;
				}
				else Debug.Log("NO ITEMS TO PICKUP!");	//There are no items in range
			}
			else {	//If an item is currently picked up, drop it.
				currentPickup.transform.parent = null;
				currentPickup = null;
			}
		}

		//Take the current pickup and move it in front of the character
		if (currentPickup != null) {
			currentPickup.transform.position = transform.position - (transform.position - pickupOffset.position);
		}
    }

	//This function returns the closest pickup from an array by comparing them all
	Collider ClosestObject(Collider[] pickups) {
		Collider closestPickup = null;
		float dist = Mathf.Infinity;
		for (int i = 0; i < pickups.Length; i++) {
			if (Vector3.Distance(transform.position, pickups[i].transform.position) < dist) {
				closestPickup = pickups[i];
				dist = Vector3.Distance(transform.position, pickups[i].transform.position);
				Debug.Log("Distance: " + dist);
			}
		}
		return closestPickup;
	}

	//Visualise the pickup position by drawing gizmos
	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0, 1, 0, 0.25f);
		Gizmos.DrawWireSphere(transform.position - (transform.position - pickupOffset.position), pickupRange);
	}
}
