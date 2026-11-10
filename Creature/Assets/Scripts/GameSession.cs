using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    [Header("Starting Creature")]
    public BaseCreatureData startingSpecies;

    [Range(1, 100)]
    public int startingLevel = 5;

    [Header("Player Team")]
    public List<CreatureInstanceData> team = new List<CreatureInstanceData>();

    public int activeCreatureIndex = 0;

    [Header("Encounter Settings")]
    public float encounterCooldown = 3f;

    private Vector3 savedPlayerPosition;
    private Quaternion savedPlayerRotation;
    private string savedOverworldScene;

    private bool transitioning;
    private bool returningToOverworld;
    private float nextEncounterTime;

    [Header("Inventory")]
    [Min(0)]
    public int potionCount = 3;

    [Min(0)]
    public int superPotionCount = 1;

    public bool CanStartEncounter => !transitioning && !returningToOverworld && Time.time >= nextEncounterTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create a starting team if none exists
        if (team.Count == 0 && startingSpecies != null)
        {
            team.Add(new CreatureInstanceData
                (
                startingSpecies,
                startingLevel
                ));
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public CreatureInstanceData GetActiveCreature()
    {
        if (activeCreatureIndex < 0 ||
            activeCreatureIndex >= team.Count)
            return null;

           return team[activeCreatureIndex];
    }

    public bool LoadActiveCreature(ActiveCreature target)
    {
        CreatureInstanceData data = GetActiveCreature();

        if (data == null || data.species == null || target == null)
            return false;

        data.LoadInto(target);
        return target.IsInitialized;
    }

    public void SaveActiveCreature(ActiveCreature source)
    {
        CreatureInstanceData data = GetActiveCreature();

        if (data != null)
        {
            data.SaveFrom(source);
        }
    }

    public void StartEncounter(string battleScene, Transform player)
    {

        if (!CanStartEncounter || player == null ||
            string.IsNullOrEmpty(battleScene))
            return;

        if (GetActiveCreature() == null)
        {
            Debug.LogWarning("Cannot start encounter with no active creature.");
            return;
        }

        savedPlayerPosition = player.position;
        savedPlayerRotation = player.rotation;

        savedOverworldScene = SceneManager.GetActiveScene().name;

        transitioning = true;

        Debug.Log("Entering battle scene: " + battleScene);

        SceneManager.LoadScene(battleScene);


    }

    public void ReturnToOverworld()
    {
        if (string.IsNullOrEmpty(savedOverworldScene) ||
            returningToOverworld)
            return;

        returningToOverworld = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(savedOverworldScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (returningToOverworld &&
            scene.name == savedOverworldScene)
        {
            StartCoroutine(RestorePlayer());
        }
    }

    private IEnumerator RestorePlayer()
    {
        // Wait until the overworld objects have initialized.
        yield return null;

        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Rigidbody playerRB =
                player.GetComponent<Rigidbody>();

            if (playerRB != null)
            {
                playerRB.position = savedPlayerPosition;
                playerRB.rotation = savedPlayerRotation;
            }
            else
            {
                player.transform.SetPositionAndRotation(
                    savedPlayerPosition,
                    savedPlayerRotation
                );
            }

            Physics.SyncTransforms();

            Debug.Log("Player position restored!");
        }
        else
        {
            Debug.LogWarning(
                "No GameObject tagged Player was found!"
            );
        }

        nextEncounterTime = Time.time + encounterCooldown;

        returningToOverworld = false;
        transitioning = false;
    }
}







