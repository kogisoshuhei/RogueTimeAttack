﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CallStageScript : MonoBehaviour {

	void Start ()
	{

	}

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			SceneManager.LoadScene("Game");
		}
	}
}