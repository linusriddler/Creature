using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public Button Attack;
    public Transform FriendlyCreature;
    public Transform EnemyCreature;
    private bool friendlyAvailable;
    private bool enemyAvailable;
    void Start()
    {
        Attack.onClick.AddListener(OnButtonClick);
        friendlyAvailable = true;
        enemyAvailable = false;
    }
    void Update()
    {
        if(enemyAvailable == true)
        {
            StartCoroutine(EnemyMoveCreature());
            enemyAvailable = false;
        }
    }
    void OnButtonClick()
    {
        if (friendlyAvailable == true)
        {
            StartCoroutine(MoveCreature());
            friendlyAvailable = false;
        }
    }
    IEnumerator MoveCreature()
    {
        Vector3 start = FriendlyCreature.position;
        FriendlyCreature.position += FriendlyCreature.forward * 2f;
        yield return new WaitForSeconds(0.5f);
        FriendlyCreature.position = start;
        enemyAvailable = true;
    }
    IEnumerator EnemyMoveCreature()
    {
        Vector3 start = EnemyCreature.position;
        EnemyCreature.position += EnemyCreature.forward * 2f;
        yield return new WaitForSeconds(0.5f);
        EnemyCreature.position = start;
        friendlyAvailable = true;
    }
}
