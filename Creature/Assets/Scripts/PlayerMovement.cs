using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float PlayerMoveSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        
        Vector3 moveDirection = new Vector3(moveHorizontal, 0, 0);

        transform.position += moveDirection * PlayerMoveSpeed * Time.deltaTime;
    }
}
