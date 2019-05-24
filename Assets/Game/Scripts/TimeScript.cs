using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


namespace Completed

{
	public class TimeScript : MonoBehaviour {

		private float allTime;
		private bool isStart = false;

		void Start () 
		{
			//timeをステージ間で引き継げるように、GameManagerから設定
			allTime = GameManager.instance.time;

			//初期値60を表示
			//float型からint型へCastし、String型に変換して表示
			GetComponent<Text>().text = "残り " + ((int)allTime).ToString() + "秒";

			//3秒遅れて開始
			Invoke ("StartTimer", 3.0f);


		}

		//PlayerのfoodをGameManageに保存する
		private void OnDisable ()
		{
			//Playerオブジェクトが無効になっているときは、
			//現在の制限時間の合計をGameManagerに保存して、次のレベルで再ロードできるようにする
			GameManager.instance.time = allTime;

		}

		void StartTimer()
		{

			isStart = true;

		}

		void Update ()
		{

			if (!isStart) 
			{
				return;
			}

			//1秒に1ずつ減らしていく
			allTime -= Time.deltaTime;

			//マイナスは表示しない
			if (allTime < 0) allTime = 0;

			GetComponent<Text> ().text = "残り " + ((int)allTime).ToString () + "秒";


			if(allTime == 0)
			{
				SceneManager.LoadScene("Result");

			}	

		}

	}

}