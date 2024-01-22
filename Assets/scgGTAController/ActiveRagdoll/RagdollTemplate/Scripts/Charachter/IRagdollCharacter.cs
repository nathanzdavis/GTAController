using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IRagdollCharacter
	{
		/// <summary>
		/// Character current velocity.
		/// </summary>
		Vector3 CharacterVelocity { get; }

		/// <summary>
		/// Turn off controller script
		/// </summary>
		/// <param name="enable">Turn off if False</param>
		void CharacterEnable(bool enable);
	}
}