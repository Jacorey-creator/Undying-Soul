using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerSpriteDirectionController : MonoBehaviour
{
    [Header("Sprites (8 Directions)")]
    public Sprite up;
    public Sprite upRight;
    public Sprite right;
    public Sprite downRight;
    public Sprite down;
    public Sprite downLeft;
    public Sprite left;
    public Sprite upLeft;

    [Header("Camera")]
    public Transform cameraTransform;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// Call this every frame with raw input vector
    /// </summary>
    public void UpdateDirection(Vector3 input)
    {
        // If no movement, skip updating sprite
        if (input.sqrMagnitude < 0.01f) return;

        // Convert input to isometric plane
        Vector3 moveDir = input.ToIso();
        moveDir.y = 0; // flatten to XZ plane

        // Calculate angle relative to world forward (Z)
        float angle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        // Pick sprite based on angle (8 directions, 45 degrees each)
        Sprite chosenSprite = up;

        if (angle >= 337.5f || angle < 22.5f) chosenSprite = up;
        else if (angle >= 22.5f && angle < 67.5f) chosenSprite = upRight;
        else if (angle >= 67.5f && angle < 112.5f) chosenSprite = right;
        else if (angle >= 112.5f && angle < 157.5f) chosenSprite = downRight;
        else if (angle >= 157.5f && angle < 202.5f) chosenSprite = down;
        else if (angle >= 202.5f && angle < 247.5f) chosenSprite = downLeft;
        else if (angle >= 247.5f && angle < 292.5f) chosenSprite = left;
        else if (angle >= 292.5f && angle < 337.5f) chosenSprite = upLeft;

        spriteRenderer.sprite = chosenSprite;

        // Make sprite face camera
        if (cameraTransform != null)
            spriteRenderer.transform.rotation = Quaternion.LookRotation(cameraTransform.forward, Vector3.up);
    }
}
