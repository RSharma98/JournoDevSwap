 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupManager : MonoBehaviour
{

	private GameObject[] pickups;
	private GameObject currentTarget;
	private bool collectedItem;
	private bool checkItemsInRange;

	public Vector3 dropoffPosition;
	public float dropoffRange;
	public LayerMask pickupLayer;

	public Text missionText;

	public static PickupManager instance;
	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(this);
	}

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
		else if(checkItemsInRange) {
			Collider[] pickupsInRange = Physics.OverlapSphere(dropoffPosition, dropoffRange, pickupLayer);
			if (pickupsInRange.Length > 0) {
				foreach (Collider col in pickupsInRange) {
					if (col.transform.parent == null) {
						if (col.gameObject.Equals(currentTarget)) {
							Debug.Log("Correct Item attained");
							collectedItem = true;
						} else Debug.Log("That's the wrong item");
						col.gameObject.GetComponent<Pickup>().Reset();
					}
				}
			}
		}
	}

	IEnumerator PickItem() {
		collectedItem = checkItemsInRange = false;
		yield return new WaitForSeconds(5.0f);
		int i = Random.Range(0, pickups.Length - 1);
		currentTarget = pickups[i];
		Debug.Log("Current Target: " + currentTarget.name);
		missionText.text = currentTarget.GetComponent<Pickup>().GetMissionText;
		checkItemsInRange = true;
		yield return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 1, 0, 0.3f);
		Gizmos.DrawSphere(dropoffPosition, dropoffRange);
	}
}
