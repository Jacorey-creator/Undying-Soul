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
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBaseAI : MonoBehaviour {
	private NavMeshAgent agent;
	private Transform target;
	[SerializeField] private State curState;

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

	void Awake() {
		agent = GetComponent<NavMeshAgent>();
	}

	void Start() {
		switchState(State.IDLE);

		//GameObject player = GameObject.FindWithTag("Player");
		//if (player != null)
		//	target = player.transform;
	}

	void Update() {
		//if (target != null) agent.SetDestination(target.position);

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
		}
	}

	void switchState(State newState) {
		curState = newState;

		switch (curState) {
			case State.IDLE:
				// play animation then patrol or chase
				playAnim(3f);
				if (Vector3.Distance(transform.position, target.position) < detectionRange) {

				} else { 
				
				}
				break;
			case State.PATROL:
				break;
			case State.CHASE:
				GameObject p = GameObject.FindWithTag("Player");
				if (p != null)
					target = p.transform;
				break;
			case State.ATTACK:
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
}