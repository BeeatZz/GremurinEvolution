using System.Collections.Generic;
using UnityEngine;
using static Combinations;
using static ElementsEnum;

public class GremMergeManager : MonoBehaviour
{
    public static GremMergeManager Instance;

    [Header("Merge Settings")]
    [SerializeField] private float mergeRadius = 0.4f;
    [SerializeField] private ElementCombinationDatabase combinationDB;
    [SerializeField] private GremBook gremRegistry;

    void Awake() => Instance = this;

    public void CheckForMergeAtPosition(Vector3 position)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, mergeRadius);

        List<GremurinWander> grems = new();
        foreach (var hit in hits)
        {
            var wander = hit.GetComponent<GremurinWander>();
            if (wander != null && !wander.IsBeingDragged)
            {
                grems.Add(wander);
            }
        }

        if (grems.Count >= 2)
        {
            TryMerge(grems[0], grems[1]);
        }
    }

    private void TryMerge(GremurinWander a, GremurinWander b)
    {
        ElementType resultType = FindCombinationResult(a.ElementType, b.ElementType);
        if (resultType == default) return; 

        Vector3 mergePosition = (a.transform.position + b.transform.position) / 2;

        
        ObjectPoolManager.ReturnObjectToPool(a.gameObject);
        ObjectPoolManager.ReturnObjectToPool(b.gameObject);

        GameObject resultPrefab = gremRegistry.GetPrefab(resultType);
        if (resultPrefab != null)
        {
            ObjectPoolManager.SpawnObject(
                resultPrefab,
                mergePosition,
                Quaternion.identity,
                ObjectPoolManager.PoolType.GameObject
            );
        }
    }

    private ElementType FindCombinationResult(ElementType a, ElementType b)
    {
        foreach (var combo in combinationDB.combos)
        {
            
            bool matchDirect = (combo.elementoA == a && combo.elementoB == b);
            bool matchReverse = (combo.elementoA == b && combo.elementoB == a);

            if (matchDirect || matchReverse)
                return combo.resultado;
        }

        return default; 
    }
}
