
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class EncounterZone : MonoBehaviour
{
    [Header("Battle Scene")]
    public string battleSceneName;

    [Header("Encounter Settings")]
    [Range(0f, 1f)]
    public float encounterChance = 0.15f;

    [Min(0.5f)]
    public float distancePerCheck = 3f;

    private Transform player;
    private Vector3 previousPosition;
    private float distanceWalked;

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement movement =
            other.GetComponentInParent<PlayerMovement>();

        if (movement == null)
            return;

        player = movement.transform;

        previousPosition = player.position;
        distanceWalked = 0f;

        Debug.Log("Entered encounter zone: " + battleSceneName);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement movement =
            other.GetComponentInParent<PlayerMovement>();

        if (movement != null && movement.transform == player)
        {
            player = null;
            distanceWalked = 0f;
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        Vector3 movement = player.position - previousPosition;
        movement.y = 0f;

        previousPosition = player.position;

        if (GameSession.Instance == null ||
            !GameSession.Instance.CanStartEncounter)
            return;

        distanceWalked += movement.magnitude;

        if (distanceWalked < distancePerCheck)
            return;

        distanceWalked = 0f;

        if (Random.value < encounterChance)
        {
            GameSession.Instance.StartEncounter(
                battleSceneName,
                player
            );
        }
    }
}
