﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//UIを使うのに必要
using UnityEngine.SceneManagement;//シーンの読み込みに必要

namespace Completed
{
	//MovingObjectクラスを継承する
	public class Player : MovingObject
	{

		public GameObject playerHp1; 				// プレイヤー残り体力1を示すUI
		public GameObject playerHp2; 				// プレイヤー残り体力2を示すUI
		public GameObject playerHp3; 				// プレイヤー残り体力3を示すUI

		public float restartLevelDelay = 1f;		//ステージ移動時の時間
		public int pointsPerFood = 1;				//食べ物の回復量
		public int enemyDamage = 1;					//敵へのダメージ量

		//効果音
		public AudioClip moveSound1;				
		public AudioClip moveSound2;				
		public AudioClip eatSound1;					
		public AudioClip eatSound2;					
		public AudioClip drinkSound1;				
		public AudioClip drinkSound2;				
		public AudioClip gameOverSound;				
		
		private Animator animator;					//アニメーション用変数
		private int hp;                           	//プレイヤーのHP

		//タッチ位置の初期位置 Vector2(-1.0, -1.0)
//        private Vector2 touchOrigin = -Vector2.one;	

		//MovingObjectクラスのStartを継承する
		protected override void Start ()
		{

			//animatorのコンポーネントを設定
			animator = GetComponent<Animator>();
			
			//hpをステージ間で引き継げるように、GameManagerから設定
			hp = GameManager.instance.playerHp;

			//各階層での体力アイコンを引き継ぎ
			//残り体力を表示
			if (hp == 2) // 体力2の場合
			{ 
				playerHp3.SetActive(false);

			}
			else if (hp == 1) // 体力1の場合
			{
				playerHp3.SetActive(false);
				playerHp2.SetActive(false);

			}
			
			//GameObjectのStartを呼び出す
			base.Start ();

		}
		
		
		//PlayerのfoodをGameManageに保存する
		private void OnDisable ()
		{
			//Playerオブジェクトが無効になっているときは、
			//現在のローカルフードの合計をGameManagerに保存して、次のレベルで再ロードできるようにする
			GameManager.instance.playerHp = hp;

		}
		
		
		private void Update ()
		{



			//プレイヤーのターンではない場合、何も実行しない
			if (!GameManager.instance.playersTurn) {

				return;
			
			}

			int horizontal = 0;  	//左右の移動
			int vertical = 0;		//上下の移動

			/////

			// //矢印操作


			horizontal = (int)(Input.GetAxisRaw("Horizontal"));
			vertical = (int)(Input.GetAxisRaw("Vertical"));

			if (horizontal != 0) {
				
				vertical = 0;

			}

			/////

			//タッチされた場合
			/*
			//タッチ数が1以上、つまり画面がタッチされたら
			if (Input.touchCount > 0)
			{
				//最初にタッチされた情報を取得
				Touch myTouch = Input.touches[0];

				//タッチ開始時
				if (myTouch.phase == TouchPhase.Began)
				{
					//タッチした場所を取得
					touchOrigin = myTouch.position;

				}

				//指を離した時かつ指の場所が0以上の場合
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//指を離した場所を取得
					Vector2 touchEnd = myTouch.position;

					//横方向の移動量を取得
					float x = touchEnd.x - touchOrigin.x;

					//縦方向の移動量を取得
					float y = touchEnd.y - touchOrigin.y;


					touchOrigin.x = -1;

					//横と縦の移動量が大きい方に移動する
					if (Mathf.Abs(x) > Mathf.Abs(y))
						
						horizontal = x > 0 ? 1 : -1;
					
					else
						
						vertical = y > 0 ? 1 : -1;
				}
			}
			*/
			//左右上下のいずれかに移動する場合
			if(horizontal != 0 || vertical != 0)
			{
				//プレイヤーの侵攻方向に敵がいるか確認
				AttemptMove<Enemy> (horizontal, vertical);
			}

		}



		/////
		/// publicで矢印の方向を指定、ボタン側で設定する

		public void OnPressedRight()
		{

//			Debug.Log ("右を押した");

			//現在地を取得
			Vector2 start = transform.position;

			//方向を取得
			Vector2 end = start + new Vector2(1, 0);

			//blockingLayerを見るためにboxColliderを非アクティブにする
			boxCollider.enabled = false;

			//現在地と移動先の間にblockingLayerがあるか確認、ある場合取得
			RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

			if (hit) {
				Enemy hitComponent = hit.transform.GetComponent<Enemy> ();

				if (hitComponent != null) {
					OnCantMove (hitComponent);

					Debug.Log ("攻撃した");
				}

			} else 
			{
				Debug.Log ("敵がいない");

				base.StartCoroutine (SmoothMovement (end));

			}

			//boxColliderをアクティブにする
			boxCollider.enabled = true;

			//プレイヤーのターン終了
			GameManager.instance.playersTurn = false;
		
		}

		public void OnPressedLeft()
		{

//			Debug.Log ("左を押した");

			//現在地を取得
			Vector2 start = transform.position;

			//移動先を取得
			Vector2 end = start + new Vector2(-1, 0);

			//blockingLayerを見るためにboxColliderを非アクティブにする
			boxCollider.enabled = false;

			//現在地と移動先の間にblockingLayerがあるか確認、ある場合取得
			RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

			if (hit)
			{
				Enemy hitComponent = hit.transform.GetComponent<Enemy>();

				if (hitComponent != null)
				{
					OnCantMove (hitComponent);
				}
			} else 
			{
				Debug.Log ("敵がいない");

				base.StartCoroutine (SmoothMovement (end));

			}

			//boxColliderをアクティブにする
			boxCollider.enabled = true;

			//プレイヤーのターン終了
			GameManager.instance.playersTurn = false;

		}

		public void OnPressedUp()
		{

//			Debug.Log ("上を押した");

			//現在地を取得
			Vector2 start = transform.position;

			//移動先を取得
			Vector2 end = start + new Vector2(0, 1);

			//blockingLayerを見るためにboxColliderを非アクティブにする
			boxCollider.enabled = false;

			//現在地と移動先の間にblockingLayerがあるか確認、ある場合取得
			RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

			if (hit)
			{
				Enemy hitComponent = hit.transform.GetComponent<Enemy>();

				if (hitComponent != null)
				{
					OnCantMove (hitComponent);
				}

			} else 
			{

				Debug.Log ("敵がいない");

				base.StartCoroutine (SmoothMovement (end));

			}

			//boxColliderをアクティブにする
			boxCollider.enabled = true;

			//プレイヤーのターン終了
			GameManager.instance.playersTurn = false;

		}

		public void OnPressedDown()
		{
			
//			Debug.Log ("下を押した");

			//現在地を取得
			Vector2 start = transform.position;

			//移動先を取得
			Vector2 end = start + new Vector2(0, -1);

			//blockingLayerを見るためにboxColliderを非アクティブにする
			boxCollider.enabled = false;

			//現在地と移動先の間にblockingLayerがあるか確認、ある場合取得
			RaycastHit2D hit = Physics2D.Linecast(start, end, blockingLayer);

			if (hit)
			{
				Enemy hitComponent = hit.transform.GetComponent<Enemy>();

				if (hitComponent != null)
				{
					OnCantMove (hitComponent);
				}

			} else 
			{
				
				Debug.Log ("敵がいない");

				base.StartCoroutine (SmoothMovement (end));

			}

			//boxColliderをアクティブにする
			boxCollider.enabled = true;

			//プレイヤーのターン終了
			GameManager.instance.playersTurn = false;

		}


		
		//AttemptMoveは、基本クラスMovingObjectのAttemptMove関数をオーバーライドします。
		//AttemptMoveは一般的なパラメータTを取ります。このパラメータはPlayerの場合はWall型になります。移動するx方向とy方向の整数も取ります。
		protected override void AttemptMove <T> (int xDir, int yDir)
		{

			
			//MovingObjectのAttemptMoveを呼び出す
			base.AttemptMove <T> (xDir, yDir);

			//hitは、Moveで行われたLinecastの結果を参照することを可能にします。
			RaycastHit2D hit;
			
			//動いた時の効果音を鳴らす
			if (Move (xDir, yDir, out hit)) 
			{
				//Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
				SoundManager.instance.RandomizeSfx (moveSound1, moveSound2);
			}
			
			//ゲームオーバーか確認
			CheckIfGameOver ();
			
			//プレイヤーのターン終了
			GameManager.instance.playersTurn = false;
		}
		
		
		//プレイヤーが敵にぶつかった場合、敵を攻撃する
		protected override void OnCantMove <T> (T component)
		{

//			Debug.Log ("敵に接触した");

			//Enemyスクリプトを使えるように設定
			Enemy hitDamage = component as Enemy;

			//敵にダメージを与える
			hitDamage.Damage (enemyDamage);

			//チョップするアニメーションを呼び出す
			animator.SetTrigger ("playerChop");

			Debug.Log ("プレイヤーが攻撃した! " + enemyDamage + " のダメージを与えた");

		}
		
		
		//プレイヤーが、Exit、Food、Sodaと接触した場合に呼び出す
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Exitと接触した場合
			if( other.tag == "Exit" && enabled == true )
			{

				Debug.Log ("出口に到着した");

				//ステージ移動の時間分待ってから、次のステージに移動する
				Invoke ("Restart", restartLevelDelay);
				
				//レベルが終わったのでプレイヤーオブジェクトを無効にする
				enabled = false;

			}

			//食料と接触した場合
			else if(other.tag == "Food" || other.tag == "Soda")
			{

				Debug.Log ("アイテムと接触した");

				if (hp < 3) {

					// 残り体力によって非表示にすべき体力アイコンを消去する
					if (hp == 2)
					{ // 体力2になった場合
						//Destroy (playerHp3); // 3つめのアイコンを消去

						//HPを回復
						hp += pointsPerFood;

						playerHp3.SetActive(true);

					}
					else if (hp == 1)
					{ // 体力1になった場合
						//Destroy (playerHp2); // 2つめのアイコンを消去

						//HPを回復
						hp += pointsPerFood;

						playerHp2.SetActive(true);

					}

					Debug.Log ("1回復");

					//増えた食料をUIに表示する
					//hpText.text = "+"+pointsPerFood+" HP: " + hp;
			
					//回復時の効果音を鳴らす
					SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
			
					//HPを削除
					other.gameObject.SetActive (false);
				} else {
					
					Debug.Log ("回復しない");
				
				}

			}

		}

		
		//プレイヤーがExitに到達した場合、次のステージを呼び出す
		private void Restart ()
		{

			GameManager.instance.NextLevel();

			//シーンを呼び直す
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

		}
		
		
		//プレイヤーが敵に攻撃された場合、HPを減らす
		public void LoseFood (int loss)
		{
			
//			Debug.Log ("敵からの攻撃!");

			//攻撃を受けたアニメーションを呼び出す
			animator.SetTrigger ("playerHit");
			
			//HPを減らす
			hp -= loss;

			Debug.Log ("敵から " + loss + " のダメージを受けた");

			//HPカウントを呼び出す
			playerHealthCount ();

			if( hp <= 0 )
			{

				Debug.Log ("ゲームオーバーかチェック");

				//ゲームオーバーか判定
				CheckIfGameOver ();

			}

		}

		//プレイヤーの体力アイコンの表示を管理
		public void playerHealthCount(){

			// 残り体力によって非表示にすべき体力アイコンを消去する
			if (hp == 2)
			{ // 体力2になった場合
				
				// 3つめのアイコンを非アクティブにする
				playerHp3.SetActive(false);
			}
			else if (hp == 1)
			{ // 体力1になった場合
				
				// 2つめのアイコンを非アクティブにする
				playerHp2.SetActive(false);

			}
			else if (hp == 0)
			{ // 体力0になった場合
				
				// 1つめのアイコンを非アクティブにする
				playerHp1.SetActive(false);

			}

		}
		
		
		//HPが0以下になった場合、ゲームオーバーにする
		private void CheckIfGameOver ()
		{
			//クリアかどうか判定
			Debug.Log ("ゲームオーバーかどうか判定");

			//GameManagerのGameOverを呼び出す
			GameManager.instance.GameOver ();

		}

	}

}