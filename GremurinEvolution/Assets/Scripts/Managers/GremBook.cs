using UnityEngine;
using System.Collections.Generic;
using static ElementsEnum; 

[CreateAssetMenu(menuName = "GremBook")]
public class GremBook : ScriptableObject
{
    [System.Serializable]
    public struct GremEntry
    {
        public ElementType type;
        public GameObject prefab;
        public int preloadCount;
    }

    public List<GremEntry> grems = new List<GremEntry>();

    private Dictionary<ElementType, GameObject> gremLookup;

    public void InitPools()
    {
        gremLookup = new Dictionary<ElementType, GameObject>();

        foreach (var grem in grems)
        {
            gremLookup[grem.type] = grem.prefab;
            ObjectPoolManager.PreloadObjects(
                grem.prefab,
                grem.preloadCount,
                ObjectPoolManager.PoolType.GameObject
            );
        }
    }

    public GameObject GetPrefab(ElementType type)
    {
        if (gremLookup.TryGetValue(type, out var prefab))
            return prefab;

        Debug.LogError($"No Grem prefab found for type {type}");
        return null;
    }
}
