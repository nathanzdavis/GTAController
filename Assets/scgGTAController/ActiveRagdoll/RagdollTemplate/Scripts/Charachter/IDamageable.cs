using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IDamageable
	{
		/// <summary>
		/// Gets or sets the health. If Value is 0, guy is dead, if 1 - fully health
		/// </summary>
		float Health { get; set; }
		/// <summary>
		/// Shot the specified ray, force and distance.
		/// </summary>
		/// <param name="impact">How much have to be subtracted from health.</param>
		void Shot(Ray ray, float impact, float maxDistance);
		/// <summary>
		/// Returns true if Health is 0.
		/// </summary>
		bool IsDead();
		/// <summary>
		/// Returns true if Health is 1.
		/// </summary>
		bool IsFullHealth();
	}
}

