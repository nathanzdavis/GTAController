using KovSoft.RagdollTemplate.Scripts.Charachter;
using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Tools
{
	/// <summary>
	/// Script have to be attached to Health Items
	/// </summary>
	public sealed class HealthItem : MonoBehaviour
	{
		/// <summary>
		/// The amount of health in item
		/// </summary>
		[SerializeField]
		float _addHealth = 0.25f;

		// Use this for initialization
		void OnTriggerEnter(Collider collider)
		{
			IDamageable damageable = collider.GetComponent<IDamageable>();
			if (damageable == null)
				return;

			if (damageable.IsFullHealth())
				return;

			damageable.Health += _addHealth;
			Destroy(gameObject);
		}
	}
}