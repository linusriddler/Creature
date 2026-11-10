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

        if (species.learnableMoves != null)
        {
            foreach (MoveData move in species.learnableMoves)
            {
                if (move == null) continue;
                if (knownMoves.Count >= 4) break;

                knownMoves.Add(move);
            }
        }
    }

    // Load saved data into an ActiveCreature in battle.
    public void LoadInto(ActiveCreature creature)
    {
        if (species == null || creature == null)
            return;

        creature.Initialize(species, level);

        creature.experience = Mathf.Max(0, experience);

        creature.currentHp = Mathf.Clamp(currentHP, 0, creature.maxHP);
    }

    // Save updated battle stats back into the team.
    public void SaveFrom(ActiveCreature creature)
    {
        if (creature == null || !creature.IsInitialized)
            return;

        species = creature.baseData;
        level = creature.currentLevel;
        experience = creature.experience;
        currentHP = creature.currentHp;

        knownMoves = new List<MoveData>(creature.activeMoves);
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
