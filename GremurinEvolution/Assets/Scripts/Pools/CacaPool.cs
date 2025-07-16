using System.Collections.Generic;
using UnityEngine;

public class CacaPool : MonoBehaviour
{
    public static CacaPool Instance;

    [System.Serializable]
    public class PooledCaca
    {
        public int level;
        public GameObject prefab;
        public int initialSize = 5;
    }

    public List<PooledCaca> cacaPrefabs;

    private Dictionary<int, Queue<GameObject>> pools = new();
    private Dictionary<int, GameObject> prefabLookup = new();

    void Awake()
    {
        Instance = this;

        foreach (var pg in cacaPrefabs)
        {
            prefabLookup[pg.level] = pg.prefab;
            pools[pg.level] = new Queue<GameObject>();

            for (int i = 0; i < pg.initialSize; i++)
            {
                GameObject obj = Instantiate(pg.prefab);
                obj.SetActive(false);
                pools[pg.level].Enqueue(obj);
            }
        }
    }

    public GameObject GetFromPool(int level)
    {
        if (!pools.ContainsKey(level))
        {
            Debug.LogWarning($"No pool for level {level}");
            return null;
        }

        if (pools[level].Count == 0)
        {
            Debug.LogWarning($"Llegaste al limite de cacas. Creando otro igualmente. xd");
            GameObject obj = Instantiate(prefabLookup[level]);
            return obj;
        }

        GameObject pooled = pools[level].Dequeue();
        pooled.SetActive(true);
        return pooled;
    }

    public void ReturnToPool(GameObject caca, int level)
    {
        caca.SetActive(false);
        pools[level].Enqueue(caca);
    }

}
