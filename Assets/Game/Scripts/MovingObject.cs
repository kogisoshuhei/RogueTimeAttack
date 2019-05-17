using UnityEngine;
using System.Collections;

namespace Completed
{
	//抽象クラスとして作成していく
	public abstract class MovingObject : MonoBehaviour
	{
		public float moveTime = 0.1f;			//動く時間
		public LayerMask blockingLayer;
		
		
		private BoxCollider2D boxCollider; 		//コンポーネント
		private Rigidbody2D rb2D;				//コンポーネント
		private float inverseMoveTime;
		
		
		//継承クラスでオーバーライドできるようにする
		protected virtual void Start ()
		{
			//コンポーネントを取得する
			boxCollider = GetComponent <BoxCollider2D> ();
			rb2D = GetComponent <Rigidbody2D> ();
			inverseMoveTime = 1f / moveTime;
		}
		
		
		//移動可能か判断する　※blockingLayerに衝突する場合、移動しない
		//boolはtrueかfalseを返す
		//outで指定した変数は、取得可能になる
		protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
		{
			//現在地を取得
			Vector2 start = transform.position;
			
			//移動先を取得
			Vector2 end = start + new Vector2 (xDir, yDir);
			
			//自身のColliderを一旦無効化する
			//※startとendの間にblockingLayerがあるか確認するときに不要なため
			boxCollider.enabled = false;
			
			//現在地と移動先の間にblockingLayerがあるか確認、ある場合取得
			hit = Physics2D.Linecast (start, end, blockingLayer);
			
			//確認が終わったため、Colliderを有効化する
			boxCollider.enabled = true;
			
			//現在地と移動先の間に、BlockingLayerがなければ移動する
			if(hit.transform == null)
			{
				
				StartCoroutine (SmoothMovement (end));
				
				//移動可能
				return true;
			}
			
			//移動不可
			return false;
		}
		
		
		//現在地から目的地までの距離を求めて移動する
		protected IEnumerator SmoothMovement (Vector3 end)
		{
			//目的地までの距離を計算 ※現在地：transform.position、目的地：end、距離を求める：.sqrMagnitude
			float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			//目的地に限りなく近くまで繰り返し実行する
			while(sqrRemainingDistance > float.Epsilon)
			{
				//1フレームあたりの移動距離を計算する ※Time.deltaTime：1フレームあたりの時間
				Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
				
				//移動距離分、移動する
				rb2D.MovePosition (newPostion);
				
				//移動した場所から、目的地までの距離を再計算する
				sqrRemainingDistance = (transform.position - end).sqrMagnitude;
				
				//移動するのに1フレーム使うため、1フレーム待機
				yield return null;
			}
		}
		
		
		//継承クラスでオーバーライドできるようにする
		//移動を試し、障害物がある場合、移動不可の場合の処理を呼び出す
		protected virtual void AttemptMove <T> (int xDir, int yDir)

			where T : Component			//後からコンポーネントを決める
		
		{
			
			RaycastHit2D hit;
			
			//Moveメソッドを実行し、移動可能か判定する
			bool canMove = Move (xDir, yDir, out hit);
			
			//hitがnullの場合 ※現在地から目的地の間にblockingLayerに該当するものがない場合
			if(hit.transform == null)
				
				return;
			
			//現在地から目的地の間にblockingLayerに該当するものがある場合
			//障害物のコンポーネントを取得
			T hitComponent = hit.transform.GetComponent <T> ();

			//障害物がある場合、移動不可の場合の処理を呼び出す
			if(!canMove && hitComponent != null)
				
				OnCantMove (hitComponent);
			
		}
		
		
		//抽象クラス 
		protected abstract void OnCantMove <T> (T component) 
			where T : Component;

		//20190515追加
		protected abstract void EnemyHere <T> (T component)
			where T : Component;
	}
}
