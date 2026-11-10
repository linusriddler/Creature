
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
        ShowPanel(attackPanel);

        selectedMove = -1;
        moveDescription.text = "Select a move to inspect it.";
        useMoveButton.interactable = false;

        var moves = battleManager.playerCreature.activeMoves;

        for (int i = 0; i < moveButtons.Length; i++)
        {
            bool valid = i < moves.Count && moves[i] != null;

            moveButtons[i].gameObject.SetActive(valid);

            if (valid)
                moveLabels[i].text = moves[i].moveName;
        }
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
        itemDescription.text = "Select an item to inspect it.";
        useItemButton.interactable = false;

        GameSession session = GameSession.Instance;

        int potions = session != null ? session.potionCount : 0;
        int superPotions =
            session != null ? session.superPotionCount : 0;

        itemLabels[0].text = $"Potion x{potions}";
        itemLabels[1].text = $"Super Potion x{superPotions}";

        itemButtons[0].interactable = potions > 0;
        itemButtons[1].interactable = superPotions > 0;
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
        creatureDetails.text =
            "Select a creature to inspect its stats.";

        switchButton.interactable = false;

        GameSession session = GameSession.Instance;

        for (int i = 0; i < teamButtons.Length; i++)
        {
            bool valid =
                session != null &&
                i < session.team.Count &&
                session.team[i] != null &&
                session.team[i].species != null;

            teamButtons[i].gameObject.SetActive(valid);

            if (!valid)
                continue;

            CreatureInstanceData data = session.team[i];

            bool active = i == session.activeCreatureIndex;

            teamLabels[i].text =
                $"{data.species.speciesName}  Lv. {data.level}" +
                (active ? " (Active)" : "");
        }
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
}
