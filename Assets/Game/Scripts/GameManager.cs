﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;		//Allows us to use Lists. 
using UnityEngine.UI;

namespace Completed
{
	
	public class GameManager : MonoBehaviour
	{

		public float levelStartDelay = 2f; 						//レベルスタート時の時間間隔

		private Text levelText; 								//レベルを表示するテキスト

		private GameObject levelImage; 							//UIの表示領域　表示のオンオフを切り替える
		private bool doingSetup; 								//設定中かどうかのフラグ

		public float turnDelay = 0.1f;							//1ターンの時間
		public int playerHp = 3;								//プレイヤーの体力

		public static GameManager instance = null;				//クラスに属し、複数のシーンで使われる変数を宣言
																//Staticにすることで、他のスクリプトからも呼び出すことができます

		[HideInInspector] public bool playersTurn = true;		//プレイヤーのターンかの判定フラグ

		private BoardManager boardScript;						//BoardManager型の変数を宣言
		private int level = 3;									//難易度
		public int floor = 5;									//階層
//		public int score = 0;									//スコア

		private List<Enemy> enemies;							//複数の敵を管理
		private bool enemiesMoving;								//敵の移動フラグ

		public float time = 30;									//制限時間


		// AwakeはStartよりも前、最初に呼ばれる
		private void Awake()
		{
			//GameManagerが存在しなければ、このオブジェクトを設定する
            if (instance == null)

                instance = this;

            else if (instance != this)

			//すでに存在する場合、このオブジェクトは不要なため破壊する
            Destroy(gameObject);
			
			//シーン遷移時に、このオブジェクトは破壊せず引き継ぐ
			DontDestroyOnLoad(gameObject);
			
			//敵を初期化
			enemies = new List<Enemy>();
			
			//BoardManagerのコンポーネントを取得
			boardScript = GetComponent<BoardManager>();
			
			//ステージ生成の関数を呼ぶための、関数を呼ぶ
			InitGame();

		}

		//シーンが呼び出されたタイミングで実行される
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		static public void CallbackInitialization()
		{

			//シーンがロードされるたびに呼び出されるコールバックを登録します
			SceneManager.sceneLoaded += OnSceneLoaded;


		}

		//シーンが呼び出されたタイミングで初期化する
		static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{

			instance.InitGame();

		}

		public void NextLevel()
		{
			Debug.Log ("今の階層は " + floor + " F");

			if( floor <= 1 )
			{
				Debug.Log ("クリア！");

				GameClear ();

			}else{

				instance.level++;
				instance.floor--;

				Debug.Log ("次は " + floor + " F");
			}

		}

		
		//Initializes the game for each level.
		void InitGame()
		{

			//設定中フラグをオンにする
			doingSetup = true;

			//levelImageにUIを設定する
			levelImage = GameObject.Find("LevelImage");

			//levelTextにUIのテキストを取得し設定する
			levelText = GameObject.Find("LevelText").GetComponent<Text>();

			//levelTextにゲーム内のlevelを設定する
			levelText.text = "脱出まで残り " + floor + " F";

			//UIを表示する
			levelImage.SetActive(true);

			//levelStartDelayで設定した秒数後にUIを非表示にする
			Invoke("HideLevelImage", levelStartDelay);
			
			//ステージ移動時は敵をリセットする
			enemies.Clear();
			
			//ステージ生成の関数を呼ぶ
			boardScript.SetupScene(level);
			
		}

		//UIを非表示にする
		private void HideLevelImage()
		{
			levelImage.SetActive(false);
			doingSetup = false;
		}

		//Update is called every frame.
		void Update()
		{

			//プレイヤーのターンか、敵の動いている場合は、アップデートしない
			if(playersTurn || enemiesMoving || doingSetup )
				
				//If any of these are true, return and do not start MoveEnemies.
				return;
			
			//敵の動いていない、敵のターンのみ敵を動かす
			StartCoroutine (MoveEnemies ());

		}
		
		//敵をリストに加える処理
		public void AddEnemyToList(Enemy script)
		{
			//Add Enemy to List enemies.
			enemies.Add(script);
		}
		
		//敵を移動させる処理
		IEnumerator MoveEnemies()
		{
			//エネミー移動フラグをtrueにする
			enemiesMoving = true;
			
			//1ターン待つ
			yield return new WaitForSeconds(turnDelay);
			
			//敵がいなければ
			if (enemies.Count == 0) 
			{
				//1ターン待つ
				yield return new WaitForSeconds(turnDelay);
			}
			
			//敵の数だけ、敵を移動させる
			for (int i = 0; i < enemies.Count; i++)
			{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
				enemies[i].MoveEnemy ();
				
				//1ターン待つ
				yield return new WaitForSeconds(enemies[i].moveTime);
			}

			//プレイヤーのターンにする
			playersTurn = true;

			Debug.Log ("プレイヤーの移動を開始する");

			enemiesMoving = false;

			Debug.Log ("敵の移動を終了する");

		}

		//ゲームオーバー時の表示
		public void GameOver()
		{
			Debug.Log ("ゲームオーバー時に呼び出された");

			//ゲームオーバーの時のBGMをストップする
			SoundManager.instance.musicSource.Stop ();

			Debug.Log ("GameOver後、GameManagerを無効にする");

			//enabledをfalseにすることで、GameManagerが無効になる
			enabled = false;

			Debug.Log ("遷移前");

			SceneManager.LoadScene("ResultDied");

			Debug.Log ("遷移後");
		
		}

		//ゲームクリア時の表示
		public void GameClear()
		{

			Debug.Log ("ゲームクリア時に呼び出された");

			//ゲームオーバーの時のBGMをストップする
			SoundManager.instance.musicSource.Stop ();

			Debug.Log ("遷移前");

			SceneManager.LoadScene ("ResultClear");

			Debug.Log ("遷移後");

			//enabledをfalseにすることで、GameManagerが無効になる
			enabled = false;

			Debug.Log ("GameClear後、GameManagerを無効にした");

		}

	}

}