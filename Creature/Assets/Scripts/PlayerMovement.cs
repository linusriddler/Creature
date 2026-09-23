using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement: MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;
    public Transform cameraTransform;
    public float jumpStrength = 5f;
    public Rigidbody rb;
    public bool isGrounded = false;
    public Animator anim;
    private bool IsWalking;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Battle")
        {
        }
        else
        {
            IsWalkingCheck();
            Walking();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
            {
                rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
                isGrounded = false;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("isGrounded", false);
        }
    }
    private void Walking()
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
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 500f * Time.deltaTime);
            anim.SetBool("isWalking", true);
        }
   
    }
    private void IsWalkingCheck()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            IsWalking = true;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            IsWalking = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            IsWalking = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            IsWalking = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            IsWalking = false;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            IsWalking = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            IsWalking = false;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            IsWalking = false;
        }
        if (IsWalking == true)
        {
            anim.SetBool("isWalking", true);
        }
        if (IsWalking == false)
        {
            anim.SetBool("isWalking", false);
        }
    }
}
