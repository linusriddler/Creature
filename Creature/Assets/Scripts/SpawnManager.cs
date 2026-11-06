
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    [Header("Encounter Pool")]
    public List<BaseCreatureData> encounterPool =
        new List<BaseCreatureData>();

    [Header("Spawn Point")]
    public Transform enemySpawnLocation;

    public ActiveCreature GenerateRandomEncounter(int minLevel, int maxLevel)
    {
        if (enemySpawnLocation == null)
        {
            Debug.LogError("Enemy spawn location is missing!");
            return null;
        }

        List<BaseCreatureData> validPool =
            encounterPool.FindAll(creature => creature != null);

        if (validPool.Count == 0)
        {
            Debug.LogWarning("Encounter Pool is empty!");
            return null;
        }

        BaseCreatureData chosenTemplate =
            validPool[Random.Range(0, validPool.Count)];

        int minimum = Mathf.Clamp(minLevel, 1, 100);
        int maximum = Mathf.Clamp(maxLevel, minimum, 100);

        int randomLevel = Random.Range(
            minimum,
            maximum + 1
        );

        GameObject enemyObject;

        Vector3 position = enemySpawnLocation.position;

        Quaternion rotation =
            enemySpawnLocation.rotation *
            Quaternion.Euler(chosenTemplate.spawnRotation);

        if (chosenTemplate.creaturePrefab != null)
        {
            enemyObject = Instantiate(
                chosenTemplate.creaturePrefab,
                position,
                rotation
            );
        }
        else
        {
            enemyObject = new GameObject();
            enemyObject.transform.SetPositionAndRotation(
                position,
                rotation
            );

            Debug.LogWarning(
                chosenTemplate.speciesName +
                " has no creature prefab!"
            );
        }

        enemyObject.name =
            $"Wild_{chosenTemplate.speciesName}";

        // Avoid adding a duplicate ActiveCreature component.
        ActiveCreature creature =
            enemyObject.GetComponent<ActiveCreature>();

        if (creature == null)
        {
            creature = enemyObject.AddComponent<ActiveCreature>();
        }

        creature.Initialize(chosenTemplate, randomLevel);

        Debug.Log(
            $"Wild Level {randomLevel} " +
            $"{chosenTemplate.speciesName} appeared!"
        );

        return creature;
    }
}
