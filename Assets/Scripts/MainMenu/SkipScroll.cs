using UnityEngine;

public class SkipScroll : MonoBehaviour
{
    // Reference to the canvas that contains the scroll
    public GameObject scrollCanvas;

    // Method to call when the button is clicked
    public void OnSkipButtonPressed()
    {
        if (scrollCanvas != null)
        {
            Destroy(scrollCanvas); // Or scrollCanvas.SetActive(false);
        }
    }
}
