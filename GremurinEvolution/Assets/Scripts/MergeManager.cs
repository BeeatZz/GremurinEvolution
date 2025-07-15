using System.Collections.Generic;
using UnityEngine;

public class GremMergeManager : MonoBehaviour
{
    public static GremMergeManager Instance;

    void Awake() => Instance = this;

    public void CheckForMergeAtPosition(Vector3 position)
    {
        float radius = 0.4f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius);

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

    void TryMerge(GremurinWander a, GremurinWander b)
    {
        if (a.level != b.level) return; // Only merge same level

        int nextLevel = a.level + 1;
        Vector3 mergePosition = (a.transform.position + b.transform.position) / 2;

        GremPool.Instance.ReturnToPool(a.gameObject, a.level);
        GremPool.Instance.ReturnToPool(b.gameObject, b.level);

        GameObject merged = GremPool.Instance.GetFromPool(nextLevel);
        merged.transform.position = mergePosition;

        // Set the level on the new instance
        GremurinWander mergedWander = merged.GetComponent<GremurinWander>();
        mergedWander.level = nextLevel;

        // Optional: VFX, sound, etc.
    }
}
