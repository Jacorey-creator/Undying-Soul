using UnityEngine;
using Unity.Cinemachine;

public class PossessionManager : MonoBehaviour
{
    [SerializeField] private PlayerMovementController ghostMovement;
    [SerializeField] private PlayerController ghostController;
    [SerializeField] private GameObject ghost;
    [SerializeField] private CinemachineCamera vcam;

    private IPossessable currentPossessed;
    private PlayerController possessedController;

    void OnEnable()
    {
        // Ghost listens by default
        PlayerController.OnPossessAttempt += Possess;
        PlayerController.OnUnpossessAttempt += Unpossess;
    }

    void OnDisable()
    {
        // Always clean up
        PlayerController.OnPossessAttempt -= Possess;
        PlayerController.OnUnpossessAttempt -= Unpossess;
    }

    private void Possess(GameObject target)
    {
        Unpossess();

        currentPossessed = target.GetComponent<IPossessable>();
        possessedController = target.GetComponent<PlayerController>();

        if (currentPossessed == null || possessedController == null) return;

        // Disable ghost
        ghostMovement.enabled = false;
        ghostController.SetActiveController(false);
        ghost.SetActive(false);

        // Enable possessed input
        possessedController.SetActiveController(true);

        currentPossessed.OnPossessed(ghost);

        vcam.Target.TrackingTarget = target.transform;

        Debug.Log("Possessed " + target.name);
    }

    private void Unpossess()
    {
        if (currentPossessed != null)
        {
            currentPossessed.OnUnpossessed();

            if (possessedController != null)
                possessedController.SetActiveController(false);

            // Get enemy transform before clearing
            Transform enemyTransform = (currentPossessed as MonoBehaviour).transform;

            currentPossessed = null;
            possessedController = null;

            // Set ghost position slightly above the enemy
            Vector3 offset = new Vector3(0, 0, 1.5f); // 1 unit above
            ghost.transform.position = enemyTransform.position + offset;
        }

        // Re-enable ghost
        ghost.SetActive(true);
        ghostMovement.enabled = true;
        ghostController.SetActiveController(true);

        vcam.LookAt = ghost.transform;

        Debug.Log("Unpossessed back to ghost");
    }
}
