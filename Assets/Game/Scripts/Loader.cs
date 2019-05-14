using UnityEngine;
using System.Collections;

namespace Completed
{	
	public class Loader : MonoBehaviour 
	{
		
		public GameObject gameManager;			//GameManager変数を宣言
		public GameObject soundManager;			//SoundManager変数を宣言
		
		// 最初に呼び出される関数
		void Awake ()
		{
			//GameManagerが存在しなければ
			if (GameManager.instance == null)
				
				//GameManagerを作成する　※InspectorでGameObjectにGameManagerを設定する
				Instantiate(gameManager);
			
			//SoundManagerが存在しなければ
			if (SoundManager.instance == null)
				
				//SoundManagerを作成する　※InspectorでGameObjectにSoundManagerを設定する
				Instantiate(soundManager);
		}
	}
}