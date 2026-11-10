
using System.Collections.Generic;
using UnityEngine;

public class ActiveCreature : MonoBehaviour
{
    [Header("Species Data")]
    public BaseCreatureData baseData;

    [Header("Progression")]
    [Range(1, 100)]
    public int currentLevel = 1;

    [Min(0)]
    public int experience = 0;

    [Header("Runtime Stats")]
    public int maxHP;
    public int currentHp;
    public int attack;
    public float defense;
    public int speed;

    [Header("Current Moves")]
    public List<MoveData> activeMoves = new List<MoveData>();

    public bool IsInitialized { get; private set; }

    public bool IsFainted => IsInitialized && currentHp <= 0;

    public void Initialize(BaseCreatureData data, int level)
    {
        if (data == null)
        {
            Debug.LogError("Cannot initialize a creature without species data.");
            return;
        }

        baseData = data;
        currentLevel = Mathf.Clamp(level, 1, CombatBalance.MaxLevel);
        experience = 0;

        CalculateStats();
        currentHp = maxHP;

        // Copy the starting moves from the species template.
        activeMoves.Clear();

        if (baseData.learnableMoves != null)
        {
            foreach (MoveData move in baseData.learnableMoves)
            {
                if (move == null)
                    continue;

                if (activeMoves.Count >= 4)
                    break;

                activeMoves.Add(move);
            }
        }

        IsInitialized = true;

        Debug.Log(
            $"{baseData.speciesName} initialized at Level {currentLevel}. " +
            $"HP: {maxHP}, ATK: {attack}, " +
            $"DEF: {defense:F1}, SPD: {speed}"
        );
    }

    private void CalculateStats()
    {
        maxHP = CombatBalance.CalculateMaxHP(
            baseData.baseHealth, currentLevel
        );

        attack = CombatBalance.CalculateStat(
            baseData.baseAttack, currentLevel
        );

        defense = CombatBalance.CalculateStat(
            baseData.baseDefense, currentLevel
        );

        speed = CombatBalance.CalculateStat(
            baseData.baseSpeed, currentLevel
        );
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsInitialized || IsFainted)
            return;

        int actualDamage =
            Mathf.Min(currentHp, Mathf.Max(0, damageAmount));

        currentHp -= actualDamage;

        Debug.Log(
            $"{baseData.speciesName} took {actualDamage} damage! " +
            $"HP: {currentHp}/{maxHP}"
        );

        if (IsFainted)
        {
            Debug.Log($"{baseData.speciesName} fainted!");
        }
    }

    public void Heal(int amount)
    {
        if (!IsInitialized || IsFainted)
            return;

        currentHp = Mathf.Min(
            maxHP,
            currentHp + Mathf.Max(0, amount)
        );
    }

    public void HealToFull()
    {
        if (!IsInitialized)
            return;

        currentHp = maxHP;
    }

    public int GainExperience(int amount)
    {
        if (!IsInitialized || currentLevel >= CombatBalance.MaxLevel)
            return 0;

        experience += Mathf.Max(0, amount);

        int levelsGained = 0;

        while (currentLevel < CombatBalance.MaxLevel)
        {
            int requiredXP =
                CombatBalance.ExperienceToNextLevel(currentLevel);

            if (experience < requiredXP)
                break;

            experience -= requiredXP;

            int previousMaxHP = maxHP;

            currentLevel++;
            levelsGained++;

            CalculateStats();

            // Increase current HP by the amount max HP increased.
            // This does not fully heal the creature.
            currentHp = Mathf.Clamp(
                currentHp + (maxHP - previousMaxHP),
                0,
                maxHP
            );

            Debug.Log(
                $"{baseData.speciesName} reached Level {currentLevel}! " +
                $"HP: {maxHP}, ATK: {attack}, " +
                $"DEF: {defense:F1}, SPD: {speed}"
            );
        }

        if (currentLevel >= CombatBalance.MaxLevel)
            experience = 0;

        return levelsGained;
    }
}
