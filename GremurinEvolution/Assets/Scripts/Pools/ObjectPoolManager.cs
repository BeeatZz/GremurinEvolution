using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private static Dictionary<string, PooledObjectInfo> objectPools = new Dictionary<string, PooledObjectInfo>();
    private GameObject objectPoolEmptyHolder;

    private static GameObject particleSystemEmpty;
    private static GameObject gameObjectsEmpty;

    public enum PoolType
    {
        ParticleSystem,
        GameObject,
        None
    }
    public static PoolType PoolingType;

    private void Awake()
    {
        SetUpEmpties();
    }

    private void SetUpEmpties()
    {
        Debug.Log("Creating Pool");
        objectPoolEmptyHolder = new GameObject("Pooled Objects");

        particleSystemEmpty = new GameObject("Particle Effects");
        particleSystemEmpty.transform.SetParent(objectPoolEmptyHolder.transform);

        gameObjectsEmpty = new GameObject("GameObjects");
        gameObjectsEmpty.transform.SetParent(objectPoolEmptyHolder.transform);
    }

    public static void PreloadObjects(GameObject objectToPreload, int count, PoolType poolType = PoolType.None)
    {
        if (!objectPools.TryGetValue(objectToPreload.name, out PooledObjectInfo pool))
        {
            pool = new PooledObjectInfo() { LookupString = objectToPreload.name };
            objectPools.Add(objectToPreload.name, pool);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject newObject = Instantiate(objectToPreload);
            newObject.SetActive(false);
            GameObject parentObject = SetParentObject(poolType);
            if (parentObject != null)
            {
                newObject.transform.SetParent(parentObject.transform);
            }
            pool.InactiveObjects.Add(newObject);
        }
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType poolType = PoolType.None)
    {
        if (!objectPools.TryGetValue(objectToSpawn.name, out PooledObjectInfo pool))
        {
            pool = new PooledObjectInfo() { LookupString = objectToSpawn.name };
            objectPools.Add(objectToSpawn.name, pool);
        }

        GameObject spawnableObj = null;
        if (pool.InactiveObjects.Count > 0)
        {
            spawnableObj = pool.InactiveObjects[0];
            pool.InactiveObjects.RemoveAt(0);
        }
        else
        {
            GameObject parentObject = SetParentObject(poolType);
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            if (parentObject != null)
            {
                spawnableObj.transform.SetParent(parentObject.transform);
            }
        }

        spawnableObj.transform.position = spawnPosition;
        spawnableObj.transform.rotation = spawnRotation;
        spawnableObj.SetActive(true);

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj, int characters = -1)
    {
        string goName = characters >= 0 ? obj.name.Substring(0, characters) : obj.name.Substring(0, obj.name.Length - 7);
        if (!objectPools.TryGetValue(goName, out PooledObjectInfo pool))
        {
            Debug.LogWarning("Trying to release an object that is not pooled " + obj.name);
            return;
        }

        obj.SetActive(false);
        pool.InactiveObjects.Add(obj);
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        return poolType switch
        {
            PoolType.ParticleSystem => particleSystemEmpty,
            PoolType.GameObject => gameObjectsEmpty,
            _ => null,
        };
    }

    private void OnDestroy()
    {
        objectPools.Clear();
    }
}

public class PooledObjectInfo
{
    public string LookupString;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
