using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    public bool CanStartEncounter => !transitioning && !returningToOverworld && Time.time >= nextEncounterTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
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
