 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

	public ParticleSystem particleEffects;
	public int turnsBeforeRandomisation;
	private int currentTurn;

	private bool startedDarkness;
	private bool playedParticles;
	private bool startedTextEffect;

	[System.Serializable]
	public class RandomArea {
		public Vector3 position;
		public Vector3 size;
	}

	public TextMeshProUGUI[] roomTexts;
	private string alphabet;

	public RandomArea[] randomAreas;

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
		currentTurn = 0;
		startedDarkness = false;
		playedParticles = false;
		startedTextEffect = false;
		alphabet = "abcdefghijklmnopqrstuvwxyz1234567890!£$%^&*()@#~?";
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
							missionText.text = "THAT'S IT!";
							collectedItem = true;
							if (!playedParticles) StartCoroutine(PlayParticles());
						} else missionText.text = "That's not right. " + currentTarget.GetComponent<Pickup>().GetMissionText;

						if (currentTurn <= turnsBeforeRandomisation)
						{
							col.gameObject.GetComponent<Pickup>().Reset();
							//if(!startedDarkness) StartCoroutine(DarknessEffect());
						}
						else
						{
							RandomisePickups();
							if (!startedTextEffect) StartCoroutine(TextEffect());
						}
					}
				}
			}
		}
	}

	IEnumerator PickItem() {
		collectedItem = checkItemsInRange = false;
		yield return new WaitForSeconds(5.0f);
		int i = Random.Range(0, pickups.Length);
		currentTarget = pickups[i];
		currentTurn++;
		Debug.Log("Current Target: " + currentTarget.name);
		missionText.text = currentTarget.GetComponent<Pickup>().GetMissionText;
		checkItemsInRange = true;
		yield return null;
	}

	IEnumerator PlayParticles() {
		particleEffects.Play();
		playedParticles = true;
		yield return new WaitForSeconds(6.0f);
		playedParticles = false;
	}

	//This function will randomise the position of all pickups
	private void RandomisePickups() {
		for (int i = 0; i < pickups.Length; i++) {
			int r = Random.Range(0, randomAreas.Length);
			RandomArea area = randomAreas[r];
			Vector3 pos = Vector3.zero;
			pos.x = Random.Range(area.position.x - (area.size.x / 2.0f), area.position.x + (area.size.x / 2.0f));
			pos.y = Random.Range(area.position.y - (area.size.y / 2.0f), area.position.y + (area.size.y / 2.0f));
			pos.z = Random.Range(area.position.z - (area.size.z / 2.0f), area.position.z + (area.size.z / 2.0f));
			pickups[i].transform.position = pos;
		}
	}

	IEnumerator TextEffect() {
		startedTextEffect = true;
		int i = 0;
		while (true) {
			int length = Random.Range(6, 15);
			char[] newText = new char[length];
			for (int j = 0; j < length; j++)
			{
				int a = Random.Range(0, alphabet.Length - 1);
				newText[j] = alphabet[a];
			}
			roomTexts[i].SetCharArray(newText, 0, newText.Length - 1);
			i++;
			if (i > roomTexts.Length - 1) i = 0;
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator DarknessEffect() {
		startedDarkness = true;
		Camera cam = Camera.main;
		yield return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 1, 0, 0.3f);
		Gizmos.DrawSphere(dropoffPosition, dropoffRange);

		Gizmos.color = new Color(0, 0, 1, 0.1f);
		for (int i = 0; i < randomAreas.Length; i++) {
			Gizmos.DrawCube(randomAreas[i].position, randomAreas[i].size);
		}
	}
}
