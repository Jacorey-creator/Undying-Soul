using System;
using System.Collections;
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
    [Header("Core References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private HealthUIController healthUIController;
    [SerializeField] private Animator animator;

    [Header("Player Stats")]
    [SerializeField] private float move_speed = 5f;
    [SerializeField] private float turn_speed = 360f;
    [SerializeField] private float player_health = 10f;
    [SerializeField] private float swipe_damage = 1f;
    [SerializeField] private float atk_speed = 0.5f;

    [Header("Skills & Cooldowns")]
    [SerializeField] private float possess_cooldown = 15f;
    [SerializeField] private float scream_cooldown = 10f;
    [SerializeField] private int scream_level = 0;
    [SerializeField] private int essence_stored;

    [Header("Weapon Settings")]
    [SerializeField] private WeaponType currentWeapon = WeaponType.None;

    private float nextPossessTime = 0f;
    private bool isPossessingAnimation = false;
    private int possessionLayerIndex = -1;

    // ====== EVENTS ======
    public static event Action<GameObject> OnPossessAttempt;
    public static event Action OnUnpossessAttempt;

    // ====== STATE ======
    public bool IsActiveController { get; private set; } = false;

    // ====== GETTERS ======
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

    // ====== SETTERS ======
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

    // ====== UNITY EVENTS ======
    private void Start()
    {
        IsActiveController = true;

        if (healthUIController)
            healthUIController.SetTarget(GetComponent<PlayerHealthController>());

        if (animator)
        {
            int layerIndex = animator.GetLayerIndex("PossessionPrep");
            possessionLayerIndex = layerIndex >= 0 ? layerIndex : -1;
        }
    }

    private void Update()
    {
        if (!GameStateManager.IsGameplay)
            return;

        if (IsActiveController && !isPossessingAnimation)
            HandlePossessionInput();
    }

    // ====== POSSESSION HANDLING ======
    private void HandlePossessionInput()
    {
        if (Time.time < nextPossessTime)
            return;

        if (Input.GetButtonDown("Possess"))
        {
            if (PossessionManager.Instance.IsPossessing)
            {
                if(animator) OnPossessionEnd();
                StartCoroutine(PlayPossessionAnimation(isUnpossess: true));
            }
            else
            {
                GameObject closest = FindClosestPossessable();
                if (closest != null)
                    StartCoroutine(PlayPossessionAnimation(isUnpossess: false, target: closest));
            }
        }
    }
    public void OnPossessionEnd()
    {
        // Play the unpossession animation
        animator.SetTrigger("EndPossession");
    }
    private IEnumerator PlayPossessionAnimation(bool isUnpossess, GameObject target = null)
    {
        isPossessingAnimation = true;

        if (animator == null || possessionLayerIndex < 0)
        {
            // Fallback: no animator, perform instantly
            yield return null;
            if (isUnpossess)
                OnUnpossessAttempt?.Invoke();
            else if (target != null)
                OnPossessAttempt?.Invoke(target);

            SetPossessionCooldown();
            isPossessingAnimation = false;
            yield break;
        }

        // Fade in Possession layer
        float fadeIn = 0f;
        while (fadeIn < 1f)
        {
            fadeIn += Time.deltaTime * 5f;
            animator.SetLayerWeight(possessionLayerIndex, Mathf.Lerp(0f, 1f, fadeIn));
            yield return null;
        }

        // Trigger the animation
        string triggerName = isUnpossess ? "Unpossess" : "StartPossession";
        animator.SetTrigger(triggerName);

        yield return null; // let animator update

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(possessionLayerIndex);
        float animLength = stateInfo.length / Mathf.Max(stateInfo.speed, 0.01f);
        yield return new WaitForSeconds(animLength);

        if (isUnpossess)
            OnUnpossessAttempt?.Invoke();
        else if (target != null)
            OnPossessAttempt?.Invoke(target);

        // Fade out layer
        float fadeOut = 0f;
        while (fadeOut < 1f)
        {
            fadeOut += Time.deltaTime * 5f;
            animator.SetLayerWeight(possessionLayerIndex, Mathf.Lerp(1f, 0f, fadeOut));
            yield return null;
        }

        isPossessingAnimation = false;
        SetPossessionCooldown();
    }

    // ====== FIND NEAREST POSSESSABLE ======
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

    // ====== COOLDOWN ======
    public void SetPossessionCooldown()
    {
        nextPossessTime = Time.time + possess_cooldown;
    }
}
