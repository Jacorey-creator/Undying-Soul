using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    [SerializeField] private Image[] segments;
    [SerializeField] private Color baseColor = Color.yellow;
    [SerializeField] private Color midColor = new Color(1f, 0f, 1f); // Neon purple
    [SerializeField] private Color maxColor = new Color(0f, 1f, 1f); // Neon turquoise

    private int maxDisplayedLevel = 15; // visual cap (5 per color set)

    public void SetLevel(int level)
    {
        // Normalize the display to cycle every 5 levels per color group
        int colorStage = Mathf.Clamp(level / 5, 0, 2);
        int filledSegments = Mathf.Clamp(level % 5, 0, 5);

        Color currentColor = colorStage switch
        {
            0 => baseColor, // 0–4 ? yellow
            1 => midColor,  // 5–9 ? purple
            2 => maxColor,  // 10–14 ? turquoise
            _ => maxColor
        };

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].color = (i < filledSegments) ? currentColor : Color.gray;
        }
    }
}
