using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CreatureSpawn : MonoBehaviour
{
    public bool MonsterSpawnable;
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
            if (MonsterSpawnable == true)
            {
                SceneManager.LoadScene("Battle");
                MonsterSpawnable = false;
            }
            else { }
            }
            if (collision.gameObject.CompareTag("CreatureRespawn"))
            {
                MonsterSpawnable = true;
            }
    }
}
