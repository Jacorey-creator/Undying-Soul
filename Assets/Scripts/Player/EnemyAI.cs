using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform target;
    public float health = 10f;

    //Will need to be added to enemies
    [SerializeField]  private Vector3 knockbackDisplacement;
    [SerializeField]  private float knockbackDecay = 10f; // how fast knockback fades

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Find the player/ghost in the scene
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
            target = player.transform;
    }

    void Update()
    {
        if (health <= 0) Destroy(gameObject);
        
        // Apply knockback displacement each frame
        if (knockbackDisplacement != Vector3.zero)
        {
            agent.Move(knockbackDisplacement * Time.deltaTime);
            knockbackDisplacement = Vector3.Lerp(knockbackDisplacement, Vector3.zero, knockbackDecay * Time.deltaTime);
        }
        
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    public void ApplyKnockback(Vector3 displacement)
    {
        knockbackDisplacement = displacement; // assign new push vector
    }
}
