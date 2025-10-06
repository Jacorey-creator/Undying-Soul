using UnityEngine;

public class ScrollText : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public float destroyY = 1016.06f; // Y position at which the text gets destroyed
    [SerializeField] GameObject scrollText;


    void Update()
    {
        // Move text up along its local Y axis
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);

        // Check if the text has reached or passed the destroy position
        if (transform.position.y >= destroyY)
        {
            Destroy(scrollText);
        }
    }
}
