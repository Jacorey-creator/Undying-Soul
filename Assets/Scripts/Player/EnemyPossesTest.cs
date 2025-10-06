using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerController))]
public class EnemmyPossesTest : MonoBehaviour, IPossessable
{
    private EnemyBaseAI ai;
    private PlayerController controller;
    private PlayerMovementController movementController;
    private NavMeshAgent navAgent;

    void Awake()
    {
        ai = GetComponent<EnemyBaseAI>();
        controller = GetComponent<PlayerController>();
        movementController = GetComponent<PlayerMovementController>();
        navAgent = GetComponent<NavMeshAgent>();

        if (controller != null) controller.enabled = false; // off until possessed
        if (movementController != null) movementController.enabled = false;
        if (ai != null) ai.enabled = true;
        if (navAgent != null) navAgent.enabled = true;
    }

    public void OnPossessed(GameObject ghost)
    {
        // Disable AI
        if (ai) ai.enabled = false;

        // Enable PlayerController input
        if (controller != null)
        {
            controller.SetActiveController(true); // enable input
            controller.enabled = true;             // make sure component is active
        }

        // Enable movement
        if (movementController != null)
            movementController.enabled = true;

        // Disable NavMeshAgent
        if (navAgent != null)
            navAgent.enabled = false;
    }

    public void OnUnpossessed()
    {
        if (ai) ai.enabled = true;

        if (controller != null)
        {
            controller.SetActiveController(false);
            // Optionally disable component if you prefer
            // controller.enabled = false;
        }

        if (movementController != null)
            movementController.enabled = false;

        if (navAgent != null)
            navAgent.enabled = true;
    }
}