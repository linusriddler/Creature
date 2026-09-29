using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnCreature : MonoBehaviour
{
    CreatureSpawn MonsterSpawnable;
    [SerializeField] private CreatureSpawn creatureSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision)
    {

    }
}
