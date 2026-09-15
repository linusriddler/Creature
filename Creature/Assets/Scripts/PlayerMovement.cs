using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;
    public Transform cameraTransform;

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore camera's up/down rotation
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
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }

}
