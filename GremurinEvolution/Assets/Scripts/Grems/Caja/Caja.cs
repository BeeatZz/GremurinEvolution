using UnityEngine;
using System.Collections;
using static ElementsEnum;

public class Caja : MonoBehaviour
{
    [Header("Fall and Bounce")]
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float bounceHeight = 0.5f;
    [SerializeField] private float bounceSpeed = 10f;
    [SerializeField] private float squashFactor = 0.7f;
    [SerializeField] private float stretchFactor = 1.2f;
    [SerializeField] private float squashDuration = 0.1f;

    [Header("Rattle Settings")]
    [SerializeField] private float minRattleDelay = 1f;
    [SerializeField] private float maxRattleDelay = 3f;
    [SerializeField] private float rattleAmount = 0.05f;
    [SerializeField] private float rattleDuration = 0.2f;

    [Header("Pool References")]
    [SerializeField] private GameObject gremPrefab; 
    [SerializeField] private GameObject cajaPrefab; 

    private bool isFalling = false;
    private bool isRattling = false;
    private Vector3 targetPosition;
    private int level = 1;
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Coroutine rattleCoroutine;

    private ElementType gremType;
    [SerializeField] private GremBook gremRegistry;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isFalling)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, fallSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                isFalling = false;
                StartCoroutine(DoBounce());
            }
        }
    }

    private IEnumerator DoBounce()
    {
        yield return StartCoroutine(Squash(new Vector3(stretchFactor, squashFactor, stretchFactor), squashDuration));

        Vector3 upPos = targetPosition + Vector3.up * bounceHeight;

        yield return StartCoroutine(Squash(new Vector3(squashFactor, stretchFactor, squashFactor), squashDuration));

        while (Vector3.Distance(transform.position, upPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, upPos, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, bounceSpeed * Time.deltaTime);
            yield return null;
        }

        yield return StartCoroutine(Squash(originalScale, squashDuration));

        transform.position = targetPosition;
        originalPosition = transform.position;

        rattleCoroutine = StartCoroutine(RattleCoroutine());
    }

    private IEnumerator Squash(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private IEnumerator RattleCoroutine()
    {
        isRattling = true;

        while (isRattling)
        {
            float delay = Random.Range(minRattleDelay, maxRattleDelay);
            yield return new WaitForSeconds(delay);

            float elapsed = 0f;

            while (elapsed < rattleDuration)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-rattleAmount, rattleAmount),
                    Random.Range(-rattleAmount, rattleAmount),
                    Random.Range(-rattleAmount, rattleAmount)
                );

                transform.position = originalPosition + offset;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPosition;
        }
    }
    public void SetDelivery(ElementType type)
    {
        gremType = type;
    }
    public void Drop(Vector3 targetPos)
    {
        transform.position = new Vector3(targetPos.x, 10.0f, targetPos.z);
        isFalling = true;
        targetPosition = targetPos;
        transform.localScale = originalScale;

        if (rattleCoroutine != null)
            StopCoroutine(rattleCoroutine);
        isRattling = false;
    }

    public void OpenCaja()
    {
        GameObject gremPrefab = gremRegistry.GetPrefab(gremType);
        if (gremPrefab != null)
        {
            ObjectPoolManager.SpawnObject(
                gremPrefab,
                transform.position,
                Quaternion.identity,
                ObjectPoolManager.PoolType.GameObject
            );
        }

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
    public void ResetCaja()
    {
        isFalling = false;
        isRattling = false;

        if (rattleCoroutine != null)
            StopCoroutine(rattleCoroutine);

        transform.localScale = originalScale;
        transform.position = originalPosition;

        gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        OpenCaja();

        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
