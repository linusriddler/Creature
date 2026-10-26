using UnityEngine;
using System.Collections.Generic;

// This line allows you to right-click in Unity and create a new creature file
[CreateAssetMenu(fileName = "New Creature", menuName = "Creature Game/Base Creature")]
public class BaseCreatureData : ScriptableObject
{
    [Header("Identity")]
    public string speciesName;

    [Header("Visuals")]
    // 3D model goes here
    public GameObject creaturePrefab;

    [Header("Position & Rotation")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    [Header("Base Stats")]
    public int baseHealth;
    public int baseAttack;
    public float baseDefense;
    public int baseSpeed;

    [Header("Combat Moves")]
    // // This allows you to drag and drop MoveData files right into this creature
    public List<MoveData> learnableMoves;
}
