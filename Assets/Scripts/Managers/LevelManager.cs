using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;
using System.Linq;

namespace FantasyErrand
{
    public enum PatternType
    {
        Straight, LeftCorner, RightCorner
    }

	public enum Difficulty
	{
		Easy,Medium,Hard
	}

	public enum PathType
	{
		Coin,Obstacle
	}

    public class LevelManager : MonoBehaviour
    {
        Queue<GameObject> spawnedTiles = new Queue<GameObject>();
        List<IObstacleMovable> movableObstacles = new List<IObstacleMovable>();

        public Player player;
        public GameObject startPrefab, straightPrefab;
        public GameObject[] obstaclePrefab;
        public Vector3 startPosition;

        [Header("Tile Generation")]
        public int maxGeneratedTile = 10;
        public int startTiles = 2;
        public float tileScale;

        [Header("Obstacles")]
        public float baseObstacleRatio = 100;

        [Header("Debug")]
        public bool showGizmos = true;
        
        //public UnityEngine.UI.Text text;

        int patternSpawned;

        // Use this for initialization

		public GameObject[] coinPrefab;
		Queue<GameObject> tempTile = new Queue<GameObject> ();
		public List<GameObject> obstacleList;
		public List<GameObject> coinList;
		public Queue<GameObject> straightPrefabQueue= new Queue<GameObject>();
		public bool SpawnedObstacle;
		public int prefabMultiplier=0;
        void Start()
        {
			if (SpawnedObstacle) 
			{
				SetListPrefab ();
				StartCoroutine (Generate ());
			}
			else 
			{	
				setStraightPrefab();
				StartCoroutine(generateStraightPath());
			}
	 	}

		void setStraightPrefab(){
			for(int i =0; i<30; i++)
			{
				GameObject a =Instantiate(straightPrefab, new Vector3(0,0,0), Quaternion.identity);
				a.SetActive(false);
				straightPrefabQueue.Enqueue(a);
			}
            print($"Queue Count: {straightPrefabQueue.Count}");
		}
        
        IEnumerator Generate()
        {
            int tiles = 0;
            bool spawnedStart = false;
            Vector3 spawnPos = startPosition;
            while(true)
            {
                if (spawnedStart && tiles < startTiles)
                {
                    GameObject obj = Instantiate(startPrefab, new Vector3(spawnPos.x, -0.5f, spawnPos.z), Quaternion.identity);
                    spawnedTiles.Enqueue(obj);
                    tiles++;
                    spawnPos += Vector3.forward * tileScale;
                }
                else
                {
                    spawnedStart = true;
                    if(Vector3.Distance(spawnPos, player.transform.position) < maxGeneratedTile * tileScale)
                    {
                        //GameObject obj = Instantiate(straightPrefab, new Vector3(spawnPos.x, -0.5f, spawnPos.z), Quaternion.identity);
						GameObject obj = ReturnPath(Difficulty.Easy);
						obj.SetActive(true);
						obj.transform.position=new Vector3(spawnPos.x,-0.5f,spawnPos.z);
						obj.transform.rotation=Quaternion.identity;
						spawnedTiles.Enqueue(obj);
                        tiles++;
                        spawnPos += Vector3.forward * tileScale;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

		IEnumerator generateStraightPath()
		{
			Vector3 spawnPos = startPosition;
			while (true) {
				if(Vector3.Distance(spawnPos, player.transform.position) < maxGeneratedTile * tileScale)
				{
					GameObject obj = straightPrefabQueue.Dequeue();
					obj.SetActive(true);
					obj.transform.position=new Vector3(spawnPos.x,-0.5f,spawnPos.z);
					spawnedTiles.Enqueue(obj);
					spawnPos += Vector3.forward * tileScale;
				}
				yield return new WaitForEndOfFrame();
			}
		}


        private void OnDrawGizmos()
        {
            if (showGizmos)
            {
                float distance = maxGeneratedTile * tileScale;
                Gizmos.color = Color.green;
                Gizmos.DrawLine(player.transform.position, player.transform.position + Vector3.forward * distance);
                Gizmos.DrawLine(player.transform.position, player.transform.position + Vector3.back * distance);
                Gizmos.color = new Color(1, 0, 0, 0.33f);
                Gizmos.DrawWireSphere(player.transform.position, distance);
            }

        }

        private void Update()
        {
            if (spawnedTiles.Count > 0 && SpawnedObstacle) {
				if (Vector3.Distance (player.transform.position, spawnedTiles.Peek ().transform.position) > maxGeneratedTile * tileScale) {
					GameObject obj = spawnedTiles.Dequeue ();
					obj.SetActive (false);
					if (obj.CompareTag ("BarrierPath"))
						obstacleList.Add (obj);
					else
						coinList.Add (obj);
				}
			} else if(!SpawnedObstacle) {
				if (Vector3.Distance (player.transform.position, spawnedTiles.Peek ().transform.position) > maxGeneratedTile * tileScale) {
					GameObject obj = spawnedTiles.Dequeue ();
					obj.SetActive (false);
					straightPrefabQueue.Enqueue(obj);
				}
			}
        }

		//Instantiate all prefab on start
		public void SetListPrefab(){
			for (int i =0; i<obstaclePrefab.Length; i++) {
				for(int x=0; x<prefabMultiplier;x++)
				{
					GameObject a = Instantiate(obstaclePrefab[i],new Vector3(10,10,10),Quaternion.identity);
					a.SetActive(false);
					obstacleList.Add(a);
				}
			}
			for (int i =0; i<coinPrefab.Length; i++) {
				for(int x = 0 ; x<prefabMultiplier ;x++)
				{
					GameObject a = Instantiate(coinPrefab[i],new Vector3(10,10,10),Quaternion.identity);
					a.SetActive(false);
					coinList.Add(a);
				}
			}
		}

		//selecting path 
		public GameObject AddPath(PathType pathType){
			int rand;
			if (pathType==PathType.Obstacle) {
				rand = (int)Random.Range(0,obstacleList.Count);
				GameObject path = obstacleList [rand];
				obstacleList.RemoveAt(rand);
				return path;
			} else {
				rand =(int) Random.Range(0,coinList.Count);
				GameObject path = coinList [rand];
				coinList.RemoveAt(rand);
				return path;
			}
		}

		public void FillTheList(Difficulty dif,ref List<GameObject>a)
		{
			//Easy= 4 coin 2 obstacle, Medium = 2 coin 4 obstacle, Hard = 0 coin 6 obstacle
			for (int i=0; i<6; i++) 
			{
				if(dif==Difficulty.Easy)
				{
					if (i < 2)
						a.Add (AddPath (PathType.Obstacle));
					if (i < 6)
						a.Add (AddPath (PathType.Coin));
				}
				else if(dif==Difficulty.Medium)
				{
					if (i < 2)
						a.Add (AddPath (PathType.Coin));
					if (i < 6)
						a.Add (AddPath (PathType.Obstacle));
				}
				else if(dif==Difficulty.Hard)
				{
					a.Add(AddPath(PathType.Obstacle));
				}	
			}
		}

		public GameObject ReturnPath(Difficulty dif)
		{
			if (tempTile.Count == 0) {
					List<GameObject>tileList= new List<GameObject>();
					FillTheList(dif,ref tileList);
					//shuffle isi list
					GameObject[]temp = tileList.ToArray();
					MathRand.Shuffle<GameObject>(ref temp);
					tileList=temp.ToList();
					int size = tileList.Count;
					//insert the tile into queue
					for(int i =0 ; i<size;i++)
					{
						tempTile.Enqueue(tileList[0]);
						tileList.RemoveAt(0);
					}
				GameObject data = (GameObject) tempTile.Dequeue();
				return data;
			}
			//Return tile on the queue 
			else{
				GameObject data = (GameObject)tempTile.Dequeue();
				return data;
			}

		}

    }
}