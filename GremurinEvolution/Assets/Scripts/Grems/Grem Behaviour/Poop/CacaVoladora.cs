using UnityEngine;

public class CacaVoladora : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;
    public float arcHeight = 2f;
    public float duration = 1f;
    public int level = 1;
    public GameObject floatingTextPrefab;
    private Vector3 lastPositionAtEnd;
    public float poopValue = 1f;
    private float elapsedTime = 0f;
    private bool fading = false;

    private SpriteRenderer sr;
    private float fadeDuration = 1f;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
        }
    }

    void OnEnable()
    {
        elapsedTime = 0f;
        fading = false;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }

    void Update()
    {
        if (!fading)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            Vector3 currentPos = Vector3.Lerp(start, end, t);
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            currentPos.y += height;
            transform.position = currentPos;

            if (t >= 1f)
            {
                lastPositionAtEnd = currentPos;  
                StartCoroutine(FadeAndReturn());
            }
        }
    }

    System.Collections.IEnumerator FadeAndReturn()
{
    fading = true;

    yield return new WaitForSeconds(1f);

    float fadeTime = 0f;
    if (floatingTextPrefab != null)
    {
        Vector3 newPosition = lastPositionAtEnd + new Vector3(0.8f, 0, 0);
        GameObject popup = Instantiate(floatingTextPrefab, newPosition, Quaternion.identity);
        popup.GetComponent<FloatingText>().SetText("+" + poopValue.ToString("F0"));
    }
    while (fadeTime < fadeDuration)
    {
        fadeTime += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, fadeTime / fadeDuration);

        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        yield return null;
    }

    

    CacaPool.Instance.ReturnToPool(gameObject, level);
}

}
