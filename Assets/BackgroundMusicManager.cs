using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{

	public static BackgroundMusicManager instance;

	private void Awake()
	{
		if (instance == null) instance = this;
		else Destroy(this.gameObject);
	}

    void Update()
    {
		DontDestroyOnLoad(this.gameObject);
    }
}
