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
        float critMultiplier = isCrit ? 1.5f : 1.0f; // 1.5x damage on crit

        // 2. RANDOM DAMAGE VARIANCE (90% to 100% of full power)
        float randomVariance = Random.Range(0.90f, 1.00f);

        // 3. CALCULATE COMBINED DAMAGE POOL
        int baseDamage = attacker.attack * move.power;
        // Prevent Division by Zero by forcing defense to be at least 1
        float baseDefense = Mathf.Max(1f, defender.defense);

        // Perform math entirely in float space, round it, and enforce a 1 HP minimum floor
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt((float)baseDamage * randomVariance * critMultiplier / baseDefense));

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


