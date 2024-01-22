using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	public interface IThirdPerson
	{
		/// <summary>
		/// The Move function is designed to be called from a separate component
		/// based on User input, or an AI control script
		/// </summary>
		/// <param name="move">Position shift</param>
		/// <param name="crouch">Crouch if True</param>
		/// <param name="jumpPressed">Jump if True</param>
		void Move(Vector3 move, bool crouch, bool jump);
	}
}
