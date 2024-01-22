using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	public sealed class ThirdPersonRigid : ThirdPersonBase
	{
		CapsuleCollider _capsuleCollider;
		Rigidbody _rigidbody;
		public LayerMask jumpRaycastLayers;

		protected override void Awake()
		{
			base.Awake();
			_capsuleCollider = GetComponent<CapsuleCollider>();
			_rigidbody = GetComponent<Rigidbody>();

			if (GetComponent<CharacterController>() != null)
				Debug.LogWarning("You do not needed to attach 'CharacterController' to controller with 'Rigidbody'");
		}

		public override void CharacterEnable(bool enable)
		{
			base.CharacterEnable(enable);
			_capsuleCollider.enabled = enable;
			_rigidbody.isKinematic = !enable;
			if (enable)
				_firstAnimatorFrame = true;
		}

		protected override Vector3 PlayerVelocity { get { return _rigidbody.velocity; } }

		protected override void ApplyCapsuleHeight()
		{
			float capsuleY = _animator.GetFloat(_animatorCapsuleY);
			_capsuleCollider.height = capsuleY;
			var c = _capsuleCollider.center;
			c.y = capsuleY / 2f;
			_capsuleCollider.center = c;
		}

#region Ground Check

		/// <summary>
		/// every FixedUpdate _groundChecker resets to false,
		/// and if collision with ground found till next FixedUpdate,
		/// the character on a ground
		/// </summary>
		[HideInInspector] public bool _groundChecker;
		float _jumpStartedTime;

		void ProccessOnCollisionOccured(Collision collision)
		{
			// if collision comes from botton, that means
			// that character on the ground
			float charBottom =
				transform.position.y +
				_capsuleCollider.center.y - _capsuleCollider.height / 2 +
				_capsuleCollider.radius * 0.8f;
			
			foreach (ContactPoint contact in collision.contacts)
			{
				if (contact.point.y < charBottom && !contact.otherCollider.transform.IsChildOf(transform))
				{
					_groundChecker = true;
					Debug.DrawRay(contact.point, contact.normal, Color.blue);
					break;
				}
			}
		}

        void OnCollisionStay(Collision collision)
        {
            ProccessOnCollisionOccured(collision);
		}
		
		void OnCollisionEnter(Collision collision)
		{
			ProccessOnCollisionOccured(collision);
        }

        void Update()
        {
            //Implement
        }

        void OnCollisionExit(Collision collision)
        {
			//Implement
        }

        protected override bool PlayerTouchGound()
		{
            bool grounded = _groundChecker;

            // Perform a raycast to check for ground at a distance of 0.3f below the character
            RaycastHit hit;
            bool hitGround = Physics.Raycast(transform.position, Vector3.down, out hit, 0.3f, jumpRaycastLayers);

            // Set _groundChecker to false only if the raycast does not hit anything
            _groundChecker = hitGround;

            // if the character is on the ground and
            // half of second was passed, return true
            return grounded & (_jumpStartedTime + 0.5f < Time.time );
		}

#endregion
		protected override void UpdatePlayerPosition(Vector3 deltaPos)
		{
			Vector3 finalVelocity = deltaPos / Time.deltaTime;
			if (!_jumpPressed)
			{
				finalVelocity.y = _rigidbody.velocity.y;
			}
			else
			{
				_jumpStartedTime = Time.time;
			}
			_airVelocity = finalVelocity;		// i need this to correctly detect player velocity in air mode
			_rigidbody.velocity = finalVelocity;
		}
	}
}