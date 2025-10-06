using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class PlayerSpriteDirectionController : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        // Example: trigger attack when left mouse button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            TriggerAttack();
        }
    }

    /// <summary>
    /// Call this every frame with the player's movement input.
    /// </summary>
    public void UpdateDirection(Vector3 input)
    {
        // Convert input to isometric plane
        Vector3 moveDir = input.ToIso();
        moveDir.y = 0;

        float speed = moveDir.magnitude;

        // Update animator speed parameter
        animator.SetFloat("Speed", speed);

        if (speed > 0.01f)
        {
            // Normalize for direction parameters
            Vector3 normalized = moveDir.normalized;
            animator.SetFloat("MoveX", normalized.x);
            animator.SetFloat("MoveY", normalized.z);
        }

        // Ensure sprite faces the camera
        if (cameraTransform != null)
            spriteRenderer.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
    }

    /// <summary>
    /// Triggers the attack animation.
    /// </summary>
    public void TriggerAttack()
    {
        animator.SetTrigger("Attack");
    }
}
