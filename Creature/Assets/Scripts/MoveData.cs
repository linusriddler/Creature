
using UnityEngine;

public enum MoveOffenseStat
{
    Attack,
    Defense
}

[CreateAssetMenu(
    fileName = "New Move",
    menuName = "Creature Game/Move"
)]
public class MoveData : ScriptableObject
{
    [Header("Identity")]
    public string moveName;

    [Header("Damage Settings")]
    [Min(1)]
    public int power = 40;

    [Range(1, 100)]
    public int accuracy = 100;

    [Tooltip("Determines which attacker stat is used to calculate damage.")]
    public MoveOffenseStat offensiveStat = MoveOffenseStat.Attack;

    [Header("Element")]
    public string elementPack;
}
