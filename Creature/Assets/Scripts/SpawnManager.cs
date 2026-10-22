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

        // Create a new GameObject named after the species when running
        GameObject newEnemyObject;
        if(chosenTemplate.creaturePrefab != null)
        {
            // Instantiates the 3D/2D model directly at the spawn point anchor location
            newEnemyObject = Instantiate(chosenTemplate.creaturePrefab, enemySpawnLocation.position, enemySpawnLocation.rotation);
            newEnemyObject.name = $"Wild_{chosenTemplate.speciesName}";
        }
        else
        {
            // Backup fallback if you forgot to drag a prefab asset into the data slot
            newEnemyObject = new GameObject($"Wild_{chosenTemplate.speciesName} (Missing Prefab)");
            newEnemyObject.transform.position = enemySpawnLocation.position;
        }


        // 3. Attach runtime tracking component and initialize stats
        ActiveCreature enemyComponent = newEnemyObject.AddComponent<ActiveCreature>();
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
