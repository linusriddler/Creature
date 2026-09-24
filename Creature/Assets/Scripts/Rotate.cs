using UnityEngine;

public class Rotate : MonoBehaviour
{
    private bool isLiving;
    public Rigidbody rotaterb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent <Rigidbody>();
        isLiving = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLiving == true)
        {
            transform.Rotate(1, 1, 1);
        }
    }
}
