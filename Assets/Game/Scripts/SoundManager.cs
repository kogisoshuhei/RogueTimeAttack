using UnityEngine;
using System.Collections;

namespace Completed
{
	public class SoundManager : MonoBehaviour 
	{
		public AudioSource efxSource;					//効果音
		public AudioSource musicSource;					//BGM
		public static SoundManager instance = null;

		//音の速さの範囲
		public float lowPitchRange = .95f;
		public float highPitchRange = 1.05f;
		
		
		void Awake ()
		{
			//シングルトンにする
			if (instance == null)
				
				instance = this;
			
			else if (instance != this)
				
				Destroy (gameObject);
			
			//シーンロード時は破壊しない
			DontDestroyOnLoad (gameObject);
		}
		
		
		//効果音を鳴らす処理.
		public void PlaySingle(AudioClip clip)
		{
			
			efxSource.clip = clip;
			
			//音を再生する
			efxSource.Play ();
		}
		
		
		//ランダムな音を、ランダムな長さで鳴らす　※効果音を鳴らす
		public void RandomizeSfx (params AudioClip[] clips)
		{
			//音をランダムに選択
			int randomIndex = Random.Range(0, clips.Length);
			
			//長さをランダムに選択
			float randomPitch = Random.Range(lowPitchRange, highPitchRange);
			
			//音の長さを設定
			efxSource.pitch = randomPitch;
			
			//鳴らす音を設定
			efxSource.clip = clips[randomIndex];
			
			//音を鳴らす
			efxSource.Play();
		}
	}
}
