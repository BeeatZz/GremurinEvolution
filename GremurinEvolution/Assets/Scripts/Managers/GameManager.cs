using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject cajaTest;
    private Caja caja;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cajaTest = CajaPool.Instance.GetFromPool(1);
        caja = cajaTest.GetComponent<Caja>();

        caja.Drop(new Vector3(5.0f, 2.0f, 0.0f));
            

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
