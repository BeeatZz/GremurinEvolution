using UnityEngine;

public class ShitCoin : MonoBehaviour
{
    public float minInterval = 1f;
    public float maxInterval = 5f;

    void Start()
    {
        StartCoroutine(Poop());
    }

    System.Collections.IEnumerator Poop()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            Debug.Log("ShitCoin does something at: " + Time.time);
        }
    }
}
