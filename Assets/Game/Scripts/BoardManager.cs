using UnityEngine;
using System;
using System.Collections.Generic; 		//配列を使う
using Random = UnityEngine.Random; 		//ランダム要素を使う

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{
		//Inspectorに表示する
		[Serializable]
		public class Count
		{
			public int minimum;
			public int maximum;
			
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		
		
		public int columns = 8; 										//ステージの横幅
		public int rows = 8;											//ステージの縦幅
		public Count wallCount = new Count (5, 8);						//壁の個数の最小と最大を決める
		public Count foodCount = new Count (1, 2);						//食料の個数の最小と最大を決める
		public GameObject exit;											//終了するために生成するプレハブ。
		public GameObject[] floorTiles;									//床プレハブの配列。
		public GameObject[] wallTiles;									//壁プレハブの配列。
		public GameObject[] foodTiles;									//食べ物プレハブの配列。
		public GameObject[] enemyTiles;									//敵プレハブの配列。
		public GameObject[] outerWallTiles;								//外側の壁プレハブの配列。
		
		private Transform boardHolder;									//オブジェクトの位置を保存
		private List <Vector3> gridPositions = new List <Vector3> ();	//オブジェクトを配置できる範囲を指定
		
		
		//敵やアイテム、内側の壁(Wall)を配置できる位置を特定
		void InitialiseList ()
		{
			//gridPositionsを初期化
			gridPositions.Clear ();
			
			//gridPositionsに敵やアイテムを配置できる範囲を設定する
			//横方向の範囲
			for(int x = 1; x < columns-1; x++)
			{
				//縦方向の範囲
				for(int y = 1; y < rows-1; y++)
				{
					//範囲をgridPositionsに追加していく
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
		
		//外側の壁(OuterWall)、床を配置する
		void BoardSetup ()
		{
			//boradHolderを位置情報を持つゲームオブジェクト(Board)として初期化する
			boardHolder = new GameObject ("Board").transform;

			for(int x = -1; x < columns + 1; x++)
			{
				
				for(int y = -1; y < rows + 1; y++)
				{
					//床をランダムに選択
					GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

					if(x == -1 || x == columns || y == -1 || y == rows)

						//ステージの外周には、OuterWallを選択する
						toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
					//選択した床や、壁を実際に生成する
					GameObject instance =
						Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
					//生成したオブジェクトをBoardオブジェクトの子オブジェクトとする
					instance.transform.SetParent (boardHolder);

				}

			}

		}
		
		
		//ランダムな位置を返す
		Vector3 RandomPosition ()
		{
			//返す位置の範囲は、アイテムを配置できる範囲
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//ランダムな位置の座標を取得
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//重複しないように、取得した場所は削除する
			gridPositions.RemoveAt (randomIndex);
			
			//位置の座標を返す
			return randomPosition;
		}
		
		
		//敵やアイテム、内側の壁(Wall)を配置する　※SetupSceneから呼ばれる
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//ランダムな回数ループする
			int objectCount = Random.Range (minimum, maximum+1);

			for(int i = 0; i < objectCount; i++)
			{
				//gridPositionからランダムな位置を取得
				Vector3 randomPosition = RandomPosition();
				
				//tileArrayからオブジェクトの種類をランダムに選択
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//ランダムで決めた位置に、ランダムに決めたオブジェクトを配置
				Instantiate(tileChoice, randomPosition, Quaternion.identity);

			}

		}
		
		
		//レベルに応じてステージを作成する　※GameManager.csから呼ばれる
		public void SetupScene (int level)
		{
			//外側の壁(OuterWall)、床を配置する
			BoardSetup ();
			
			//敵やアイテム、内側の壁(Wall)を配置できる位置を特定
			InitialiseList ();
			
			//敵やアイテム、内側の壁(Wall)を配置する
			LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);


			//敵の数をLogで計算
			int enemyCount = (int)Mathf.Log(level, 2f);

			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
			//出口を右上に配置する
			Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);

		}

	}

}