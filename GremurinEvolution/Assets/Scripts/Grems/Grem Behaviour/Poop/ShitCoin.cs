using UnityEngine;
using DG.Tweening;

public class ShitCoin : MonoBehaviour
{
    [Header("Poop Data & Motion")]
    [SerializeField] private ListaDeCacas poopData;
    [SerializeField] private GameObject poopPrefab;
    [SerializeField] private Transform poopSpawnPoint;
    [SerializeField] private Transform poopSpawnPointFlipped;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float launchDistance = 2f;
    [SerializeField] private int specialPoopRequirement;
    [SerializeField] private int poopLevel;
    [SerializeField] private int specialPoopLevel;
    [SerializeField] private int poopCounter = 0;

    private float poopTimer = 0f;
    private float elapsedTime = 0f;

    private void Update()
    {
        if (poopData == null)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float interval = poopData.listaDeCacas[poopLevel].poopRateCurve.Evaluate(elapsedTime);

        if (poopTimer >= interval)
        {
            if (poopCounter < specialPoopRequirement)
            {
                LaunchPoop(poopLevel);
            }
            else
            {
                LaunchPoop(specialPoopLevel);
            }
            poopTimer = 0f;
        }
    }

    public void LaunchPoop(int level)
    {
        if (poopData == null) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Transform spawnPointToUse = poopSpawnPoint;

        if (sr != null && sr.flipX && poopSpawnPointFlipped != null)
        {
            spawnPointToUse = poopSpawnPointFlipped;
        }

        GameObject poop = ObjectPoolManager.SpawnObject(poopPrefab, spawnPointToUse.position, Quaternion.identity);

        CacaVoladora cacaEnMov = poop.GetComponent<CacaVoladora>();

        if (!cacaEnMov)
        {
            Debug.LogError("Prefab no contiene CacaVoladora");
            return;
        }

        Vector3 direction = spawnPointToUse.right;

        if (spawnPointToUse == poopSpawnPointFlipped)
        {
            direction = -direction;
        }

        cacaEnMov.start = spawnPointToUse.position;
        cacaEnMov.end = spawnPointToUse.position + direction * launchDistance;
        cacaEnMov.arcHeight = arcHeight;
        cacaEnMov.duration = duration;
        cacaEnMov.level = level;
        cacaEnMov.poopValue = poopData.listaDeCacas[level -1].poopValue;

        if (poopCounter < specialPoopRequirement)
        {
            poopCounter++;
        }
        else
        {
            poopCounter = 0;
        }
    }
}
