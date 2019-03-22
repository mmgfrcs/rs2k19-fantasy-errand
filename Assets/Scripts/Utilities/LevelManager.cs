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
        void Start()
        {
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
                        GameObject obj = Instantiate(straightPrefab, new Vector3(spawnPos.x, -0.5f, spawnPos.z), Quaternion.identity);
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
                    Destroy(spawnedTiles.Dequeue());
            }
        }
    }
}