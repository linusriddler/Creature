using UnityEngine;

// This line allows you to right-click in Unity and create a new move file
[CreateAssetMenu(fileName = "New Move", menuName = "Creature Game/Move")]
public class MoveData : ScriptableObject
{
    public string moveName;
    public int power;
    public int accuracy = 100;

    // Later for element types (Fire, Water, etc.)
    public string elementPack;

}
