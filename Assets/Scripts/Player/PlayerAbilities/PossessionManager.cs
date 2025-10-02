using UnityEngine;
using Unity.Cinemachine;

public class PossessionManager : MonoBehaviour
{
    public static PossessionManager Instance { get; private set; }

    [SerializeField] private PlayerMovementController ghostMovement;
    [SerializeField] private PlayerController ghostController; // the ghost’s controller
    [SerializeField] private GameObject ghost;
    [SerializeField] private CinemachineCamera vcam;

    private IPossessable currentPossessed;
    private bool isPossessing = false;

    public bool IsPossessing => isPossessing;
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // Optional: DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        PlayerController.OnPossessAttempt += Possess;
        PlayerController.OnUnpossessAttempt += Unpossess;
    }

    void OnDisable()
    {
        PlayerController.OnPossessAttempt -= Possess;
        PlayerController.OnUnpossessAttempt -= Unpossess;
    }

    private void Possess(GameObject target)
    {
        if (isPossessing) return; // already possessing something

        currentPossessed = target.GetComponent<IPossessable>();
        if (currentPossessed == null) return;

        // Disable ghost
        ghostMovement.enabled = false;
        ghost.SetActive(false);

        // Hand control to the possessed enemy
        currentPossessed.OnPossessed(ghost);

        // Update camera
        vcam.LookAt = target.transform;

        isPossessing = true;
        ghostController.SetPossessionCooldown(); // cooldown managed by ghostController
        Debug.Log("Possessed " + target.name);
    }

    private void Unpossess()
    {
        if (!isPossessing) return;

        // Release the current enemy
        currentPossessed?.OnUnpossessed();
        Transform enemyTransform = (currentPossessed as MonoBehaviour)?.transform;
        currentPossessed = null;

        // Respawn ghost near enemy
        if (enemyTransform != null)
            ghost.transform.position = enemyTransform.position + new Vector3(0, 0, 1.5f);

        ghost.SetActive(true);
        ghostMovement.enabled = true;

        // Update camera
        vcam.LookAt = ghost.transform;

        isPossessing = false;
        ghostController.SetPossessionCooldown(); // cooldown again
        Debug.Log("Unpossessed back to ghost");
    }
}
