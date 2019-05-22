using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeScript : MonoBehaviour {
	
	private float time = 60;
	private bool isStart = false;

	void Start () {
		
		//初期値60を表示
		//float型からint型へCastし、String型に変換して表示
		GetComponent<Text>().text = ((int)time).ToString();

		//3秒遅れて開始
		Invoke ("StartTimer", 3.0f);

	}

	void StartTimer()
	{

		isStart = true;

	}

	void Update (){

		if (!isStart) 
		{
			return;
		}

		//1秒に1ずつ減らしていく
		time -= Time.deltaTime;

		//マイナスは表示しない
		if (time < 0) 

			time = 0;

			GetComponent<Text> ().text = ((int)time).ToString ();

	}

}