using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;
    public Transform cameraTransform;

    public float jumpStrength = 5f;
    public Rigidbody rb;

    public bool isGrounded = false;

    public Animator anim;


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

            Walking();
            Jumping();


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
        //Get WASD Input 
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        //Get the camera's forward and right directions
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        //Ignore Camera's Y Rotation
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Create movement direction based on camera orientation and input
        Vector3 moveDirection =
            right * moveHorizontal +
            forward * moveVertical;

        //Move the player
        transform.position += moveDirection * PlayerMoveSpeed * Time.deltaTime;

        //Check if the player is walking
        bool isWalking = moveDirection.magnitude > 0.1f;

        //Tell the animator if the player is walking or not
        anim.SetBool("isWalking", isWalking);

        //Rotate the player to face the direction of movement
        if (isWalking)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 500f * Time.deltaTime);
        }
    }

    private void Jumping()
    {
               if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            isGrounded = false;
            anim.SetBool("isGrounded", false);
        }
    }
}
