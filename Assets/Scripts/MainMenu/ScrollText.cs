using UnityEngine;
using System.Collections;

public class ScrollText : MonoBehaviour
{
    public float scrollSpeed = 2f;
    public float destroyTime = 100f; // 2 minutes (in seconds)
    [SerializeField] GameObject scrollText;

    void Start()
    {
        // Start the coroutine to destroy after 2 minutes
        StartCoroutine(DestroyAfterTime());
    }

    void Update()
    {
        // Move text up along its local Y axis
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime);
        Destroy(scrollText);
    }
}
