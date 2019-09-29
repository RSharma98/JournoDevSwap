using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelScript : MonoBehaviour
{
	public void LoadLevel(string level) {
		SceneManager.LoadScene(level);
	}

	public void HideOverlay(GameObject overlay) {
		overlay.SetActive(false);
		UnityStandardAssets.Characters.FirstPerson.FirstPersonController.instance.EnableMouse();
	}
}
