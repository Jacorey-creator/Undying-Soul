using UnityEngine;

public class HurtBox : MonoBehaviour {
	public float damage = 10;

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player")) {
			var health = other.GetComponent<PlayerHealthController>();
			if (health != null)
			{
				health.TakeDamage(damage);
				gameObject.SetActive(false);
			}
		}
	}
}
