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
            if (objects.Count > 0)
            {
                GameObject go = objects.Pop();
                go.transform.position = pos;
                go.SetActive(true);
                return go;
            }
            else
            {
                GameObject go = Instantiate(MathRand.Pick(objectPrefab), pos, Quaternion.identity);
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
            objects.Push(go);
        }

        init = true;
    }

    void Update()
    {
        debugObjects = objects.ToArray();
    }
}
