using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IRagdoll
	{
		/// <summary>
		/// Check if you hit character. Works as Physics.Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
		/// </summary>
		/// <returns>True if hit occured</returns>
		bool Raycast(Ray ray, out RaycastHit hit, float distance);
		/// <summary>
		/// Adding extra move
		/// </summary>
		/// <param name="move">direction and magnitude</param>
		void AddExtraMove(Vector3 move);
		/// <summary>
		/// If character is ragdolled, the value is True. Otherwise False
		/// </summary>
		bool IsRagdolled { get; set; }
	}
}

