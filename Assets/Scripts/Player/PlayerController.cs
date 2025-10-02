using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float move_speed = 5f;
    [SerializeField] private float turn_speed = 360f;

    [Header("Skills")]
    [SerializeField] private float player_health = 10f;
    [SerializeField] private float swipe_damage = 1f;
    [SerializeField] private float possess_cooldown = 15f;
    [SerializeField] private float atk_speed = 0.5f;

    private float nextPossessTime = 0f;
    private bool isPossessing = false;

    public static event Action<GameObject> OnPossessAttempt;
    public static event Action OnUnpossessAttempt;

    public bool IsActiveController { get; private set; } = false;

    public void SetActiveController(bool state)
    {
        IsActiveController = state;
    }

    // Getters
    public Rigidbody GetRigidBody() => rb;
    public float GetSpeed() => move_speed;
    public float GetTurnSpeed() => turn_speed;

    void Start()
    {
        IsActiveController = true; // ghost can always read input first
    }
    private void Update()
    {
        if (IsActiveController)
            HandlePossessionInput();
    }

    private void HandlePossessionInput()
    {
        if (Time.time < nextPossessTime) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isPossessing)
            {
                // Try to possess something
                float range = 3f;
                Collider[] hits = Physics.OverlapSphere(transform.position, range);
                GameObject closest = null;
                float minDist = Mathf.Infinity;

                foreach (var hit in hits)
                {
                    if (hit.TryGetComponent<IPossessable>(out _))
                    {
                        float dist = (hit.transform.position - transform.position).sqrMagnitude;
                        if (dist < minDist)
                        {
                            minDist = dist;
                            closest = hit.gameObject;
                        }
                    }
                }

                if (closest != null)
                {
                    OnPossessAttempt?.Invoke(closest);
                    isPossessing = true;
                    nextPossessTime = Time.time + possess_cooldown;
                }
            }
            else
            {
                // If already possessing, then pressing E = unpossess
                OnUnpossessAttempt?.Invoke();
                isPossessing = false;
                nextPossessTime = Time.time + possess_cooldown;
            }
        }
    }
}
