using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//UIを使うのに必要
using UnityEngine.SceneManagement;//シーンの読み込みに必要

namespace Completed
{
	//MovingObjectクラスを継承する
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//ステージ移動時の時間
		public int pointsPerFood = 10;				//食べ物の回復量
		public int pointsPerSoda = 20;				//ソーダの回復量
		public int wallDamage = 1;					//壁へのダメージ量
		public Text foodText;						//食料を表示するテキスト

		//効果音
		public AudioClip moveSound1;				//1 of 2 Audio clips to play when player moves.
		public AudioClip moveSound2;				//2 of 2 Audio clips to play when player moves.
		public AudioClip eatSound1;					//1 of 2 Audio clips to play when player collects a food object.
		public AudioClip eatSound2;					//2 of 2 Audio clips to play when player collects a food object.
		public AudioClip drinkSound1;				//1 of 2 Audio clips to play when player collects a soda object.
		public AudioClip drinkSound2;				//2 of 2 Audio clips to play when player collects a soda object.
		public AudioClip gameOverSound;				//Audio clip to play when player dies.
		
		private Animator animator;					//アニメーション用変数
		private int food;                           //食料

#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        private Vector2 touchOrigin = -Vector2.one;	//Used to store location of screen touch origin for mobile controls.
#endif
		
		
		//MovingObjectクラスのStartを継承する
		protected override void Start ()
		{
			//animatorのコンポーネントを設定
			animator = GetComponent<Animator>();
			
			//foodをステージ間で引き継げるように、GameManagerから設定
			food = GameManager.instance.playerFoodPoints;
			
			//foodTextを初期化.
			foodText.text = "Food: " + food;
			
			//GameObjectのStartを呼び出す
			base.Start ();
		}
		
		
		//PlayerのfoodをGameManageに保存する
		private void OnDisable ()
		{
			//Playerオブジェクトが無効になっているときは、
			//現在のローカルフードの合計をGameManagerに保存して、次のレベルで再ロードできるようにする
			GameManager.instance.playerFoodPoints = food;
		}
		
		
		private void Update ()
		{
			//プレイヤーのターンではない場合、何も実行しない
			if(!GameManager.instance.playersTurn) return;

			int horizontal = 0;  	//左右の移動
			int vertical = 0;		//上下の移動
			
			//Check if we are running either in the Unity editor or in a standalone build.
#if UNITY_STANDALONE || UNITY_WEBPLAYER
			
			//左右の移動量を受け取る
			horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
			
			//上下の移動量を受け取る
			vertical = (int) (Input.GetAxisRaw ("Vertical"));
			
			//上下左右のいずれかに移動を制限する
			if(horizontal != 0)
			{
				vertical = 0;
			}
			//Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
			
			//Check if Input has registered more than zero touches
			if (Input.touchCount > 0)
			{
				//Store the first touch detected.
				Touch myTouch = Input.touches[0];
				
				//Check if the phase of that touch equals Began
				if (myTouch.phase == TouchPhase.Began)
				{
					//If so, set touchOrigin to the position of that touch
					touchOrigin = myTouch.position;
				}
				
				//If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
				else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
				{
					//Set touchEnd to equal the position of this touch
					Vector2 touchEnd = myTouch.position;
					
					//Calculate the difference between the beginning and end of the touch on the x axis.
					float x = touchEnd.x - touchOrigin.x;
					
					//Calculate the difference between the beginning and end of the touch on the y axis.
					float y = touchEnd.y - touchOrigin.y;
					
					//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
					touchOrigin.x = -1;
					
					//Check if the difference along the x axis is greater than the difference along the y axis.
					if (Mathf.Abs(x) > Mathf.Abs(y))
						//If x is greater than zero, set horizontal to 1, otherwise set it to -1
						horizontal = x > 0 ? 1 : -1;
					else
						//If y is greater than zero, set horizontal to 1, otherwise set it to -1
						vertical = y > 0 ? 1 : -1;
				}
			}
			
#endif //End of mobile platform dependendent compilation section started above with #elif

			//左右上下のいずれかに移動する場合
			if(horizontal != 0 || vertical != 0)
			{
				//プレイヤーの侵攻方向に壁があるか確認
				AttemptMove<Wall> (horizontal, vertical);
			}
		}
		
		//AttemptMoveは、基本クラスMovingObjectのAttemptMove関数をオーバーライドします。
		//AttemptMoveは一般的なパラメータTを取ります。このパラメータはPlayerの場合はWall型になります。移動するx方向とy方向の整数も取ります。
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
			//移動するたびに食料が減る
			food--;
			
			//減らした食料をUIに表示する
			foodText.text = "Food: " + food;
			
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
		
		
		//プレイヤーが壁にぶつかった場合、壁をチョップする
		protected override void OnCantMove <T> (T component)
		{
			//Wallスクリプトを使えるように設定
			Wall hitWall = component as Wall;
			
			//壁にダメージを与える
			hitWall.DamageWall (wallDamage);
			
			//チョップするアニメーションを呼び出す
			animator.SetTrigger ("playerChop");
		}
		
		
		//プレイヤーが、Exit、Food、Sodaと接触した場合に呼び出す
		private void OnTriggerEnter2D (Collider2D other)
		{
			//Exitと接触した場合
			if(other.tag == "Exit")
			{
				//ステージ移動の時間分待ってから、次のツテージに移動する
				Invoke ("Restart", restartLevelDelay);
				
				//レベルが終わったのでプレイヤーオブジェクトを無効にする
				enabled = false;
			}
			
			//食料と接触した場合
			else if(other.tag == "Food")
			{
				//食料を回復
				food += pointsPerFood;
				
				//増えた食料をUIに表示する
				foodText.text = "+" + pointsPerFood + " Food: " + food;
				
				//食べた時の効果音を鳴らす
				SoundManager.instance.RandomizeSfx (eatSound1, eatSound2);
				
				//食料を削除
				other.gameObject.SetActive (false);
			}
			
			//Check if the tag of the trigger collided with is Soda.
			else if(other.tag == "Soda")
			{
				//食料を回復
				food += pointsPerSoda;
				
				//増えた食料をUIに表示する
				foodText.text = "+" + pointsPerSoda + " Food: " + food;
				
				//飲んだ時の効果音を鳴らす
				SoundManager.instance.RandomizeSfx (drinkSound1, drinkSound2);
				
				//ソーダを削除
				other.gameObject.SetActive (false);
			}
		}
		
		
		//プレイヤーがExitに到達した場合、次のステージを呼び出す
		private void Restart ()
		{
			//シーンを呼び直す
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
		}
		
		
		//プレイヤーが敵に攻撃された場合、食料を減らす
		public void LoseFood (int loss)
		{
			//攻撃を受けたアニメーションを呼び出す
			animator.SetTrigger ("playerHit");
			
			//食料を減らす
			food -= loss;
			
			//減らした食料をUIに表示する
			foodText.text = "-"+ loss + " Food: " + food;
			
			//ゲームオーバーか判定
			CheckIfGameOver ();
		}
		
		
		//食料が0いかになった場合、ゲームオーバーにする
		private void CheckIfGameOver ()
		{
			//食料の合計が0以下かどうかを確認
			if (food <= 0) 
			{
				//SoundManagerのPlaySingle関数を呼び出して、再生するオーディオクリップとしてgameOverSoundを渡します。
				SoundManager.instance.PlaySingle (gameOverSound);

				//ゲームオーバーの時の効果音を鳴らす
				SoundManager.instance.musicSource.Stop();
				
				//GameManagerのGameOverを呼び出す
				GameManager.instance.GameOver ();
			}
		}
	}
}

