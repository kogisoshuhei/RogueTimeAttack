﻿using UnityEngine;
using System.Collections;

namespace Completed
{
	//MovingObjectを継承する
	public class Enemy : MovingObject
	{
		
		public int playerDamage; 			//プレイヤーへのダメージ

		//敵の攻撃の効果音
		public AudioClip attackSound1;
		public AudioClip attackSound2;
		
		private Animator animator;
		private Transform target;			//プレイヤー場所
		private bool skipMove;				//動くかどうかの判定

		//20190515追加
		public int hp = 2;					//モンスターHP

		public int enemyAllPoint = 0;		//モンスターポイント
		public int enemyPoint = 100;		//モンスターポイント


		//継承クラス
		protected override void Start ()
		{

			Debug.Log ("初期設定");

			//敵をリストに加える
			GameManager.instance.AddEnemyToList (this);
			
			//Animatorコンポーネント取得
			animator = GetComponent<Animator> ();
			
			//プレイヤーの場所を取得する
			target = GameObject.FindGameObjectWithTag ("Player").transform;
			
			//MovingObjectクラスのStartを呼び出す
			base.Start ();

		}
		
		//敵のターンか判定、移動を試みる処理
		protected override void AttemptMove <T> (int xDir, int yDir)
		{

			Debug.Log ("敵のターン開始");

			//敵のターンではない場合
			if(skipMove)
			{

				Debug.Log ("敵のターンじゃない");

				skipMove = false;
				return;
				
			}
			
			//MovingObjectからAttemptMove関数を呼び出す
			base.AttemptMove <T> (xDir, yDir);
			
			//敵のターンを終わる
			skipMove = true;

			Debug.Log ("敵のターン終了");

		}
		
		
		//敵を移動させる処理
		public void MoveEnemy ()
		{
			int xDir = 0;			//左右の移動量
			int yDir = 0;			//上下の移動量

			Debug.Log ("敵の移動開始");

			//敵とプレイヤーの左右の距離がほぼ0の場合
			if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				
				//プレイヤーが上にいれば、1とし、下にいれば、-1とする
				yDir = target.position.y > transform.position.y ? 1 : -1;
			
			else
				//プレイヤーが右にいれば、1とし、左にいれば、-1とする
				xDir = target.position.x > transform.position.x ? 1 : -1;
			
			//移動する
			AttemptMove <Player> (xDir, yDir);

			Debug.Log ("敵の移動終了");

		}
		
		
		//敵が攻撃する時に呼び出す
		protected override void OnCantMove <T> (T component)
		{

			Debug.Log ("敵が攻撃をする");

			//衝突したプレイヤーを設定
			Player hitPlayer = component as Player;

			if( hp > 0){

				Debug.Log ("敵が生きてるので攻撃する");

				//プレイヤーのHPを減らす
				hitPlayer.LoseFood (playerDamage);

				//プレイヤーに攻撃するアニメーションを呼び出す
				animator.SetTrigger ("enemyAttack");

				//攻撃する効果音を鳴らす
				SoundManager.instance.RandomizeSfx (attackSound1, attackSound2);

			}

		}

		//20190515追加
		//敵が攻撃されたときに呼ばれる
		public void Damage (int loss)
		{

			Debug.Log ("敵が攻撃を受けた");

			//HPを減らす
			hp -= loss;

			//チョップするアニメーションを呼び出す
			animator.SetTrigger ("enemyHit");

			Debug.Log ("敵のHPは " + hp);

			//hpが0以下になった場合
			if(hp <= 0){

				gameObject.SetActive (false);

				Debug.Log ("敵消える");

			}

		}

	}

}