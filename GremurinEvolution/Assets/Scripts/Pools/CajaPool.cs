using System.Collections.Generic;
using UnityEngine;

public class CajaPool : MonoBehaviour
{
    public static CajaPool Instance;

    [System.Serializable]
    public class PooledCaja
    {
        public int level;
        public GameObject prefab;
        public int initialSize = 5;
    }

    public List<PooledCaja> cajaPrefabs;

    private Dictionary<int, Queue<GameObject>> pools = new();
    private Dictionary<int, GameObject> prefabLookup = new();

    void Awake()
    {
        Instance = this;

        foreach (var pg in cajaPrefabs)
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
            return null;
        }

        if (pools[level].Count == 0)
        {
            GameObject obj = Instantiate(prefabLookup[level]);
            return obj;
        }

        GameObject pooled = pools[level].Dequeue();
        pooled.SetActive(true);
        return pooled;
    }

    public void ReturnToPool(GameObject caja, int level)
    {
        Caja cajaAux = caja.GetComponent<Caja>();
        cajaAux.ResetCaja();
        pools[level].Enqueue(caja);
    }

}
