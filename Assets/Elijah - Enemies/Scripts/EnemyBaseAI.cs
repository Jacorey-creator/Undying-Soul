using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum State {
	IDLE,
	PATROL,
	CHASE,
	ATTACK,
	DEATH,
	DASH,
	THROW,
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBaseAI : MonoBehaviour {
	private NavMeshAgent agent;
	private Transform target;
	[SerializeField] private State curState;
	[SerializeField] private State[] states;

	[Header("References")]
	public Animator animator;
	public Vector3[] patrolPoints;
	[SerializeField] GameObject hurtbox;

	[Header("Settings")]
	public float idleDuration = 2f;
	public float detectionRange = 10f;
	public float attackRange = 2f;
	public float attackCooldown = 1.5f;

	private int currentPatrolIndex;
	private float idleTimer;
	private float attackTimer;

	public float health = 10f;

	[SerializeField] private Vector3 knockbackDisplacement;
	[SerializeField] private float knockbackDecay = 10f; // how fast knockback fades

	[SerializeField] public int fearThreshold = 1; // higher = braver enemies

	private bool isStunned = false;
	private float stunTimer = 0f;

	private bool isScared = false;
	private float scaredTimer = 0f;
	private Vector3 scaredDirection;

	void Awake() {
		agent = GetComponent<NavMeshAgent>();
	}

	void Start() {
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
			target = player.transform;
		
		hurtbox.SetActive(false);

		SwitchState(states[0]);
	}

	void Update() {
		if (health <= 0) SwitchState(states[states.Length - 1]);

		switch (curState) {
			case State.IDLE:
				idleUpdate();
				break;
			case State.PATROL:
				patrolUpdate();
				break;
			case State.CHASE:
				chaseUpdate();
				break;
			case State.ATTACK:
				attackUpdate();
				break;
			case State.DEATH:
				deathUpdate();
				break;
			case State.DASH:
				dashUpdate();
				break;
			case State.THROW:
				throwUpdate();
				break;
		}
	}

	IEnumerator SwitchState(State newState) {
		curState = newState;

		switch (curState) {
			case State.IDLE:
				// play animation then patrol or chase
				animator.SetTrigger("DoIdle");
				yield return new WaitForSeconds(3f);
				if (Vector3.Distance(transform.position, target.position) < detectionRange) {
					SwitchState(State.CHASE);
				} else {
					SwitchState(State.PATROL);
				}
				break;
			case State.PATROL:
				target.position = patrolPoints[Random.Range(0, patrolPoints.Length)];
				break;
			case State.CHASE:
				GameObject p = GameObject.FindWithTag("Player");
				if (p != null)
					target = p.transform;
				break;
			case State.ATTACK:
				break;
			case State.DEATH:
				playAnim(3f, "DoDeath");
				Destroy(gameObject);
				break;
			case State.DASH:
				break;
		}

		yield return new WaitForSeconds(0.1f);
	}

	IEnumerator playAnim(float time, string a) {
		animator.SetTrigger(a);
		yield return new WaitForSeconds(time);
	}

	void idleUpdate() {

	}

	void patrolUpdate() {
		if (Vector3.Distance(transform.position, target.position) < 1) {
			SwitchState(states[0]);
			return;
		}

		if (Vector3.Distance(transform.position, target.position) < detectionRange) {
			SwitchState(State.CHASE);
			return;
		}
	}

	void chaseUpdate() {
		if (target != null) agent.SetDestination(target.position);

		if (Vector3.Distance(transform.position, transform.position) <= attackRange) SwitchState(State.ATTACK);
	}

	void attackUpdate() {
		transform.LookAt(target);

		attackTimer -= Time.deltaTime;

		if (Vector3.Distance(transform.position, target.position) > attackRange + 0.5f) {
			SwitchState(State.CHASE);
			return;
		}

		if (attackTimer <= 0f) {
			animator.SetTrigger("DoAttack");
			//playAnim(attackCooldown);
			attackTimer = attackCooldown;
		}
	}

	void deathUpdate() {
	
	}

	void dashUpdate() {
	
	}

	void throwUpdate() {
	
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