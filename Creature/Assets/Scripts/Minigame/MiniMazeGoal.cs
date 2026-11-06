using UnityEngine;

public class MiniMazeGoal : MonoBehaviour
{
    public GameObject MiniMaze;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(MiniMaze);
        }
    }
}
