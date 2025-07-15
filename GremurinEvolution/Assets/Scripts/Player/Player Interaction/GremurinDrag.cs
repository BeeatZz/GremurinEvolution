using UnityEngine;

public class GremurinDrag : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = false;
    private GremurinWander wander;

    [Header("Drag Scale")]
    public float dragScaleMultiplier = 1.2f;
    public float dragScaleSpeed = 6f;

    [Header("Drag Delay")]
    public float dragSmoothTime = 0.1f; // Lower = less delay, higher = more delay

    private Vector3 originalScale;
    private Vector3 targetScale;

    private Vector3 targetPosition; // New target position for smooth dragging

    void Start()
    {
        wander = GetComponent<GremurinWander>();
        originalScale = transform.localScale;
        targetScale = originalScale;
        targetPosition = transform.position;
    }

    void Update()
    {
        // Smooth scale transition
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * dragScaleSpeed);

        if (isDragging)
        {
            // Smoothly move towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / dragSmoothTime);
        }
    }

    void OnMouseDown()
    {
        offset = transform.position - GetMouseWorldPos();
        isDragging = true;
        wander?.SetDragging(true);

        targetScale = originalScale * dragScaleMultiplier;

        // Initialize targetPosition to current position
        targetPosition = transform.position;
    }

    void OnMouseUp()
    {
        isDragging = false;
        wander?.SetDragging(false);
        GremMergeManager.Instance.CheckForMergeAtPosition(transform.position);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update the target position to mouse + offset instead of snapping directly
            targetPosition = GetMouseWorldPos() + offset;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10f; // Distance from camera
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
