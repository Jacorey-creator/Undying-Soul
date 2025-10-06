using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class PossessionManager : MonoBehaviour
{
    public static PossessionManager Instance { get; private set; }

    [SerializeField] private PlayerMovementController ghostMovement;
    [SerializeField] private PlayerController ghostController; // ghost’s controller
    [SerializeField] private PlayerHealthController ghostHealth; // ghost’s health controller
    [SerializeField] private GameObject ghost;
    [SerializeField] private CinemachineCamera vcam;

    [Header("Possession Settings")]
    [SerializeField] private float possessionDuration = 10f;
    [SerializeField] private float drainRate = 1f; // how much health drains per second
    [SerializeField] private float transferEfficiency = 1f; // 1 = full health transfer

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip possessSound;
    [SerializeField] private AudioClip unpossessSound;

    private IPossessable currentPossessed;
    private EnemyBaseAI currentEnemyAI;
    private Coroutine possessionTimerCoroutine;
    private bool isPossessing = false;

    public bool IsPossessing => isPossessing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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
        if (isPossessing) return;

        currentPossessed = target.GetComponent<IPossessable>();
        currentEnemyAI = target.GetComponent<EnemyBaseAI>();
        if (currentPossessed == null || currentEnemyAI == null) return;

        // Disable ghost
        ghostMovement.enabled = false;
        ghost.SetActive(false);

        AudioHelper.PlaySound(possessSound, audioSource);

        // Hand control to the possessed enemy
        currentPossessed.OnPossessed(ghost);
        vcam.LookAt = target.transform;

        isPossessing = true;
        ghostController.SetPossessionCooldown();

        // Start possession drain
        possessionTimerCoroutine = StartCoroutine(PossessionRoutine());
        Debug.Log($"Possessed {target.name}!");
    }

    private IEnumerator PossessionRoutine()
    {
        float elapsed = 0f;

        while (elapsed < possessionDuration && currentEnemyAI != null)
        {
            // Drain enemy health over time
            float drainAmount = drainRate * Time.deltaTime;
            currentEnemyAI.health -= drainAmount;

            // Heal player by the same amount (adjusted by efficiency)
            ghostHealth.Heal(drainAmount * transferEfficiency);

            // If the enemy dies mid-possession, end early
            if (currentEnemyAI.health <= 0)
            {
                Debug.Log("Possessed target died — ending possession.");
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // End possession automatically
        Unpossess();
    }

    private void Unpossess()
    {
        if (!isPossessing) return;

        // Stop any running possession timers
        if (possessionTimerCoroutine != null)
            StopCoroutine(possessionTimerCoroutine);

        currentPossessed?.OnUnpossessed();
        Transform enemyTransform = (currentPossessed as MonoBehaviour)?.transform;

        currentPossessed = null;
        currentEnemyAI = null;

        AudioHelper.PlaySound(unpossessSound, audioSource);

        // Respawn ghost near enemy
        if (enemyTransform != null)
            ghost.transform.position = enemyTransform.position + new Vector3(0, 0, 1.5f);

        ghost.SetActive(true);
        ghostMovement.enabled = true;
        vcam.LookAt = ghost.transform;

        isPossessing = false;
        ghostController.SetPossessionCooldown();

        Debug.Log("Unpossessed back to ghost!");
    }
}
