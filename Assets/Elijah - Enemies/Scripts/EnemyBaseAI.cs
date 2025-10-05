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
	public Transform[] patrolPoints;

	[Header("Settings")]
	public float idleDuration = 2f;
	public float detectionRange = 10f;
	public float attackRange = 2f;
	public float attackCooldown = 1.5f;

	private int currentPatrolIndex;
	private float idleTimer;
	private float attackTimer;

	public float health = 10f;

	void Awake() {
		agent = GetComponent<NavMeshAgent>();
	}

	void Start() {
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
			target = player.transform;
		
		switchState(states[0]);
	}

	void Update() {
		if (health <= 0) switchState(states[states.Length - 1]);

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

	void switchState(State newState) {
		curState = newState;

		switch (curState) {
			case State.IDLE:
				// play animation then patrol or chase
				playAnim(3f);
				if (Vector3.Distance(transform.position, target.position) < detectionRange) {
					switchState(State.CHASE);
				} else {
					switchState(State.PATROL);
				}
				break;
			case State.PATROL:
				target = patrolPoints[Random.Range(0, patrolPoints.Length)];
				break;
			case State.CHASE:
				GameObject p = GameObject.FindWithTag("Player");
				if (p != null)
					target = p.transform;
				break;
			case State.ATTACK:
				playAnim(3f);
				break;
			case State.DEATH:

				break;
			case State.DASH:
				break;
		}
	}

	IEnumerator playAnim(float time) {
		yield return new WaitForSeconds(time);
	}

	void idleUpdate() {

	}

	void patrolUpdate() {
		if (Vector3.Distance(transform.position, target.position) < 1) {
			switchState(states[0]);
			target = patrolPoints[Random.Range(0, patrolPoints.Length)];
			return;
		}
	}

	void chaseUpdate() {
		if (target != null) agent.SetDestination(target.position);

		if (Vector3.Distance(transform.position, transform.position) <= attackRange) switchState(State.ATTACK);
	}

	void attackUpdate() {
		transform.LookAt(target);

		attackTimer -= Time.deltaTime;

		if (Vector3.Distance(transform.position, target.position) > attackRange + 0.5f) {
			switchState(State.CHASE);
			return;
		}

		if (attackTimer <= 0f) {
			animator.SetTrigger("DoAttack");
			playAnim(attackCooldown);
		}
	}

	void deathUpdate() {
	
	}

	void dashUpdate() {
	
	}

	void throwUpdate() {
	
	}
}