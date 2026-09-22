using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Camera : MonoBehaviour
{
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;
    private float mouseX;
    private float mouseY;
    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate horizontally around target's up axis
        transform.RotateAround(target.position, Vector3.up, mouseX);

        // Rotate vertically around camera's right axis
        transform.RotateAround(target.position, transform.right, -mouseY);
        // Check whether or not to lock the mouse
        if (SceneManager.GetActiveScene().name == "Battle")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
