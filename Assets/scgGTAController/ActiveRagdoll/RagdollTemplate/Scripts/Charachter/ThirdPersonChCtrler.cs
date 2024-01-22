using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(CharacterController))]
	public sealed class ThirdPersonChCtrler : ThirdPersonBase
	{
		CharacterController _characterController;

		protected override void Awake()
		{
			base.Awake();
			_characterController = GetComponent<CharacterController>();

			if (GetComponent<CapsuleCollider>() != null)
				Debug.LogWarning("You do not needed to attach 'CapsuleCollider' to controller with 'CharacterController'");
			if (GetComponent<Rigidbody>() != null)
				Debug.LogWarning("You do not needed to attach 'rigidbody' to controller with 'CharacterController'");
		}

		public override void CharacterEnable(bool enable)
		{
			base.CharacterEnable(enable);
			_characterController.enabled = enable;
			if (enable)
				_firstAnimatorFrame = true;
		}

		protected override Vector3 PlayerVelocity { get { return _characterController.velocity; } }

		protected override void ApplyCapsuleHeight()
		{
			float capsuleY = _animator.GetFloat(_animatorCapsuleY);
			_characterController.height = capsuleY;
			var c = _characterController.center;
			c.y = capsuleY / 2f;
			c.y += 0.03f; // plus skin wight
			_characterController.center = c;
		}

		protected override bool PlayerTouchGound()
		{
			return _characterController.isGrounded;
		}

		protected override void UpdatePlayerPosition(Vector3 deltaPos)
		{
			if (_characterController.enabled)
			{
				_characterController.Move(deltaPos);

				if (!_characterController.isGrounded)
					return;
			}

			_airVelocity = Vector3.zero;
		}
	}
}