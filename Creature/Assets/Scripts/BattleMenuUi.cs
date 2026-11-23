
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleMenuUI : MonoBehaviour
{
    [Header("Battle")]
    public BattleManager battleManager;

    [Header("Menu Root and Panels")]
    public GameObject menuRoot;
    public GameObject actionPanel;
    public GameObject attackPanel;
    public GameObject itemsPanel;
    public GameObject teamPanel;

    [Header("Main Menu Buttons")]
    public Button attackButton;
    public Button runButton;
    public Button itemsButton;
    public Button teamButton;

    [Header("Back Buttons")]
    public Button attackBackButton;
    public Button itemsBackButton;
    public Button teamBackButton;

    [Header("Attack Menu")]
    public Button[] moveButtons = new Button[4];
    public TMP_Text[] moveLabels = new TMP_Text[4];
    public TMP_Text moveDescription;
    public Button useMoveButton;

    [Header("Items Menu")]
    public Button[] itemButtons = new Button[2];
    public TMP_Text[] itemLabels = new TMP_Text[2];
    public TMP_Text itemDescription;
    public Button useItemButton;

    [Header("Team Menu")]
    public Button[] teamButtons = new Button[6];
    public TMP_Text[] teamLabels = new TMP_Text[6];
    public TMP_Text creatureDetails;
    public Button switchButton;

    private int selectedMove = -1;
    private int selectedItem = -1;
    private int selectedTeamMember = -1;

    private BattleState previousState = BattleState.START;

    private void Awake()
    {
        // Main menu
        attackButton.onClick.AddListener(OpenAttackMenu);
        runButton.onClick.AddListener(() => battleManager.SelectRun());
        itemsButton.onClick.AddListener(OpenItemsMenu);
        teamButton.onClick.AddListener(OpenTeamMenu);

        // Back buttons
        attackBackButton.onClick.AddListener(ShowMainMenu);
        itemsBackButton.onClick.AddListener(ShowMainMenu);
        teamBackButton.onClick.AddListener(ShowMainMenu);

        // Attack buttons
        for (int i = 0; i < moveButtons.Length; i++)
        {
            int index = i;

            moveButtons[i].onClick.AddListener(
                () => PreviewMove(index)
            );
        }

        useMoveButton.onClick.AddListener(ConfirmMove);

        // Item buttons
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int index = i;

            itemButtons[i].onClick.AddListener(
                () => PreviewItem(index)
            );
        }

        useItemButton.onClick.AddListener(ConfirmItem);

        // Team buttons
        for (int i = 0; i < teamButtons.Length; i++)
        {
            int index = i;

            teamButtons[i].onClick.AddListener(
                () => PreviewTeamMember(index)
            );
        }

        switchButton.onClick.AddListener(ConfirmSwitch);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainMenu();
    }

    private void Update()
    {
        if (battleManager == null)
            return;

        bool playerTurn =
            battleManager.currentState == BattleState.PLAYERTURN;

        // Hide controls while an attack or enemy turn executes.
        if (menuRoot.activeSelf != playerTurn)
            menuRoot.SetActive(playerTurn);

        // Reset to main menu when a new player turn begins.
        if (playerTurn &&
            previousState != BattleState.PLAYERTURN)
        {
            ShowMainMenu();
        }

        previousState = battleManager.currentState;
    }

    private void ShowPanel(GameObject panel)
    {
        actionPanel.SetActive(panel == actionPanel);
        attackPanel.SetActive(panel == attackPanel);
        itemsPanel.SetActive(panel == itemsPanel);
        teamPanel.SetActive(panel == teamPanel);
    }

    public void ShowMainMenu()
    {
        ShowPanel(actionPanel);
    }

    // ---------------- ATTACK MENU ----------------


    public void OpenAttackMenu()
    {
        if (battleManager == null ||
            battleManager.playerCreature == null)
        {
            Debug.LogWarning("No active player creature!");
            return;
        }

        ShowPanel(attackPanel);

        selectedMove = -1;
        useMoveButton.interactable = false;

        moveDescription.text = "Select a move to inspect it.";

        var moves = battleManager.playerCreature.activeMoves;

        for (int i = 0; i < moveButtons.Length; i++)
        {
            bool hasMove =
                moves != null &&
                i < moves.Count &&
                moves[i] != null;

            string name = hasMove ? moves[i].moveName : "";

            UpdateSlot(moveButtons[i], hasMove, name);
        }

        Debug.Log(
            $"Attack menu refreshed: {moves?.Count ?? 0} move slots."
        );
    }


    private void PreviewMove(int index)
    {
        var moves = battleManager.playerCreature.activeMoves;

        if (index < 0 || index >= moves.Count)
            return;

        MoveData move = moves[index];

        if (move == null)
            return;

        selectedMove = index;

        string description =
            string.IsNullOrWhiteSpace(move.description)
            ? "No description available."
            : move.description;

        moveDescription.text =
            $"{move.moveName}\n\n" +
            $"{description}\n\n" +
            $"Power: {move.power}\n" +
            $"Accuracy: {move.accuracy}%\n" +
            $"Uses: {move.offensiveStat}";

        useMoveButton.interactable = true;
    }

    private void ConfirmMove()
    {
        if (selectedMove < 0)
            return;

        battleManager.SelectMove(selectedMove);
    }

    // ---------------- ITEMS MENU ----------------


    public void OpenItemsMenu()
    {
        ShowPanel(itemsPanel);

        selectedItem = -1;
        useItemButton.interactable = false;

        itemDescription.text = "Select an item to inspect it.";

        GameSession session = GameSession.Instance;

        int potions = session != null
            ? session.potionCount
            : 0;

        int superPotions = session != null
            ? session.superPotionCount
            : 0;

        // Show the actual inventory quantities.
        if (itemButtons.Length > 0)
        {
            UpdateSlot(
                itemButtons[0],
                true,
                $"Potion x{potions}"
            );

            itemButtons[0].interactable = potions > 0;
        }

        if (itemButtons.Length > 1)
        {
            UpdateSlot(
                itemButtons[1],
                true,
                $"Super Potion x{superPotions}"
            );

            itemButtons[1].interactable = superPotions > 0;
        }
    }

    private void PreviewItem(int index)
    {
        selectedItem = index;

        if (index == 0)
        {
            itemDescription.text =
                "Potion\n\nRestores 20 HP to your active creature.";
        }
        else if (index == 1)
        {
            itemDescription.text =
                "Super Potion\n\nRestores 50 HP to your active creature.";
        }

        ActiveCreature creature = battleManager.playerCreature;

        useItemButton.interactable =
            creature.currentHp > 0 &&
            creature.currentHp < creature.maxHP;
    }

    private void ConfirmItem()
    {
        if (selectedItem < 0)
            return;

        battleManager.SelectItem(selectedItem);
    }

    // ---------------- TEAM MENU ----------------


    public void OpenTeamMenu()
    {
        ShowPanel(teamPanel);

        selectedTeamMember = -1;
        switchButton.interactable = false;

        creatureDetails.text =
            "Select a creature to inspect its stats.";

        GameSession session = GameSession.Instance;

        for (int i = 0; i < teamButtons.Length; i++)
        {
            bool hasCreature =
                session != null &&
                session.team != null &&
                i < session.team.Count &&
                session.team[i] != null &&
                session.team[i].species != null;

            if (!hasCreature)
            {
                // Hide unused team slots completely.
                UpdateSlot(teamButtons[i], false, "");
                continue;
            }

            CreatureInstanceData creature = session.team[i];

            bool isActive = i == session.activeCreatureIndex;

            int maxHP = CombatBalance.CalculateMaxHP(
                creature.species.baseHealth,
                creature.level
            );

            int currentHP = creature.currentHP;

            // Read live battle HP for the active creature.
            if (isActive &&
                battleManager != null &&
                battleManager.playerCreature != null)
            {
                currentHP = battleManager.playerCreature.currentHp;
            }

            string displayName =
                $"{creature.species.speciesName} Lv.{creature.level}\n" +
                $"HP: {currentHP}/{maxHP}";

            if (isActive)
                displayName += " (Active)";

            UpdateSlot(teamButtons[i], true, displayName);
        }

        Debug.Log(
            $"Team menu refreshed: {session?.team?.Count ?? 0} creatures."
        );
    }

    private void PreviewTeamMember(int index)
    {
        GameSession session = GameSession.Instance;

        if (session == null ||
            index < 0 ||
            index >= session.team.Count)
            return;

        CreatureInstanceData creature = session.team[index];

        if (creature == null || creature.species == null)
            return;

        selectedTeamMember = index;

        bool active = index == session.activeCreatureIndex;

        int maxHP = CombatBalance.CalculateMaxHP(
            creature.species.baseHealth,
            creature.level
        );

        int hp = active
            ? battleManager.playerCreature.currentHp
            : creature.currentHP;

        int attack = CombatBalance.CalculateStat(
            creature.species.baseAttack,
            creature.level
        );

        float defense = CombatBalance.CalculateStat(
            creature.species.baseDefense,
            creature.level
        );

        int speed = CombatBalance.CalculateStat(
            creature.species.baseSpeed,
            creature.level
        );

        creatureDetails.text =
            $"{creature.species.speciesName}\n" +
            $"Level: {creature.level}\n" +
            $"HP: {hp}/{maxHP}\n" +
            $"Attack: {attack}\n" +
            $"Defense: {defense:F1}\n" +
            $"Speed: {speed}\n" +
            $"XP: {creature.experience}";

        // Cannot switch to the active or fainted creature.
        switchButton.interactable = !active && hp > 0;
    }

    private void ConfirmSwitch()
    {
        if (selectedTeamMember < 0)
            return;

        battleManager.SelectSwitch(selectedTeamMember);
    }


    private void UpdateSlot(Button button, bool visible, string label)
    {
        if (button == null)
            return;

        button.gameObject.SetActive(visible);

        if (!visible)
            return;

        // Finds the TextMeshPro text inside the button.
        TMP_Text buttonText =
            button.GetComponentInChildren<TMP_Text>(true);

        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

}


