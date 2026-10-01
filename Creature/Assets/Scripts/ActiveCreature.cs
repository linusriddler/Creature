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
    public int defense;

    [Header("Current Moves")]
    // The specific moves this exact creature can use right now
    public List<MoveData> activeMoves = new List<MoveData>();

    // This function acts like a set up process
    public void Initialize(BaseCreatureData data, int level)
    {
        baseData = data;
        currentLevel = level;

        //RPG MATH: Scale Stats based on level
        maxHP = baseData.baseHealth + (level * 5);
        attack = baseData.baseAttack + (level * 2);
        speed = baseData.baseSpeed + (level * 2);
        defense = baseData.baseDefense + (level * 2);

        // Start combat at full health
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
