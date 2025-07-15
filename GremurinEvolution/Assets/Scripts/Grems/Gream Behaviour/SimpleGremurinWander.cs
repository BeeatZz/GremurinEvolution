using UnityEngine;

public class SimpleGremurinWander : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float wanderRadius = 2f;
    public float pauseDurationMin = 0.8f;
    public float pauseDurationMax = 2.0f;

    public Sprite defaultIdleSprite;  // Assign your default sprite here in inspector

    private Vector3 targetPosition;
    private float pauseTimer = 0f;
    private float currentPauseDuration = 1f;

    private bool isPaused = true;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Store default sprite if not assigned
        if (defaultIdleSprite == null)
            defaultIdleSprite = spriteRenderer.sprite;

        PickNewTarget();
        StartPaused();
    }

    void Update()
    {
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= currentPauseDuration)
            {
                isPaused = false;
                PickNewTarget();
                EnableAnimator();
            }
            else
            {
                DisableAnimator();
            }
            return;
        }

        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;

        if (distance > 0.05f)
        {
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;

            if (animator != null && !animator.enabled)
            {
                EnableAnimator();
            }

            if (animator != null)
                animator.SetBool("IsWalking", true);
        }
        else
        {
            StartPaused();
            if (animator != null)
                animator.SetBool("IsWalking", false);
        }
    }

    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        targetPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);
    }

    void StartPaused()
    {
        isPaused = true;
        pauseTimer = 0f;
        currentPauseDuration = Random.Range(pauseDurationMin, pauseDurationMax);
        DisableAnimator();
    }

    void DisableAnimator()
    {
        if (animator != null && animator.enabled)
        {
            animator.enabled = false;
            // Set sprite to default idle
            if (spriteRenderer != null && defaultIdleSprite != null)
                spriteRenderer.sprite = defaultIdleSprite;
        }
    }

    void EnableAnimator()
    {
        if (animator != null && !animator.enabled)
        {
            animator.enabled = true;
            animator.Play("Walk", 0, 0f);
        }
    }
}
