using System;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackController : MonoBehaviour
{
    private PlayerController controller;
    private float nextAttackTime = 0f;
    private float nextScreamTime = 0f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private LayerMask attackMask;

    [Header("Scream Settings")]
    [SerializeField] private float screamRadius = 5f;

    [Header("Weapon Modifiers")]
    [SerializeField] private float soulSwordRange = 3.5f;
    [SerializeField] private float soulSwordDamageMultiplier = 2f;
    [SerializeField] private float soulSwordKnockbackMultiplier = 0.5f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip screamClip;
    [SerializeField] private AudioClip soulSwordSound;

    public event Action OnAttack;
    public event Action OnScream;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        if (audioSource == null)  audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!GameStateManager.IsGameplay) return;
        if (!controller.IsActiveController) return;

        // Primary attack
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            PerformAttack(controller.GetCurrentWeapon());
            nextAttackTime = Time.time + controller.GetAttackSpeed();
        }

        // Scream ability
        if (Input.GetButtonDown("Fire2") && Time.time >= nextScreamTime)
        {
            PerformScream();
            nextScreamTime = Time.time + controller.GetScreamCooldown();
        }
    }

    private void PerformAttack(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.SoulSword:
                PerformSoulSwordAttack();
                break;
            case WeaponType.None:
                PerformSwipeAttack();
                break;
        }
    }
    private void PerformSwipeAttack()
    {
        Vector3 attackOrigin = transform.position + transform.forward * 1f;
        Collider[] hits = Physics.OverlapSphere(attackOrigin, attackRange, attackMask);

        foreach (Collider hit in hits)
        {
            EnemyBaseAI enemy = hit.GetComponent<EnemyBaseAI>();
            if (enemy == null) continue;

            enemy.health -= controller.GetSwipeDamage();

            Vector3 knockDir = (hit.transform.position - transform.position).normalized;
            enemy.ApplyKnockback(knockDir * knockbackForce);
        }

        AudioHelper.PlaySound(attackClip,audioSource);
        OnAttack?.Invoke();
        Debug.Log($"{name} performed a basic swipe attack!");
    }
    private void PerformSoulSwordAttack()
    {
        Vector3 attackOrigin = transform.position + transform.forward * 1.5f;
        Collider[] hits = Physics.OverlapSphere(attackOrigin, soulSwordRange, attackMask);

        foreach (Collider hit in hits)
        {
            EnemyBaseAI enemy = hit.GetComponent<EnemyBaseAI>();
            if (enemy == null) continue;

            float dmg = controller.GetSwipeDamage() * soulSwordDamageMultiplier;
            enemy.health -= dmg;

            Vector3 knockDir = (hit.transform.position - transform.position).normalized;
            enemy.ApplyKnockback(knockDir * knockbackForce * soulSwordKnockbackMultiplier);
        }

        AudioHelper.PlaySound(soulSwordSound,audioSource);
        OnAttack?.Invoke();
        Debug.Log($"{name} unleashed a Soul Sword attack!");
    }
    private void PerformScream()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, screamRadius, attackMask);

        foreach (Collider hit in hits)
        {
            EnemyBaseAI enemyAI = hit.GetComponent<EnemyBaseAI>();
            if (enemyAI == null) continue;

            int playerScreamLevel = controller.GetScreamLevel();

            if (playerScreamLevel >= enemyAI.fearThreshold)
            {
                enemyAI.Scare(3f, transform.position);
                Debug.Log($"{enemyAI.name} scared by scream!");
            }
            else
            {
                enemyAI.Stun(1f);
                Debug.Log($"{enemyAI.name} stunned by scream!");
            }
        }

        AudioHelper.PlaySound(screamClip, audioSource);
        OnScream?.Invoke();
        Debug.Log($"{name} screamed!");
    }

    private void OnDrawGizmosSelected()
    {
        // Attack
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, attackRange);

        // Soul sword
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1.5f, soulSwordRange);

        // Scream
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f);
        Gizmos.DrawSphere(transform.position, screamRadius);
    }

    public void checkAliveEnemies() {
		if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0) {

		}
	}
}
