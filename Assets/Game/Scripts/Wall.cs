using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
		//効果音
		public AudioClip chopSound1;
		public AudioClip chopSound2;

		//sprite = 画像
		public Sprite dmgSprite;					//攻撃された時の壁の画像
		public int hp = 3;							//壁のHP
		
		
		private SpriteRenderer spriteRenderer;		//画像を表示するコンポーネント
		
		
		void Awake ()
		{
			//コンポーネントを読み込む
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		
		
		//壁が攻撃されたときに呼ばれる
		public void DamageWall (int loss)
		{
			//
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
			//攻撃された時の画像を表示.
			spriteRenderer.sprite = dmgSprite;
			
			//HPを減らす
			hp -= loss;
			
			//hpが0以下になった場合
			if(hp <= 0)
				
				//壁を無効にする
				gameObject.SetActive (false);
			
		}

	}

}