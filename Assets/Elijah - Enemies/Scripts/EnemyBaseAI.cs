using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseAI : MonoBehaviour {
	private NavMeshAgent agent;
	private Transform target;

	void Awake() {
		agent = GetComponent<NavMeshAgent>();
	}

	void Start() {
		// Find the player/ghost in the scene
		GameObject player = GameObject.FindWithTag("Player");
		if (player != null)
			target = player.transform;
	}

	void Update() {
		if (target != null) {
			agent.SetDestination(target.position);
		}
	}
}
