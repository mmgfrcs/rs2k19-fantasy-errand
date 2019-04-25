using FantasyErrand.Entities.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour {

    public GameObject[] PooledObjects { get { return objectPrefab; } }

    GameObject[] objectPrefab;
    int initialPoolSize = 20;

    Stack<GameObject> objects = new Stack<GameObject>();

    GameObject[] debugObjects;

    Vector3 poolPos = new Vector3(0, -10000, 0);

    bool init = false;
    public int instantiatedObjects = 0;
    public List<float> obstacleSpawnRate = new List<float>();

    bool IsInitialized()
    {
        if (init) return true;
        else
        {
            Debug.LogWarning("ObjectPooler has not been initialized. Please call ObjectPooler.Initialize() before calling any other ObjectPooler functions");
            return false;
        }
    }

    public GameObject Instantiate(Vector3 pos)
    {
        if (IsInitialized())
        {
            instantiatedObjects++;
            if (objects.Count > 0 && obstacleSpawnRate.Count==0)
            {
                GameObject go = objects.Pop();
                go.transform.position = pos;
                go.SetActive(true);
                return go;
            }
            else if (objects.Count > 0 && obstacleSpawnRate.Count != 0)
            {
                int searchIndex = MathRand.WeightedPick(obstacleSpawnRate)+1;
                int pickIndex = obstacleSpawnRate.Count - searchIndex;
                Stack<GameObject> tempStack = new Stack<GameObject>();
                obstacleSpawnRate.RemoveAt(searchIndex-1);
                for (int i = 0; i < pickIndex; i++)
                {
                    tempStack.Push(objects.Pop());
                }
                GameObject go = objects.Pop();
                while (tempStack.Count != 0)
                {
                    objects.Push(tempStack.Pop());
                }
                go.transform.position = pos;
                go.SetActive(true);
                return go;

            }
            else
            {
                GameObject go = Instantiate(MathRand.Pick(objectPrefab), pos, Quaternion.identity);
                IObstacle obs = go.GetComponent<IObstacle>();
                if (obs != null)
                {
                    obstacleSpawnRate.Add(obs.SpawnRate);
                }
                return go;
            }
            
        }
        else return null;
        
    }

    public void Destroy(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.position = poolPos;
        objects.Push(obj);
        IObstacle obs = obj.GetComponent<IObstacle>();
        if (obs != null)
        {
            obstacleSpawnRate.Add(obs.SpawnRate);
        }
        instantiatedObjects--;
    }

    // Use this for initialization
    public void Initialize(int poolsize, params GameObject[] prefabs)
    {
        objectPrefab = prefabs;
        initialPoolSize = poolsize;

        for(int i = 0; i < initialPoolSize; i++)
        {
            GameObject go = Instantiate(MathRand.Pick(objectPrefab), poolPos, Quaternion.identity);
            go.SetActive(false);
            IObstacle obs = go.GetComponent<IObstacle>();
            if (obs != null)
            {
                obstacleSpawnRate.Add(obs.SpawnRate);
            }
            objects.Push(go);
        }

        init = true;
    }

    void Update()
    {
        debugObjects = objects.ToArray();
    }
}
