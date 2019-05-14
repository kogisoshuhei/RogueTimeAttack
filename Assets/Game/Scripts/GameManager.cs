using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Completed
{
	using System.Collections.Generic;		//Allows us to use Lists. 
//	using UnityEngine.UI;					//Allows us to use UI.
	
	public class GameManager : MonoBehaviour
	{
		public float turnDelay = 0.1f;							//1ターンの時間
		public int playerFoodPoints = 100;						//プレイヤーの食料

		public static GameManager instance = null;				//クラスに属し、複数のシーンで使われる変数を宣言
																//Staticにすることで、他のスクリプトからも呼び出すことができます

		[HideInInspector] public bool playersTurn = true;		//プレイヤーのターンかの判定フラグ

		private BoardManager boardScript;						//BoardManager型の変数を宣言
		private int level = 1;									//敵が出現するレベル
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

		
		//Initializes the game for each level.
		void InitGame()
		{
			
			//ステージ移動時は敵をリセットする
			enemies.Clear();
			
			//ステージ生成の関数を呼ぶ
			boardScript.SetupScene(level);
			
		}

		
		//Update is called every frame.
		void Update()
		{
			//プレイヤーのターンか、敵の動いている場合は、アップデートしない
			if(playersTurn || enemiesMoving )
				
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
		
		
		//GameOver is called when the player reaches 0 food points
		public void GameOver()
		{
			
			//enabledをfalseにすることで、GameManagerが無効になる
			enabled = false;

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

	}

}