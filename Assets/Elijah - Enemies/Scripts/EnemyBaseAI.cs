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
	private Vector3 targetPos;
	[SerializeField] private State curState;
	[SerializeField] private State[] states;

	[Header("References")]
	public Animator animator;
	[SerializeField] GameObject hurtbox;

	[Header("Settings")]
	public float idleDuration = 2f;
	public float detectionRange = 10f;
	public float attackRange = 2f;
	public float attackCooldown = 1.5f;
	private float attackTimer;

	public float health = 10f;
	[SerializeField] float dmg = 1f;

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
		if (player != null) {
			target = player.transform;
		}

		hurtbox.GetComponent<HurtBox>().damage = dmg;
		hurtbox.SetActive(false);

		SwitchState(State.IDLE);
	}

	void Update() {
		if (health <= 0) SwitchState(State.DEATH);

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

	void SwitchState(State newState) {
		curState = newState;

		switch (curState) {
			case State.IDLE:
				// play animation then patrol or chase
				if (animator != null) animator.SetTrigger("DoIdle");
				StartCoroutine(IdleCoroutine());
				break;
			case State.PATROL:
				Vector3 randomDirection = Random.insideUnitSphere * 5;
				randomDirection += transform.position;

				NavMeshHit hit;

				// Check if the random point is on the NavMesh
				if (NavMesh.SamplePosition(randomDirection, out hit, 5, NavMesh.AllAreas)) {
					targetPos = hit.position;
					agent.SetDestination(targetPos);
				} else {
					// Fallback (if no valid point found)
					targetPos = transform.position;
					agent.SetDestination(targetPos);
				}
				break;
			case State.CHASE:
				GameObject p = GameObject.FindWithTag("Player");
				if (p != null) {
					target = p.transform;
				}
				break;
			case State.ATTACK:
				attackTimer = attackCooldown;
				break;
			case State.DEATH:
				if (animator != null) animator.SetTrigger("DoDeath");
				StartCoroutine(DeathCoroutine());
				break;
			case State.DASH:
				break;
		}
	}

	IEnumerator IdleCoroutine() {
		yield return new WaitForSeconds(3f);
		if (Vector3.Distance(transform.position, target.position) < detectionRange) {
			SwitchState(State.CHASE);
		} else {
			SwitchState(State.PATROL);
		}
	}

	IEnumerator DeathCoroutine() {
		yield return new WaitForSeconds(2f);
		Destroy(gameObject);
	}

	void idleUpdate() {

	}

	void patrolUpdate() {
		if (Vector3.Distance(transform.position, targetPos) < 2.5f) {
			SwitchState(states[0]);
			return;
		}

		if (Vector3.Distance(transform.position, target.position) < detectionRange) {
			SwitchState(State.CHASE);
			return;
		}
	}

	void chaseUpdate() {
		if (target != null) {
			agent.SetDestination(target.position);
		}

		if (Vector3.Distance(transform.position, target.position) > detectionRange) {
			SwitchState(State.IDLE);
			return;
		}

		if (Vector3.Distance(transform.position, target.position) <= attackRange) SwitchState(State.ATTACK);
	}

	void attackUpdate() {
		transform.LookAt(target);

		attackTimer -= Time.deltaTime;

		if (Vector3.Distance(transform.position, target.position) > attackRange + 0.5f) {
			SwitchState(State.CHASE);
			return;
		}

		if (attackTimer <= 0f) {
			if (animator != null) animator.SetTrigger("DoAttack");

			Vector3 attackOrigin = transform.position + transform.forward * attackRange;
			Collider[] hits = Physics.OverlapSphere(attackOrigin, attackRange);

			foreach (Collider hit in hits) {
				PlayerHealthController enemy = hit.GetComponent<PlayerHealthController>();
				if (enemy == null) continue;

				enemy.TakeDamage(dmg);
			}

			attackTimer = attackCooldown;
		}
	}

	void deathUpdate() {
		
	}

	void dashUpdate() {
	
	}

	void throwUpdate() {
	
	}

	public void ApplyKnockback(Vector3 displacement) {
		knockbackDisplacement = displacement; // assign new push vector
	}

	public void Stun(float duration) {
		isStunned = true;
		stunTimer = duration;
		agent.ResetPath(); // stop immediately
	}

	public void Scare(float duration, Vector3 fromPosition) {
		isScared = true;
		scaredTimer = duration;
		scaredDirection = (transform.position - fromPosition).normalized;
	}
}