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

    [SerializeField] public int fearThreshold = 1; // higher = braver enemies

    private bool isStunned = false;
    private float stunTimer = 0f;

    private bool isScared = false;
    private float scaredTimer = 0f;
    private Vector3 scaredDirection;


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
        if (health <= 0)
        {
            Destroy(gameObject); 
            return;
        }
        
        // Handle stun
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f) isStunned = false;
            return; // skip movement while stunned
        }

        // Apply knockback displacement each frame
        if (knockbackDisplacement != Vector3.zero)
        {
            agent.Move(knockbackDisplacement * Time.deltaTime);
            knockbackDisplacement = Vector3.Lerp(knockbackDisplacement, Vector3.zero, knockbackDecay * Time.deltaTime);
        }

        // Handle scared
        if (isScared)
        {
            scaredTimer -= Time.deltaTime;
            if (scaredTimer > 0f)
            {
                agent.SetDestination(transform.position + scaredDirection * 5f); // run away
                return;
            }
            else isScared = false;
        }
        //Chase
        if (target != null)
        {
            agent.SetDestination(target.position);
        }
    }
    public void ApplyKnockback(Vector3 displacement)
    {
        knockbackDisplacement = displacement; // assign new push vector
    }
    public void Stun(float duration)
    {
        isStunned = true;
        stunTimer = duration;
        agent.ResetPath(); // stop immediately
    }

    public void Scare(float duration, Vector3 fromPosition)
    {
        isScared = true;
        scaredTimer = duration;
        scaredDirection = (transform.position - fromPosition).normalized;
    }
}
