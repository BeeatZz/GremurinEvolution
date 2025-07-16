using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 30f;
    public float fadeDuration = 1f;

    private TMP_Text textMesh;
    private Color originalColor;
    private float elapsed = 0f;

    void Awake()
    {
        textMesh = GetComponentInChildren<TMP_Text>();
        if (textMesh != null)
        {
            originalColor = textMesh.color;
        }
        else
        {
            Debug.LogError("FloatingText: TMP_Text not found in children!");
        }
    }

    public void SetText(string text)
    {
        if (textMesh == null) return;

        textMesh.text = text;
        elapsed = 0f;
        textMesh.color = originalColor;
    }

    void Update()
    {
        if (textMesh == null) return;

        // Move upward in local space
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // Fade out
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (elapsed >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
