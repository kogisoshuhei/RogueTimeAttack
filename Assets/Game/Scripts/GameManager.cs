using UnityEngine;
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
		private int level = 5;									//階層
		private List<Enemy> enemies;							//複数の敵を管理
		private bool enemiesMoving;								//敵の移動フラグ


		// AwakeはStartよりも前、最初に呼ばれる
		void Awake()
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
			instance.level--;
			instance.InitGame();
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
			levelText.text = "脱出まで残り " + level + " F";

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
			enemiesMoving = false;

		}

		//ゲームオーバー時の表示
		public void GameOver()
		{
			//ゲームオーバー時のテキストを設定
			levelText.text = "プレイヤーは " + level + " で生き絶えた";
			//UIを表示
			levelImage.SetActive(true);

			//enabledをfalseにすることで、GameManagerが無効になる
			enabled = false;

		}

		//ゲームクリア時の表示
		public void GameClear()
		{
			if ( level == 1 ) {
				
				//ゲームクリア時のテキストを設定
				levelText.text = "プレイヤーは無事に生還した!";
				//UIを表示
				levelImage.SetActive (true);

			}

		}

	}

}