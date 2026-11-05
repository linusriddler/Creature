using UnityEngine;

public class MiniCubeMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 moveDirection = Vector3.zero;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(horizontal) >= Mathf.Abs(vertical))
        {
            vertical = 0f;
        }
        else
        {
            horizontal = 0f;
        }
        moveDirection = new Vector3(horizontal, 0f, vertical);

        if (moveDirection != Vector3.zero)
        {
            transform.forward = moveDirection;
        }
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
