using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CreatureSpawn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SpawnZone"))
        {
            SceneManager.LoadScene("Battle");
        }
    }
}
