using UnityEngine;

public class GremurinWander : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.5f;
    public float wanderRadius = 2f;
    public float minMoveTime = 3f;

    [Header("Drag Scaling")]
    public float dragScaleMultiplier = 1.2f;
    public float dragScaleSpeed = 6f;

    [Header("Pause Settings")]
    public float minPauseTime = 0.8f;
    public float maxPauseTime = 2.0f;

    [Header("Screen Padding")]
    public float padding = 0.3f;

    // public Sprite defaultIdleSprite; 

    private Vector3 targetPosition;
    private Vector3 baseScale;
    private Vector3 currentScale;
    private Vector3 targetScale;

    private float pauseTimer = 0f;
    private float currentPauseDuration = 1.5f;

    private float moveTimer = 0f; 

    private bool isPaused = false;
    private bool isBeingDragged = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private int originalSortingOrder;
    public int dragSortingOrder = 100;

    public int level = 1;
    public bool IsBeingDragged => isBeingDragged;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalSortingOrder = spriteRenderer.sortingOrder;

        baseScale = transform.localScale;
        currentScale = baseScale;
        targetScale = baseScale;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("Main Camera not found! Please tag your main camera as 'MainCamera'.");
            return;
        }

        float zDistance = transform.position.z - cam.transform.position.z;
        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, zDistance));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, zDistance));
        minBounds = new Vector2(bottomLeft.x + padding, bottomLeft.y + padding);
        maxBounds = new Vector2(topRight.x - padding, topRight.y - padding);

        StartPaused(); 
    }

    void Update()
    {
        if (isBeingDragged)
        {
            SetAnimationToIdle(); 
            targetScale = baseScale * dragScaleMultiplier;
            currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * dragScaleSpeed);
            transform.localScale = currentScale;
            return; 
        }

        
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= currentPauseDuration)
            {
                isPaused = false; 
                PickNewTarget();  
            }

            SetAnimationToIdle(); 
            targetScale = baseScale; 
            currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * dragScaleSpeed);
            transform.localScale = currentScale;
            return; 
        }

        
        Vector3 dir = (targetPosition - transform.position);
        float distanceToTarget = dir.magnitude;

        
        if (distanceToTarget <= 0.05f)
        {
            isPaused = true;
            pauseTimer = 0f; 
            currentPauseDuration = Random.Range(minPauseTime, maxPauseTime); 
            moveTimer = 0f; 

            ResetToIdle(); 
            SetAnimationToIdle(); 

            targetScale = baseScale; 
        }
        else 
        {
            
            if (animator != null && !animator.GetBool("IsWalking"))
            {
                animator.SetBool("IsWalking", true);
            }

         
            Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;

            if (dir.x > 0.01f)
                spriteRenderer.flipX = true;
            else if (dir.x < -0.01f)
                spriteRenderer.flipX = false;

            targetScale = baseScale; 
        }

        currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * dragScaleSpeed);
        transform.localScale = currentScale;
    }

  
    private void SetAnimationToIdle()
    {
        if (animator != null && animator.GetBool("IsWalking"))
        {
            animator.SetBool("IsWalking", false);
        }
    }


    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        Vector3 potentialTarget = transform.position + (Vector3)randomOffset;

        potentialTarget.x = Mathf.Clamp(potentialTarget.x, minBounds.x, maxBounds.x);
        potentialTarget.y = Mathf.Clamp(potentialTarget.y, minBounds.y, maxBounds.y);

        targetPosition = potentialTarget;
        moveTimer = 0f;
     
    }

    
    void ResetToIdle()
    {
        transform.rotation = Quaternion.identity;
    }

    
    public void StartPaused()
    {
        isPaused = true;
        pauseTimer = 0f;
        currentPauseDuration = Random.Range(minPauseTime, maxPauseTime);
        SetAnimationToIdle(); 
    }


    public void SetDragging(bool dragging)
    {
        isBeingDragged = dragging;

        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = dragging ? dragSortingOrder : originalSortingOrder;

        if (!dragging)
        {
            StartPaused(); 
        }
    }
}