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

    // No longer directly setting sprite if Animator handles all states
    // public Sprite defaultIdleSprite; 

    private Vector3 targetPosition;
    private Vector3 baseScale;
    private Vector3 currentScale;
    private Vector3 targetScale;

    private float pauseTimer = 0f;
    private float currentPauseDuration = 1.5f;

    private float moveTimer = 0f; // Tracks time spent in current movement phase
    // currentMoveDuration is now implicitly set by minMoveTime when picking a new target

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

        // Calculate screen bounds with padding
        Camera cam = Camera.main;
        // Ensure the camera exists and is active, otherwise this will fail
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

        StartPaused(); // Ensure the Gremurin starts in a paused (idle) state
    }

    void Update()
    {
        // 1. Handle Dragging State (Highest Priority)
        if (isBeingDragged)
        {
            SetAnimationToIdle(); // Ensure idle animation when dragged
            targetScale = baseScale * dragScaleMultiplier;
            currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * dragScaleSpeed);
            transform.localScale = currentScale;
            return; // Exit Update early, no other logic runs
        }

        // 2. Handle Paused State
        if (isPaused)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= currentPauseDuration)
            {
                isPaused = false; // End pause
                PickNewTarget();  // Start a new movement phase
            }

            SetAnimationToIdle(); // Keep idle animation when paused
            targetScale = baseScale; // Ensure scale is base when not dragged
            currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * dragScaleSpeed);
            transform.localScale = currentScale;
            return; // Exit Update early, no other logic runs
        }

        // 3. Handle Movement State (when not dragged or paused)
        Vector3 dir = (targetPosition - transform.position);
        float distanceToTarget = dir.magnitude;

        // Check if Gremurin has reached its current target
        if (distanceToTarget <= 0.05f) // Threshold to consider "at target"
        {
            // If at target, transition to a paused (idle) state
            isPaused = true;
            pauseTimer = 0f; // Reset pause timer
            currentPauseDuration = Random.Range(minPauseTime, maxPauseTime); // Randomize next pause duration
            moveTimer = 0f; // Reset move timer for the next movement cycle

            ResetToIdle(); // Resets rotation (if any)
            SetAnimationToIdle(); // Tell Animator to transition to Idle state

            targetScale = baseScale; // Ensure scale is base when idle
        }
        else // Gremurin is still actively moving towards the target
        {
            // Ensure animator is playing the walking animation
            if (animator != null && !animator.GetBool("IsWalking"))
            {
                animator.SetBool("IsWalking", true);
            }

            // Handle actual position movement
            Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
            transform.position += move;

            // Flip sprite based on direction
            if (dir.x > 0.01f)
                spriteRenderer.flipX = true;
            else if (dir.x < -0.01f)
                spriteRenderer.flipX = false;

            targetScale = baseScale; // Maintain base scale when walking
        }

        // Apply scale changes (always happens, ensures smooth scale transitions)
        currentScale = Vector3.Lerp(currentScale, targetScale, Time.deltaTime * dragScaleSpeed);
        transform.localScale = currentScale;
    }

    /// <summary>
    /// Sets the Animator's "IsWalking" parameter to false, prompting a transition to the idle state.
    /// Does not manually disable the Animator or set the sprite.
    /// </summary>
    private void SetAnimationToIdle()
    {
        if (animator != null && animator.GetBool("IsWalking"))
        {
            animator.SetBool("IsWalking", false);
        }
    }

    /// <summary>
    /// Picks a new random target position within the wander radius and screen bounds.
    /// </summary>
    void PickNewTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
        Vector3 potentialTarget = transform.position + (Vector3)randomOffset;

        // Clamp the potential target to stay within screen bounds
        potentialTarget.x = Mathf.Clamp(potentialTarget.x, minBounds.x, maxBounds.x);
        potentialTarget.y = Mathf.Clamp(potentialTarget.y, minBounds.y, maxBounds.y);

        targetPosition = potentialTarget;
        moveTimer = 0f; // Reset move timer for the start of this new movement phase
        // The actual duration for which it will move is dictated by moveSpeed and distance to target
        // until it reaches the target or hits the minMoveTime for the *next* decision, if you re-implement that.
    }

    /// <summary>
    /// Resets the Gremurin's rotation to identity (no rotation).
    /// </summary>
    void ResetToIdle()
    {
        transform.rotation = Quaternion.identity;
    }

    /// <summary>
    /// Puts the Gremurin into a paused state, immediately setting its animation to idle.
    /// </summary>
    public void StartPaused()
    {
        isPaused = true;
        pauseTimer = 0f;
        currentPauseDuration = Random.Range(minPauseTime, maxPauseTime);
        SetAnimationToIdle(); // Ensure it starts in an idle animation state
    }

    /// <summary>
    /// Sets the dragging state of the Gremurin. Adjusts sorting order and transitions to idle when dropped.
    /// </summary>
    /// <param name="dragging">True if the Gremurin is currently being dragged, false otherwise.</param>
    public void SetDragging(bool dragging)
    {
        isBeingDragged = dragging;

        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = dragging ? dragSortingOrder : originalSortingOrder;

        if (!dragging)
        {
            StartPaused(); // When dropped, immediately go to a paused (idle) state
        }
    }
}