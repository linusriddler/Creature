using Unity.VisualScripting;
using UnityEngine;

public class DropperPlayer : MonoBehaviour
{
    public float moveSpeed = 5;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
    }
    void Update()
    {
        if (rb.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            rb.constraints &= ~RigidbodyConstraints.FreezeRotation;
        }
    }
}
