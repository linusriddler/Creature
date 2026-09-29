using UnityEngine;

public class UniversalCreature : MonoBehaviour
{
    private float maximumHealth;
    private float basicAttackDamage;
    void Start()
    {
        if (gameObject.CompareTag("Steve"))
        {
            maximumHealth = 10;
            basicAttackDamage = 10;
        }
        else if (gameObject.CompareTag("John"));
        {
            maximumHealth = 12;
            basicAttackDamage = 8;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
