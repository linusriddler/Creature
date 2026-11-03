using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CreatureInstanceData
{
    public BaseCreatureData species;

    [Range(1, 100)]
    public int level = 1;

    public int experience;
    public int currentHP;

    public List<MoveData> knownMoves = new List<MoveData>();

    // Create a new owned creature.
    public CreatureInstanceData(BaseCreatureData species, int level)
    {
        this.species = species;
        this.level = Math.Clamp(level, 1, 100);

        experience = 0;

        currentHP = CombatBalance.CalculateMaxHP(species.baseHealth, this.level);
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
