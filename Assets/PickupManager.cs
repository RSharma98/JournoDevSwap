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

	public int turnsBeforeSlowingPlayer;
	public float slowedDownPlayerSpeed;

	public int turnsBeforeTimer;
	public float maxTimer;
	public Text timerText;
	private float timeRemaining;
	private bool startedTimer;
	private bool pauseTimer;

	public int turnsBeforeEnd;
	public Image endScreen;
	public Text endScreenText;
	public Button endScreenButton;

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
		startedTimer = false;
		pauseTimer = false;
		endScreen.enabled = false;
		endScreenText.enabled = false;
		endScreenButton.gameObject.SetActive(false);
		timeRemaining = maxTimer;
		alphabet = "abcdefghijklmnopqrstuvwxyz1234567890!£$%^&*()@#~?";
	}


	// Update is called once per frame
	void Update()
	{
		if (collectedItem)
		{
			StartCoroutine(PickItem(3.0f));
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

						if (currentTurn < turnsBeforeRandomisation)
						{
							col.gameObject.GetComponent<Pickup>().Reset();
						}
						else
						{
							RandomisePickups();
							if (!startedTextEffect) StartCoroutine(TextEffect());
							if (!startedDarkness) StartCoroutine(DarknessEffect());

							if (currentTurn >= turnsBeforeTimer) {
								startedTimer = true;
								pauseTimer = true;
							}

							if (currentTurn >= turnsBeforeSlowingPlayer) {
							UnityStandardAssets.Characters.FirstPerson.FirstPersonController.instance.SetPlayerSpeed(slowedDownPlayerSpeed);
							}

							if (currentTurn >= turnsBeforeEnd) {
								StartCoroutine(FadeInEndScreen());
							}
						}
					}
				}
			}
		}

		if (startedTimer) {
			if(!pauseTimer) timeRemaining -= Time.deltaTime;
			if (timeRemaining <= 0) {
				RandomisePickups();
				StartCoroutine(PickItem(0.0f));
				timeRemaining = maxTimer;
			}
			string textForTimer = timeRemaining < 10.0f ? timeRemaining.ToString("F1") : timeRemaining.ToString("F0");
			timerText.text = textForTimer;
		}
	}

	IEnumerator PickItem(float timeToWait) {
		collectedItem = checkItemsInRange = false;
		yield return new WaitForSeconds(timeToWait);
		int i = Random.Range(0, pickups.Length);
		currentTarget = pickups[i];
		currentTurn++;
		Debug.Log("Current Target: " + currentTarget.name);
		missionText.text = currentTarget.GetComponent<Pickup>().GetMissionText;
		checkItemsInRange = true;
		pauseTimer = false;
		timeRemaining = maxTimer;
	}

	IEnumerator FadeInEndScreen() {
		endScreen.enabled = true;
		endScreenText.enabled = true;
		float alpha = 0;
		while (alpha < 1) {
			Color endScreenColour = endScreen.color;
			endScreenColour.a = alpha;
			endScreen.color = endScreenColour;

			Color endTextColour = endScreenText.color;
			endTextColour.a = alpha;
			endScreenText.color = endTextColour;

			alpha += Time.deltaTime;
			yield return null;
		}
		endScreenButton.gameObject.SetActive(true);
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
		Color blackColour = new Color(0.1f, 0.1f, 0.1f);
		while (cam.backgroundColor != blackColour) {
			Color camColor = cam.backgroundColor;
			camColor.r = Mathf.MoveTowards(camColor.r, blackColour.r, Time.deltaTime);
			camColor.g = Mathf.MoveTowards(camColor.g, blackColour.g, Time.deltaTime);
			camColor.b = Mathf.MoveTowards(camColor.b, blackColour.b, Time.deltaTime);
			cam.backgroundColor = camColor;
			yield return null;
		}
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
