
using System.Collections;
using UnityEngine;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    BUSY,
    WON,
    LOST,
    ESCAPED
}



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

    [Header("Player Settings")]
    [Range(1, 100)]
    public int startingPlayerLevel = 5;

    [Header("Encounter Settings")]
    [Range(1, 100)]
    public int minimumEncounterLevel = 3;

    [Range(1, 100)]
    public int maximumEncounterLevel = 6;

    [Header("Combat Settings")]
    [Range(0f, 1f)]
    public float criticalHitChance = 0.05f;

    public float actionDelay = 0.6f;



    // The move chosen by the player for the current turn.
    private MoveData selectedPlayerMove;

    private void Start()
    {
        SetupBattle();
    }

    private enum PlayerActionChoice
    {
        None,
        Attack,
        Item,
        Switch,
        Run
    }

    private PlayerActionChoice pendingAction =
    PlayerActionChoice.None;

    private int pendingActionIndex = -1;
    private bool escapedBattle = false;




    private void SetupBattle()
    {
        currentState = BattleState.START;

        Debug.Log("--- BATTLE START ---");

        // STEP 1: Check required references
        if (playerCreature == null || spawnManager == null)
        {
            Debug.LogError(
                "BattleManager is missing PlayerCreature or SpawnManager!"
            );
            enabled = false;
            return;
        }

        // STEP 2: Load the player's creature from GameSession
        if (GameSession.Instance != null)
        {
            bool loadedSuccessfully =
                GameSession.Instance.LoadActiveCreature(playerCreature);

            if (!loadedSuccessfully)
            {
                Debug.LogError(
                    "Failed to load the player's creature from GameSession!"
                );
                enabled = false;
                return;
            }

            Debug.Log(
                $"Loaded {playerCreature.baseData.speciesName} " +
                $"from team at Level {playerCreature.currentLevel}. " +
                $"HP: {playerCreature.currentHp}/{playerCreature.maxHP}"
            );
        }
        else
        {
            // Fallback for testing the battle scene directly
            // without entering from the Overworld.
            if (!playerCreature.IsInitialized)
            {
                BaseCreatureData data =
                    playerTemplate != null
                    ? playerTemplate
                    : playerCreature.baseData;

                if (data == null)
                {
                    Debug.LogError("No player creature template assigned!");
                    enabled = false;
                    return;
                }

                playerCreature.Initialize(data, startingPlayerLevel);
            }
        }

        // STEP 3: Make sure the player can battle
        if (!playerCreature.IsInitialized || playerCreature.IsFainted)
        {
            Debug.LogError(
                "Player creature is uninitialized or has fainted!"
            );
            enabled = false;
            return;
        }

        // STEP 4: Generate a random wild creature
        int minLevel = Mathf.Clamp(
            minimumEncounterLevel,
            1,
            CombatBalance.MaxLevel
        );

        int maxLevel = Mathf.Clamp(
            maximumEncounterLevel,
            minLevel,
            CombatBalance.MaxLevel
        );

        enemyCreature = spawnManager.GenerateRandomEncounter(
            minLevel,
            maxLevel
        );

        // STEP 5: Make sure the enemy spawned correctly
        if (enemyCreature == null || !enemyCreature.IsInitialized)
        {
            Debug.LogError("Failed to generate enemy creature!");
            enabled = false;
            return;
        }

        // STEP 6: Verify both creatures have usable moves
        if (!HasUsableMoves(playerCreature) ||
            !HasUsableMoves(enemyCreature))
        {
            Debug.LogError(
                "One or both creatures have no usable moves!"
            );
            enabled = false;
            return;
        }

        // STEP 7: Display battle information
        Debug.Log(
            $"{playerCreature.baseData.speciesName} " +
            $"(Level {playerCreature.currentLevel}) " +
            $"VS Wild {enemyCreature.baseData.speciesName} " +
            $"(Level {enemyCreature.currentLevel})"
        );

        // STEP 8: Start the turn-based battle
        StartCoroutine(BattleLoop());
    }

    private bool HasUsableMoves(ActiveCreature creature)
    {
        if (creature.activeMoves == null)
            return false;

        foreach (MoveData move in creature.activeMoves)
        {
            if (move != null)
                return true;
        }

        return false;
    }

    private bool BothCreaturesAlive()
    {
        return !playerCreature.IsFainted && !enemyCreature.IsFainted;
    }


    private IEnumerator BattleLoop()
    {
        escapedBattle = false;

        while (BothCreaturesAlive() && !escapedBattle)
        {
            bool playerFirst =
                playerCreature.speed > enemyCreature.speed ||
                (playerCreature.speed == enemyCreature.speed &&
                 Random.value < 0.5f);

            if (playerFirst)
            {
                yield return PlayerAction();

                if (!BothCreaturesAlive() || escapedBattle)
                    break;

                yield return EnemyAction();
            }
            else
            {
                yield return EnemyAction();

                if (!BothCreaturesAlive() || escapedBattle)
                    break;

                yield return PlayerAction();
            }
        }

        // Escaping should not count as a win or loss.
        if (!escapedBattle)
            EndBattle();
    }



    private IEnumerator PlayerAction()
    {
        pendingAction = PlayerActionChoice.None;
        pendingActionIndex = -1;

        currentState = BattleState.PLAYERTURN;

        Debug.Log("Choose an action!");

        // Wait for the player to use one of the UI options.
        yield return new WaitUntil(
            () => pendingAction != PlayerActionChoice.None
        );

        currentState = BattleState.BUSY;

        switch (pendingAction)
        {
            case PlayerActionChoice.Attack:
                {
                    MoveData move =
                        playerCreature.activeMoves[pendingActionIndex];

                    yield return ExecuteMove(
                        playerCreature,
                        enemyCreature,
                        move
                    );

                    break;
                }

            case PlayerActionChoice.Item:
                {
                    GameSession session = GameSession.Instance;

                    int healAmount;

                    if (pendingActionIndex == 0)
                    {
                        session.potionCount--;
                        healAmount = 20;
                    }
                    else
                    {
                        session.superPotionCount--;
                        healAmount = 50;
                    }

                    playerCreature.Heal(healAmount);

                    Debug.Log(
                        $"Used a healing item! " +
                        $"HP: {playerCreature.currentHp}/{playerCreature.maxHP}"
                    );

                    yield return new WaitForSeconds(actionDelay);
                    break;
                }

            case PlayerActionChoice.Switch:
                {
                    GameSession session = GameSession.Instance;

                    // Save the outgoing creature.
                    session.SaveActiveCreature(playerCreature);

                    // Change the active team member.
                    session.activeCreatureIndex = pendingActionIndex;

                    // Load the incoming creature's stats.
                    session.LoadActiveCreature(playerCreature);

                    Debug.Log(
                        $"Go, {playerCreature.baseData.speciesName}!"
                    );

                    yield return new WaitForSeconds(actionDelay);
                    break;
                }

            case PlayerActionChoice.Run:
                {
                    escapedBattle = true;
                    currentState = BattleState.ESCAPED;

                    Debug.Log("Successfully escaped the battle!");

                    yield return new WaitForSeconds(actionDelay);

                    if (GameSession.Instance != null)
                    {
                        GameSession.Instance.SaveActiveCreature(
                            playerCreature
                        );

                        GameSession.Instance.ReturnToOverworld();
                    }

                    break;
                }
        }
    }


    private IEnumerator EnemyAction()
    {
        currentState = BattleState.ENEMYTURN;

        Debug.Log(
            $"{enemyCreature.baseData.speciesName} is choosing a move..."
        );

        yield return new WaitForSeconds(actionDelay);

        // Basic AI: randomly choose an available move.
        int index = Random.Range(
            0,
            enemyCreature.activeMoves.Count
        );

        MoveData chosenMove = enemyCreature.activeMoves[index];

        yield return ExecuteMove(
            enemyCreature,
            playerCreature,
            chosenMove
        );
    }



    private IEnumerator ExecuteMove(
        ActiveCreature attacker,
        ActiveCreature defender,
        MoveData move)
    {
        currentState = BattleState.BUSY;

        Debug.Log(
            $"{attacker.baseData.speciesName} used {move.moveName}!"
        );

        yield return new WaitForSeconds(0.25f);

        // STEP 1: ACCURACY CHECK
        int accuracyRoll = Random.Range(0, 100);

        if (accuracyRoll >= move.accuracy)
        {
            Debug.Log($"{move.moveName} missed!");

            yield return new WaitForSeconds(actionDelay);
            yield break;
        }

        // STEP 2: DODGE CHECK
        float dodgeChance = CombatBalance.CalculateDodgeChance(
            attacker.speed,
            defender.speed
        );

        if (Random.value < dodgeChance)
        {
            Debug.Log(
                $"<color=cyan>{defender.baseData.speciesName} " +
                $"dodged the attack!</color>"
            );

            yield return new WaitForSeconds(actionDelay);
            yield break;
        }

        // STEP 3: DETERMINE OFFENSIVE STAT
        int offensiveStat;

        if (move.offensiveStat == MoveOffenseStat.Defense)
        {
            offensiveStat = Mathf.RoundToInt(attacker.defense);
        }
        else
        {
            offensiveStat = attacker.attack;
        }

        // STEP 4: CRITICAL HIT CHECK
        bool isCritical = Random.value < criticalHitChance;

        // STEP 5: CALCULATE DAMAGE
        int damage = CombatBalance.CalculateDamage(
            attacker.currentLevel,
            offensiveStat,
            defender.defense,
            move.power,
            isCritical
        );

        if (isCritical)
        {
            Debug.Log("<color=yellow>Critical hit!</color>");
        }

        // STEP 6: APPLY DAMAGE
        defender.TakeDamage(damage);

        yield return new WaitForSeconds(actionDelay);
    }


    // This method can also be called from future UI buttons.

    public void SelectMove(int index)
    {
        if (currentState != BattleState.PLAYERTURN)
            return;

        if (index < 0 ||
            index >= playerCreature.activeMoves.Count ||
            playerCreature.activeMoves[index] == null)
            return;

        pendingActionIndex = index;
        pendingAction = PlayerActionChoice.Attack;

        currentState = BattleState.BUSY;
    }

    public void SelectRun()
    {
        if (currentState != BattleState.PLAYERTURN)
            return;

        pendingAction = PlayerActionChoice.Run;
        currentState = BattleState.BUSY;
    }

    public void SelectItem(int itemIndex)
    {
        if (currentState != BattleState.PLAYERTURN)
            return;

        GameSession session = GameSession.Instance;

        if (session == null)
            return;

        if (playerCreature.IsFainted ||
            playerCreature.currentHp >= playerCreature.maxHP)
            return;

        if (itemIndex == 0 && session.potionCount <= 0)
            return;

        if (itemIndex == 1 && session.superPotionCount <= 0)
            return;

        if (itemIndex < 0 || itemIndex > 1)
            return;

        pendingActionIndex = itemIndex;
        pendingAction = PlayerActionChoice.Item;

        currentState = BattleState.BUSY;
    }

    public void SelectSwitch(int teamIndex)
    {
        if (currentState != BattleState.PLAYERTURN)
            return;

        GameSession session = GameSession.Instance;

        if (session == null)
            return;

        if (teamIndex < 0 || teamIndex >= session.team.Count)
            return;

        if (teamIndex == session.activeCreatureIndex)
            return;

        CreatureInstanceData creature = session.team[teamIndex];

        if (creature == null ||
            creature.species == null ||
            creature.currentHP <= 0)
            return;

        pendingActionIndex = teamIndex;
        pendingAction = PlayerActionChoice.Switch;

        currentState = BattleState.BUSY;
    }


    private void Update()
    {
        if (currentState != BattleState.PLAYERTURN)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1) ||
            Input.GetKeyDown(KeyCode.Space))
        {
            SelectMove(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectMove(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectMove(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectMove(3);
        }
    }

    private void EndBattle()
    {
        bool playerWon =
            !playerCreature.IsFainted && enemyCreature.IsFainted;

        if (playerWon)
        {
            currentState = BattleState.WON;

            Debug.Log("Player won the battle!");

            int reward = CombatBalance.ExperienceReward(
                enemyCreature.currentLevel
            );

            Debug.Log($"Gained {reward} EXP!");

            playerCreature.GainExperience(reward);
        }
        else
        {
            currentState = BattleState.LOST;

            Debug.Log("Player lost the battle!");

            // Temporary prototype behavior:
            // Heal after defeat to avoid returning with
            // an unusable creature.
            playerCreature.HealToFull();
        }

        // Save updated progress to the persistent team.
        if (GameSession.Instance != null)
        {
            GameSession.Instance.SaveActiveCreature(playerCreature);

            StartCoroutine(ReturnAfterBattle());
        }
    }

    private IEnumerator ReturnAfterBattle()
    {
        yield return new WaitForSecondsRealtime(2f);

        GameSession.Instance.ReturnToOverworld();
    }

}
