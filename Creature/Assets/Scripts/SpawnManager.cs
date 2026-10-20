using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Encounter Pool'")]
    // List of creatures that can be encountered 
    public List<BaseCreatureData> encounterPool = new List<BaseCreatureData>();

    [Header("Spawn Points")]
    public Transform enemySpawnLocation;

    public ActiveCreature GenerateRandomEncounter(int minLevel, int maxLevel)
    {
        // Safety Check
        if (encounterPool.Count == 0)
        {
            Debug.LogWarning("Encounter Pool is empty! Cannot generate encounter.");
            return null;
        }

        // Picks a random creature template from the encounter pool
        int randomIndex = Random.Range(0, encounterPool.Count);
        BaseCreatureData chosenTemplate = encounterPool[randomIndex];

        // Create a new blank GameObject named after the species when running
        GameObject newEnemyObject = new GameObject($"Wild_{chosenTemplate.speciesName}");

        //Spawn it at the designated battle spawn location
        if (enemySpawnLocation != null)
        {
            newEnemyObject.transform.position = enemySpawnLocation.position;
        }
        else
        {
            Debug.LogWarning("Enemy spawn location is not set!");
        }

        // Add the ActiveCreature component to the new GameObject
        ActiveCreature enemyComponent = newEnemyObject.AddComponent<ActiveCreature>();

        //Randomize the level and stat of the creature based on the chosen template
        int randomLevel = Random.Range(minLevel, maxLevel + 1);
        enemyComponent.Initialize(chosenTemplate, randomLevel);

        Debug.Log($"[SPAWNER] Successfully generated a Level {randomLevel} {chosenTemplate.speciesName}!");
        return enemyComponent;
    }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
