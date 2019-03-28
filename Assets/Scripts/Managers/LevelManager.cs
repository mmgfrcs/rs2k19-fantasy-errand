using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FantasyErrand.Entities;
using FantasyErrand.Entities.Interfaces;

namespace FantasyErrand
{
    public enum PatternType
    {
        Straight, Junction, LeftCorner, RightCorner
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
        
        //public UnityEngine.UI.Text text;

        int patternSpawned;

        // Use this for initialization

		public GameObject[] coinPrefab;
		Queue<GameObject> tempTile = new Queue<GameObject> ();
		public List<GameObject> obstacleList;
		public List<GameObject> coinList;
		public int spawnCounter=0;
        void Start()
        {
			setListPrefab ();
            StartCoroutine(Generate());
        }
        
        IEnumerator Generate()
        {
            int tiles = 0;
            bool spawnedStart = false;
            Vector3 spawnPos = startPosition;
            while(true)
            {
                if (!spawnedStart && tiles < startTiles)
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
						GameObject obj = returnPath("Easy");
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


        private void OnDrawGizmos()
        {
            float distance = maxGeneratedTile * tileScale;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(player.transform.position, player.transform.position + Vector3.forward * distance);
            Gizmos.DrawLine(player.transform.position, player.transform.position + Vector3.back * distance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, distance);
        }

        private void Update()
        {
            if(spawnedTiles.Count > 0)
            {
                if (Vector3.Distance(player.transform.position, spawnedTiles.Peek().transform.position) > maxGeneratedTile * tileScale)
				{
					GameObject obj =spawnedTiles.Dequeue();
					obj.SetActive(false);
					if(obj.CompareTag("Obstacle"))
						obstacleList.Add(obj);
					else
						coinList.Add(obj);
				}
            }
        }

		//Instantiate all prefab on start
		public void setListPrefab(){
			for (int i =0; i<obstaclePrefab.Length; i++) {
				for(int x=0; x<7;x++)
				{
					GameObject a = Instantiate(obstaclePrefab[i],new Vector3(10,10,10),Quaternion.identity);
					a.SetActive(false);
					obstacleList.Add(a);
				}
			}
			for (int i =0; i<coinPrefab.Length; i++) {
				for(int x = 0 ; x<7 ;x++)
				{
					GameObject a = Instantiate(coinPrefab[i],new Vector3(10,10,10),Quaternion.identity);
					a.SetActive(false);
					coinList.Add(a);
				}
			}
		}

		//selecting path 
		public GameObject addPath(string pathType){
			int rand;
			if (pathType == "Obstacle") {
				rand = (int)Random.Range(0,obstacleList.Count-1);
				GameObject path = obstacleList [rand];
				obstacleList.RemoveAt(rand);
				return path;
			} else {
				rand =(int) Random.Range(0,coinList.Count-1);
				GameObject path = coinList [rand];
				coinList.RemoveAt(rand);
				return path;
			}
		}

		//Shuffling the list
		public List<GameObject>listShuffle(List<GameObject> some){
			for (int i=0; i<some.Count; i++) {
				GameObject temp = some[i];
				int rand = Random.Range(i,some.Count);
				some[i]=some[rand];
				some[rand]=temp;
			}
			return some;
		}

		public GameObject returnPath(string difficulty)
		{
			if (spawnCounter == 0) {
				List<GameObject>tileList= new List<GameObject>();
					for(int i =0;i<2;i++){
						tileList.Add(addPath("Obstacle"));
						spawnCounter++;
					}
					for(int i=0;i<3;i++){
						tileList.Add(addPath("Coin"));
						spawnCounter++;
					}
					//shuffle isi list
					tileList=listShuffle(tileList);
					int size = tileList.Count;
					//insert the tile into queue
					for(int i =0 ; i<size;i++){
						tempTile.Enqueue(tileList[0]);
						tileList.RemoveAt(0);
					}
				Debug.Log("Tile Size = "+tileList.Count+"Spawn = "+spawnCounter+"Queue Size "+tempTile.Count);
				spawnCounter--;
				GameObject data = (GameObject) tempTile.Dequeue();
				return data;
			}
			//Return tile on the queue 
			else{
				spawnCounter--;
				GameObject data = (GameObject)tempTile.Dequeue();
				return data;
			}

		}

    }
}