using UnityEngine;

public class ShitCoin : MonoBehaviour
{
    [Header("Poop Data & Motion")]
    public CacaData poopData;
    public Transform poopSpawnPoint;
    public Transform poopSpawnPointFlipped;
    public float arcHeight = 2f;
    public float duration = 1f;
    public float launchDistance = 2f;  

    private float poopTimer = 0f;
    private float elapsedTime = 0f;

    void Update()
    {
        if (poopData == null)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        poopTimer += Time.deltaTime;

        float interval = poopData.poopRateCurve.Evaluate(elapsedTime);

        if (poopTimer >= interval)
        {
            poopTimer = 0f;
            LaunchPoop();
        }
    }

    public void LaunchPoop()
    {
        if (poopData == null) return;

        GameObject poop = CacaPool.Instance.GetFromPool(poopData.poopLevel);
        if (poop == null) return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        Transform spawnPointToUse = poopSpawnPoint;

        if (sr != null && sr.flipX && poopSpawnPointFlipped != null)
        {
            spawnPointToUse = poopSpawnPointFlipped;
        }

        poop.transform.position = spawnPointToUse.position;
        poop.transform.rotation = Quaternion.identity;

        CacaVoladora mover = poop.GetComponent<CacaVoladora>();
        if (mover == null) mover = poop.AddComponent<CacaVoladora>();

        Vector3 direction = spawnPointToUse.right;

        if (spawnPointToUse == poopSpawnPointFlipped)
        {
            direction = -direction;
        }


        mover.start = spawnPointToUse.position;
        mover.end = spawnPointToUse.position + direction * launchDistance;
        mover.arcHeight = arcHeight;
        mover.duration = duration;
        mover.level = poopData.poopLevel;
        mover.poopValue = poopData.poopValue;
    }
}
