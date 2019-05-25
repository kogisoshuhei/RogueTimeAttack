using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Completed

{
	
	public class CallTitleScript : MonoBehaviour {

		public AudioSource audioSource;

		void Start ()
		{

			audioSource.Play(); 

		}

		void Update ()
		{
			
			if (Input.GetMouseButtonDown (0)) 
			{

				audioSource.Stop(); 

				SceneManager.LoadScene ("Title");

			}

		}

	}

}