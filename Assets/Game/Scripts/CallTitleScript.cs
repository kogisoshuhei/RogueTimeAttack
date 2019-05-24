using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Completed

{
	
	public class CallTitleScript : MonoBehaviour {

		void Start()
		{
			

		}

		void Update ()
		{
			if (Input.GetMouseButtonDown (0)) {
				SceneManager.LoadScene("Title");
			}
		}
	}
}