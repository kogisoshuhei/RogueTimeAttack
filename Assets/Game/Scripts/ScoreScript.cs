//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
//namespace Completed
//
//{
//	
//	public class ScoreScript : MonoBehaviour {
//
//		//スコア表示用のテキスト
//		public int allScore;
//
//		private int enemyScore;
//
//		// Use this for initialization
//		void Start () {
//
//			//timeをステージ間で引き継げるように、GameManagerから設定
//			allScore = GameManager.instance.score;
//
//			enemyScore = Enemy.instance.enemyAllPoint;
//
//			//float型からint型へCastし、String型に変換して表示
//			GetComponent<Text>().text = "Score: " + ((int)allScore).ToString();
//
//		}
//
//		//PlayerのfoodをGameManageに保存する
//		private void OnDisable ()
//		{
//
//			//Playerオブジェクトが無効になっているときは、
//			//現在の制限時間の合計をGameManagerに保存して、次のレベルで再ロードできるようにする
//			GameManager.instance.score = allScore;
//
//		}
//
//		void Update()
//		{
//			allScore += enemyScore;
//
//		}
//			
//	}
//
//}