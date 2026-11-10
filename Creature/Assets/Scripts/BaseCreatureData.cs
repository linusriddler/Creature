
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New Creature",
    menuName = "Creature Game/Base Creature"
)]
public class BaseCreatureData : ScriptableObject
{
    [Header("Identity")]
    public string speciesName;

    [Header("Visuals")]
    public GameObject creaturePrefab;

    [Header("Position & Rotation")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    [Header("Base Stats (10 = Average)")]
    [Min(1)] public int baseHealth = 10;
    [Min(1)] public int baseAttack = 10;
    [Min(1)] public float baseDefense = 10f;
    [Min(1)] public int baseSpeed = 10;

    [Header("Combat Moves")]
    public List<MoveData> learnableMoves = new List<MoveData>();
}

