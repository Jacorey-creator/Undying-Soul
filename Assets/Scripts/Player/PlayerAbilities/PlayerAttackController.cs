using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
public class PlayerAttackController : MonoBehaviour
{
    private PlayerController controller;
    private float nextAttackTime = 0f;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private LayerMask attackMask;

    // Event: Invoked whenever the player attacks
    public event Action OnAttack;

    void Awake()
    {
        controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!controller.IsActiveController) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + controller.GetAttackSpeed();
        }
    }

    private void PerformAttack()
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

    void OnDrawGizmosSelected()
    {
        // Visualize attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, attackRange);
    }
}
