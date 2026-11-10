
using UnityEngine;

public static class CombatBalance
{
    public const int MaxLevel = 100;

    // STAT GROWTH
    public const float StatGrowthPerLevel = 0.08f;
    public const float HPBaseMultiplier = 2f;
    public const float HPGrowthPerLevel = 0.30f;

    // DAMAGE BALANCE
    public const float DamageDivisor = 20f;
    public const float DefenseRatioExponent = 0.75f;
    public const float MinimumDamageVariance = 0.85f;
    public const float CriticalMultiplier = 1.5f;

    // DODGE BALANCING
    public const float BaseDodgeChance = 0.05f;
    public const float SpeedDodgeInfluence = 0.30f;
    public const float MinDodgeChance = 0.02f;
    public const float MaxDodgeChance = 0.20f;

    public static int CalculateMaxHP(int baseHealth, int level)
    {
        level = Mathf.Clamp(level, 1, MaxLevel);

        float multiplier =
            HPBaseMultiplier + (HPGrowthPerLevel * level);

        return Mathf.Max(
            1,
            Mathf.RoundToInt(Mathf.Max(1, baseHealth) * multiplier)
        );
    }

    public static int CalculateStat(int baseStat, int level)
    {
        level = Mathf.Clamp(level, 1, MaxLevel);

        float multiplier =
            1f + StatGrowthPerLevel * (level - 1);

        return Mathf.Max(
            1,
            Mathf.RoundToInt(Mathf.Max(1, baseStat) * multiplier)
        );
    }

    public static float CalculateStat(float baseStat, int level)
    {
        level = Mathf.Clamp(level, 1, MaxLevel);

        float multiplier =
            1f + StatGrowthPerLevel * (level - 1);

        return Mathf.Max(1f, baseStat * multiplier);
    }

    public static int CalculateDamage(
        int attackerLevel,
        int attackerAttack,
        float defenderDefense,
        float movePower,
        bool criticalHit)
    {
        attackerLevel = Mathf.Clamp(attackerLevel, 1, MaxLevel);

        // The relationship between attack and defense.
        float statRatio =
            (float)Mathf.Max(1, attackerAttack)
            / Mathf.Max(1f, defenderDefense);

        // Limit extreme matchups.
        statRatio = Mathf.Clamp(statRatio, 0.2f, 5f);

        // Soften the effect of big stat differences.
        float statModifier =
            Mathf.Pow(statRatio, DefenseRatioExponent);

        float levelFactor = 0.4f * attackerLevel + 2f;

        float rawDamage =
            2f +
            (levelFactor * Mathf.Max(0f, movePower)
             * statModifier / DamageDivisor);

        float variance =
            Random.Range(MinimumDamageVariance, 1f);

        float criticalModifier =
            criticalHit ? CriticalMultiplier : 1f;

        int damage = Mathf.RoundToInt(
            rawDamage * variance * criticalModifier
        );

        return Mathf.Max(1, damage);
    }


    public static float CalculateDodgeChance(
        int attackerSpeed,
        int defenderSpeed)
    {
        attackerSpeed = Mathf.Max(1, attackerSpeed);
        defenderSpeed = Mathf.Max(1, defenderSpeed);

        // Compare the two creatures' speeds.
        // Positive = defender is faster.
        // Negative = attacker is faster.
        float speedDifference =
            (float)(defenderSpeed - attackerSpeed) /
            (defenderSpeed + attackerSpeed);

        // Calculate chance based on relative speed.
        float dodgeChance =
            BaseDodgeChance +
            (speedDifference * SpeedDodgeInfluence);

        // Prevent creatures from becoming impossible to hit.
        return Mathf.Clamp(
            dodgeChance,
            MinDodgeChance,
            MaxDodgeChance
        );
    }


    // EXP REQUIRED FOR THE NEXT LEVEL
    public static int ExperienceToNextLevel(int level)
    {
        if (level >= MaxLevel)
            return 0;

        return Mathf.RoundToInt(
            40f + 16f * level + 0.35f * level * level
        );
    }

    // EXP AWARDED FOR DEFEATING A CREATURE
    public static int ExperienceReward(int defeatedLevel)
    {
        defeatedLevel = Mathf.Clamp(defeatedLevel, 1, MaxLevel);

        return Mathf.RoundToInt(
            15f +
            4f * defeatedLevel +
            0.04f * defeatedLevel * defeatedLevel
        );
    }
}


