using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackController : MonoBehaviour
{
    private PlayerController controller;
    private float nextAttackTime = 0f;
    private float nextScreamTime = 0f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float knockbackForce = 5f;

    [SerializeField] private float screamRadius = 5f;
    
    [SerializeField] private LayerMask attackMask;

    // Event: Invoked whenever the player attacks
    public event Action OnAttack;
    public event Action OnScream;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!controller.IsActiveController) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            PerformSwipeAttack();
            nextAttackTime = Time.time + controller.GetAttackSpeed();
        }
        // Scream
        if (Input.GetButtonDown("Fire2") && Time.time >= nextScreamTime)
        {
            PerformScream();
            nextScreamTime = Time.time + controller.GetScreamCooldown();
        }
    }

    private void PerformSwipeAttack()
    {
        // Define attack direction (forward from player)
        Vector3 attackOrigin = transform.position + transform.forward * 1f;

        // Check for enemies in range
        Collider[] hits = Physics.OverlapSphere(attackOrigin, attackRange, attackMask);

        foreach (Collider hit in hits)
        {
            //Apply damage(uncomment when you have enemy health hooked up)
            float enemyHealth = hit.GetComponent<EnemyAI>().health;
            if (enemyHealth != 0)
            {
                hit.GetComponent<EnemyAI>().health -= controller.GetSwipeDamage();
            }

            // Apply knockback
            EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                Vector3 knockDir = (hit.transform.position - transform.position).normalized;
                enemyAI.ApplyKnockback(knockDir * knockbackForce);
            }
        }

        Debug.Log($"{name} performed swipe attack!");

        // Fire the event for polish (VFX, SFX, animation, camera shake, etc.)
        OnAttack?.Invoke();
    }

    private void PerformScream()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, screamRadius, attackMask);

        foreach (Collider hit in hits)
        {
            EnemyAI enemyAI = hit.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                int playerScreamLevel = controller.GetScreamLevel();

                if (playerScreamLevel >= enemyAI.fearThreshold)
                {
                    // Enemy is scared away
                    enemyAI.Scare(3f, transform.position);
                    Debug.Log($"{enemyAI.name} scared by scream!");
                }
                else
                {
                    // Enemy is stunned instead
                    enemyAI.Stun(1f);
                    Debug.Log($"{enemyAI.name} stunned by scream!");
                }
            }
        }

        Debug.Log($"{name} screamed!");
        OnScream?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        // Visualize attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, attackRange);

        Gizmos.color = new Color(1f, 0.5f, 0f, 0.35f); // orange & transparent
        Gizmos.DrawSphere(transform.position, screamRadius);
    }
}
