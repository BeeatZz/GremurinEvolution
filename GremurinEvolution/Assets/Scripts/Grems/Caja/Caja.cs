using UnityEngine;

public class Caja : MonoBehaviour
{
    private bool isFalling = false;
    private Vector3 targetPosition;
    private int level = 1;
    


    void Update()
    {
        if (isFalling)
        {
            // Smoothly move towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
            if (transform.position == targetPosition)
            {
                CajaPool.Instance.ReturnToPool(gameObject, level);
            }
        }
    }

    public void Drop(Vector3 targetPos)
    {
        transform.position = new Vector3(targetPos.x, 10.0f, targetPos.z);
        isFalling = true;
        targetPosition = targetPos;
    }

    public void OpenCaja()
    {
        GameObject grem = GremPool.Instance.GetFromPool(level);
        if (grem != null)
        {
            grem.transform.position = transform.position;
            grem.SetActive(true);
        }
    }

    public void ResetCaja()
    {
        isFalling = false;
        gameObject.SetActive(false);
    }

    void OnMouseDown()
    {
        OpenCaja();
        CajaPool.Instance.ReturnToPool(gameObject, level);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = 10f; // Distance from camera
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
