using System;
using UnityEngine;

public enum WeaponType
{
    None,
    SoulSword,
    SpecterAxe,
    ShadeBow,
    SpiritBracers
}

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int essence_stored;

    [Header("Movement")]
    [SerializeField] private float move_speed = 5f;
    [SerializeField] private float turn_speed = 360f;

    [Header("Skills")]
    [SerializeField] private float player_health = 10f;
    [SerializeField] private float swipe_damage = 1f;
    [SerializeField] private float possess_cooldown = 15f;
    [SerializeField] private float scream_cooldown = 10f;
    [SerializeField] private int scream_level = 0;
    [SerializeField] private float atk_speed = 0.5f;

    [Header("Weapon Settings")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.None;

    private float nextPossessTime = 0f;

    public static event Action<GameObject> OnPossessAttempt;
    public static event Action OnUnpossessAttempt;

    public bool IsActiveController { get; private set; } = false;

    // ====== Getters ======
    public Rigidbody GetRigidBody() => rb;
    public float GetSpeed() => move_speed;
    public float GetTurnSpeed() => turn_speed;
    public float GetMaxHealth() => player_health;
    public float GetSwipeDamage() => swipe_damage;
    public float GetPossessCooldown() => possess_cooldown;
    public float GetAttackSpeed() => atk_speed;
    public float GetScreamCooldown() => scream_cooldown;
    public int GetScreamLevel() => scream_level;
    public int GetEssenceStored() => essence_stored;
    public WeaponType GetCurrentWeapon() => currentWeapon;

    // ====== Setters ======
    public void SetActiveController(bool state) => IsActiveController = state;
    public void SetSpeed(float value) => move_speed = Mathf.Max(0, value);
    public void SetTurnSpeed(float value) => turn_speed = Mathf.Max(0, value);
    public void SetSwipeDamage(float value) => swipe_damage = Mathf.Max(0, value);
    public void SetAttackSpeed(float value) => atk_speed = Mathf.Max(0, value);
    public void SetPossessCooldown(float value) => possess_cooldown = Mathf.Max(0, value);
    public void SetScreamCooldown(float value) => scream_cooldown = Mathf.Max(0, value);
    public void SetScreamLevel(int value) => scream_level = Math.Max(0, value);
    public void SetEssenceStored(int value) => essence_stored = Math.Max(0, value);
    public void SetMaxHealth(float value) => player_health = Mathf.Max(1, value);
    public void EquipWeapon(WeaponType weapon) => currentWeapon = weapon;

    private void Start()
    {
        IsActiveController = true; // Ghost is input owner initially
    }

    private void Update()
    {
        if (IsActiveController)
            HandlePossessionInput();
    }

    private void HandlePossessionInput()
    {
        if (Time.time < nextPossessTime)
            return;

        if (Input.GetButtonDown("Possess"))
        {
            if (PossessionManager.Instance.IsPossessing)
            {
                // Unpossess
                OnUnpossessAttempt?.Invoke();
            }
            else
            {
                // Try to possess nearest
                GameObject closest = FindClosestPossessable();
                if (closest != null)
                    OnPossessAttempt?.Invoke(closest);
            }
        }
    }

    private GameObject FindClosestPossessable()
    {
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

        return closest;
    }

    public void SetPossessionCooldown()
    {
        nextPossessTime = Time.time + possess_cooldown;
    }
}
