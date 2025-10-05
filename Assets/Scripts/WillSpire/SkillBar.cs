using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    [SerializeField] private Image[] segments;
    [SerializeField] private Color baseColor = Color.yellow;
    [SerializeField] private Color midColor = new Color(1f, 0f, 1f); // Neon purple
    [SerializeField] private Color maxColor = new Color(0f, 1f, 1f); // Neon turquoise

    private const int LevelsPerColor = 5; // 5 segments per color stage

    public void SetLevel(int level)
    {
        // Determine which color stage we’re in (0 = yellow, 1 = purple, 2 = turquoise)
        int colorStage = Mathf.Clamp(level / LevelsPerColor, 0, 2);

        // Compute how many segments should be filled (cap at 5)
        int filledSegments = level % LevelsPerColor;

        // Fix the “blank bar” issue: when level is exactly multiple of 5,
        // all segments should stay filled with the previous color
        if (filledSegments == 0 && level > 0)
        {
            colorStage = Mathf.Clamp(colorStage - 1, 0, 2);
            filledSegments = LevelsPerColor;
        }

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
