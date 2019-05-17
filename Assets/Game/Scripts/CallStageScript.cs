using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CallStageScript : MonoBehaviour {

	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			SceneManager.LoadScene("Game");
		}
	}
}