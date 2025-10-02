using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerController))]
public class EnemmyPossesTest : MonoBehaviour, IPossessable
{
    private EnemyAI ai;
    private PlayerMovementController movementController;

    void Awake()
    {
        ai = GetComponent<EnemyAI>();
        movementController = GetComponent<PlayerMovementController>();

        if (movementController != null)
            movementController.enabled = false; // off until possessed
    }

    public void OnPossessed(GameObject possessor)
    {
        if (ai) ai.enabled = false;

        if (movementController)
        {
            // Make the movement controller use THIS object's PlayerController
            movementController.SetController(GetComponent<PlayerController>());
            movementController.enabled = true;
        }

        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Debug.Log($"{name} possessed with speed {GetComponent<PlayerController>().GetSpeed()}");
    }

    public void OnUnpossessed()
    {
        if (ai) ai.enabled = true;
        if (movementController) movementController.enabled = false;
        
        var agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = true;
    }
}
