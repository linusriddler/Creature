using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;
    public Transform cameraTransform;
    public float jumpStrength = 5f;
    public Rigidbody rb;
    public bool isGrounded = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            right * moveHorizontal +
            forward * moveVertical;

        transform.position += moveDirection * PlayerMoveSpeed * Time.deltaTime;
        if (moveDirection != Vector3.zero)
        {
           Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, 500f *  Time.deltaTime);
        }
        

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
            {
                rb.AddForce (Vector3.up * jumpStrength, ForceMode.Impulse);
                isGrounded = false;
            }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
