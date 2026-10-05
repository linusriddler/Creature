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
    public BaseCreatureData enemyTemplate;

     void Start()
    {
        currentState = BattleState.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        Debug.Log("--- BATTLE START ---");

        //Initialize both creatures at level 5 with templates
        playerCreature.Initialize(playerTemplate, 5);
        enemyCreature.Initialize(enemyTemplate, 5);

        Debug.Log($"A wild {enemyCreature.baseData.speciesName} appeared !");
        Debug.Log($"Player sent out {playerCreature.baseData.speciesName}!");

        //Player goes first until we have speed checks
        PlayerTurn();
       
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

        // SIMPLE DAMAGE FORMULA: Attacker, Attack + Move Power
        int damage = attacker.attack + move.power;

        // Deal the damage to the defender
        defender.TakeDamage(damage);

        // Check if the defender has fainted
        if(defender.currentHp <= 0)
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


