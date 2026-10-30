using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


// This Enum tracks the current state of the battle
public enum BattleState { START, PLAYERTURN, ENEMYTURN, BUSY, WON, LOST }

public class BattleManager : MonoBehaviour
{
    public BattleState currentState;

    [Header("Combatants")]
    public ActiveCreature playerCreature;
    public ActiveCreature enemyCreature;

    [Header("Setup Templates")]
    public BaseCreatureData playerTemplate;

    [Header("Managers")]
    public SpawnManager spawnManager;

    [Header("Encounter Settings")]
    // These will now show up as boxes you can type numbers into in Unity!
    public int minimumEncounterLevel = 3;
    public int maximumEncounterLevel = 6;

    [Header("Modifiers")]
    public float criticalHitChance = 0.05f; // 5% chance to crit




    void Start()
    {
        currentState = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        Debug.Log("--- BATTLE START ---");

        // Initialize stats
        playerCreature.Initialize(playerTemplate, 5);
        enemyCreature = spawnManager.GenerateRandomEncounter(minimumEncounterLevel, maximumEncounterLevel);

        // TURN ORDER SPEED CHECK: Compare who is faster!
        if (playerCreature.speed >= enemyCreature.speed)
        {
            Debug.Log($"{playerCreature.baseData.speciesName} is faster!");
            PlayerTurn();
        }
        else
        {
            Debug.Log($"The wild {enemyCreature.baseData.speciesName} is faster!");
            Invoke(nameof(EnemyTurn), 1.0f); // Enemy goes first automatically
        }

    }

    void PlayerTurn()
    {
        currentState = BattleState.PLAYERTURN;
        Debug.Log($"What will {playerCreature.baseData.speciesName} do? (Press Spacebar to use Move 1)");
    }

     void Update()
    {
        // Keyboard tracking for now until we make a good UI Menu
        if(currentState == BattleState.PLAYERTURN)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                //Execute First Move
                ExecuteMove(playerCreature, enemyCreature,playerCreature.activeMoves[0]);
            }
        }
    }

    void ExecuteMove(ActiveCreature attacker, ActiveCreature defender, MoveData move)
    {
        currentState = BattleState.BUSY; // Lock inputs so can't spam moves
        Debug.Log($"{attacker.baseData.speciesName} used {move.moveName}!");

       
    // 1. CRITICAL HIT MATH (5% chance to crit)
    bool isCrit = Random.value <= criticalHitChance;
    float critMultiplier = isCrit ? 1.5f : 1.0f; 

    // 2. RANDOM DAMAGE VARIANCE (85% to 100% of full power)
    float randomVariance = Random.Range(0.85f, 1.00f);

    // 3. FIXED PACE MATH (The Formula Overhaul)
    // We introduce a level weight component and a scaling divisor to control the huge Move Power numbers.
    float levelWeight = ((2f * attacker.currentLevel) / 5f) + 2f; 
    
    // Prevent Division by Zero
    float targetDefense = Mathf.Max(1f, defender.defense);

    // Calculate core mitigation: (Attack / Defense)
    float statRatio = (float)attacker.attack / targetDefense;

    // Divide by 50 (or a custom pacing scaler) to shrink the Move Power down to an digestible size
    float rawDamage = ((levelWeight * move.power * statRatio) / 50f);

    // 4. ADDITIVE FLOOR + FINAL ROUNDING
    // Adding +2 guarantees a baseline output. Even if stats are identical or defense is buffed to high heaven,
    // the floor ensures early combat progresses cleanly.
    int finalDamage = Mathf.RoundToInt(rawDamage * randomVariance * critMultiplier) + 2;

    // Absolute fallback constraint just in case code encounters a edge-case negative modifier
    finalDamage = Mathf.Max(1, finalDamage);

    if (isCrit) Debug.Log("<color=yellow>A critical hit!</color>");

    // Deal damage
    defender.TakeDamage(finalDamage);

        if (isCrit) Debug.Log("<color=yellow>A critical hit!</color>");


        // Deal damage
        defender.TakeDamage(finalDamage);

        // Check if the defender has fainted
        if (defender.currentHp <= 0)
        {
            EndBattle(attacker == playerCreature); // If attacker was player, player won!
        }
        else
        {
            // If the battle continues, pass the turn to the other combatant
            if (attacker == playerCreature)
            {
                Invoke(nameof(EnemyTurn), 1.5f); // Wait so text is readable
            }
            else
            {
                Invoke(nameof(PlayerTurn), 1.5f); // Wait so text is readable
            }
            
        }
    }
    
    void EnemyTurn()
    {
        currentState = BattleState.ENEMYTURN;
        Debug.Log($"{enemyCreature.baseData.speciesName} is deciding what to do...");

        // AI Logic: For now, just use the first move
        MoveData enemyMove = enemyCreature.activeMoves[0];

        ExecuteMove(enemyCreature, playerCreature, enemyMove);
    }

    void EndBattle(bool playerWon)
    {
        if(playerWon)
        {
            currentState = BattleState.WON;
            Debug.Log("Player won the battle!");
        }
        else
        {
            currentState = BattleState.LOST;
            Debug.Log("Player lost the battle!");
        }
    }
}


