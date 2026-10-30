using System.Collections.Generic;
using UnityEngine;

public class ActiveCreature : MonoBehaviour
{
    [Header("Data Template")]
    // Drop your ScriptableObject here
    public BaseCreatureData baseData;

    [Header("Runtime Stats")]
    public int currentLevel = 1;
    public int maxHP;
    public int currentHp;
    public int attack;
    public int speed;
    public float defense;

    [Header("Current Moves")]
    // The specific moves this exact creature can use right now
    public List<MoveData> activeMoves = new List<MoveData>();

    // This function acts like a set up process
    public void Initialize(BaseCreatureData data, int level)
    {
        baseData = data;
        currentLevel = level;

        // HP scales slightly faster so battles don't become instant-kills late game
        // A base health of 10 results in ~31 HP at Level 1, and ~120 HP at Level 10
        maxHP = Mathf.RoundToInt(baseData.baseHealth * (1f + (level * 0.2f))) + (level * 6);

        // Multiplicative scaling: Every level adds 10% of the creature's native BASE stat
        // If Base Attack is 14, it gains 1.4 per level. If Base Attack is 6, it gains 0.6.
        attack = Mathf.RoundToInt(baseData.baseAttack * (1f + (level * 0.1f)));
        speed = Mathf.RoundToInt(baseData.baseSpeed * (1f + (level * 0.1f)));
        defense = Mathf.RoundToInt(baseData.baseDefense * (1f + (level * 0.1f)));

        currentHp = maxHP;

        //Clear any existing moves and copy up to 4 moves from the template data
        activeMoves.Clear();
        for (int i =0; i < Mathf.Min(4, baseData.learnableMoves.Count); i++)
        {
            activeMoves.Add(baseData.learnableMoves[i]);
        }

    }

    // A helper function to let this creature take damage safely
    public void TakeDamage(int damageAmount)
    {
        currentHp -= damageAmount;

        // Don't let the HP go below 0
        if (currentHp < 0)
        {
            currentHp = 0;
        }

        Debug.Log($"{baseData.speciesName} took {damageAmount} damage! HP is now {currentHp}/{maxHP}");


    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize(baseData, currentLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
