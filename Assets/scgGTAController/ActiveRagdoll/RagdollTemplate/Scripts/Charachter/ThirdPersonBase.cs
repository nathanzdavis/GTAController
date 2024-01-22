using System;
using UnityEngine;
using scgGTAController;
using System.Collections;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(Animator))]
	public abstract class ThirdPersonBase : MonoBehaviour, IRagdollCharacter, IThirdPerson
	{
		// animator parameters:
		readonly int _animatorForward = Animator.StringToHash("Forward");
		readonly int _animatorTurn = Animator.StringToHash("Turn");
		readonly int _animatorCrouch = Animator.StringToHash("Crouch");
		readonly int _animatorOnGround = Animator.StringToHash("OnGround");
		readonly int _animatorJump = Animator.StringToHash("Jump");
		readonly int _animatorJumpLeg = Animator.StringToHash("JumpLeg");
		protected readonly int _animatorCapsuleY = Animator.StringToHash("CapsuleY");

		// animator animations:
		readonly int _animatorGrounded = Animator.StringToHash("Base Layer.Grounded.Grounded");

		// constants:
		const float JumpPower = 5f;		// determines the jump force applied when jumping (and therefore the jump height)
		const float AirSpeed = 5f;		// determines the max speed of the character while airborne
		const float AirControl = 2f;	// determines the response speed of controlling the character while airborne
		const float StationaryTurnSpeed = 180f;	// additional turn speed added when the player is stationary (added to animation root rotation)
		const float MovingTurnSpeed = 360f;		// additional turn speed added when the player is moving (added to animation root rotation)
		const float RunCycleLegOffset = 0.2f;	// animation cycle offset (0-1) used for determining correct leg to jump off

		protected Animator _animator; // The animator for the character

		// parameters needed to control character
		bool _onGround; // Is the character on the ground
		Vector3 _moveInput;
		bool _crouch;
		bool _jump;
		bool jumping;
		float _turnAmount;
		float _forwardAmount;
		bool _enabled = true;
		protected Vector3 _airVelocity;
		protected bool _jumpPressed = false;
		protected bool _firstAnimatorFrame = true;  // needed for prevent changing position in first animation frame
        public bool moving;
        protected virtual void Awake()
		{
			_animator = GetComponent<Animator>();
        }

		#region IRagdollCharacter
		public Vector3 CharacterVelocity { get { return _onGround ? PlayerVelocity : _airVelocity; } }

		public virtual void CharacterEnable(bool enable)
		{
			_enabled = enable;
		}
		#endregion

		#region IThirdPerson
		public void Move(Vector3 move, bool crouch, bool jump)
		{
			_moveInput = move;
			_crouch = crouch;
			_jump = jump;
		}
		#endregion

		/// <summary>
		/// current character velocity
		/// </summary>
		protected abstract Vector3 PlayerVelocity { get; }
		/// <summary>
		/// Player is grounded
		/// </summary>
		protected abstract bool PlayerTouchGound();
		/// <summary>
		/// Set capsule height. You need scale capsule when character crouch
		/// </summary>
		protected abstract void ApplyCapsuleHeight();
		/// <summary>
		/// Move character by specified delta vector
		/// </summary>
		protected abstract void UpdatePlayerPosition(Vector3 deltaPos);

		private void HandleGroundedVelocities(int currentAnimation)
		{
			bool animationGrounded = currentAnimation == _animatorGrounded;

			// check whether conditions are right to allow a jump
			if (!(_jump & !_crouch & animationGrounded))
				return;

			// jump!
			Vector3 newVelocity = CharacterVelocity;
			newVelocity.y += JumpPower;
			_airVelocity = newVelocity;

			_jump = false;
			_onGround = false;
			_jumpPressed = true;
		}

		private void UpdateAnimator()
		{
			// Here we tell the animator what to do based on the current states and inputs.

			// update the animator parameters
			_animator.SetFloat(_animatorForward, _forwardAmount, 0.1f, Time.deltaTime);
			_animator.SetFloat(_animatorTurn, _turnAmount, 0.1f, Time.deltaTime);
			_animator.SetBool(_animatorOnGround, _onGround);
			if (!_onGround)	// if flying
			{
				_animator.SetFloat(_animatorJump, CharacterVelocity.y);
			}
			else
			{
				// calculate which leg is behind, so as to leave that leg trailing in the jump animation
				// (This code is reliant on the specific run cycle offset in our animations,
				// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
				float runCycle = Mathf.Repeat(
						_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + RunCycleLegOffset, 1);

				float jumpLeg = (runCycle < 0.5f ? 1 : -1) * _forwardAmount;
				_animator.SetFloat(_animatorJumpLeg, jumpLeg);
			}
		}
		/// <summary>
		/// Prepare input movement data for animator
		/// </summary>
		private void ConvertMoveInput()
		{
			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction. 
			Vector3 localMove = transform.InverseTransformDirection(_moveInput);
            int currentAnimation = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            if ((Math.Abs(localMove.x) > float.Epsilon) &
                (Math.Abs(localMove.z) > float.Epsilon))
            {
                _turnAmount = Mathf.Atan2(localMove.x, localMove.z);
                ApplyExtraTurnRotation(currentAnimation);		// this is in addition to root rotation in the animations
                moving = true;
            }
            else
            {
                GunController[] gc = GetComponentsInChildren<GunController>();
                if (GetComponentInChildren<GunController>()){
                    foreach (GunController gunc in gc)
                    {
                        if (gunc.aiming || gunc.firing)
                        {
                            _turnAmount = Input.GetAxis("Mouse X");
                            transform.Rotate(0, _turnAmount, 0);
                        }
                        else
                        {
                            _turnAmount = 0f;
                        }
                    }
                }
                else
                {
                    _turnAmount = 0f;
                }
                moving = false;
            }

            _forwardAmount = localMove.z;
		}

        public void setTurnAmount(int amount)
        {
            _turnAmount = amount;
        }

		/// <summary>
		/// Animation rotates very slow, so adding a little rotation will be very helpful
		/// </summary>
		private void ApplyExtraTurnRotation(int currentAnimation)
		{
			if (currentAnimation != _animatorGrounded)
				return;
            
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed,
										 _forwardAmount);
            transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);

        }

		private void HandleAirborneVelocities()
		{
			// we allow some movement in air, but it's very different to when on ground
			// (typically allowing a small change in trajectory)
			Vector3 airMove = new Vector3(_moveInput.x * AirSpeed, _airVelocity.y, _moveInput.z * AirSpeed);
			_airVelocity = Vector3.Lerp(_airVelocity, airMove, Time.deltaTime * AirControl);
		}

        IEnumerator CheckLandingDelay()
        {
            // Introduce a delay of 0.5 seconds
            yield return new WaitForSeconds(0.5f);

            // Wait until the character is on the ground
            yield return new WaitUntil(() => _onGround);

            // Check if the character has landed on the ground
            if (jumping && GetComponent<ThirdPersonControl>().state != "InCar")
            {
                GetComponent<IKFeet>().enabled = true; // Enable IKFeet component
                jumping = false;
            }
        }

        void FixedUpdate()
		{
            // Check if the character is in the air and the jump button is pressed
            if (Input.GetButtonDown("Jump"))
            {
                GetComponent<IKFeet>().enabled = false;
                jumping = true;

                // Check if the character has landed on the ground after a delay
                StartCoroutine(CheckLandingDelay());
            }


            if (!_enabled)
				return;

			_onGround = !_jumpPressed && PlayerTouchGound();
			_animator.SetBool(_animatorCrouch, _crouch);
			int currentAnimation = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

			ApplyCapsuleHeight();
			ConvertMoveInput();				// converts the relative move vector into local turn & fwd values

			// control and velocity handling is different when grounded and airborne:
			if (_onGround)
			{
                HandleGroundedVelocities(currentAnimation);
            }
			else
				HandleAirborneVelocities();

			UpdateAnimator(); // send input and other state parameters to the animator
		}

		void OnAnimatorMove()
		{
			if (Time.deltaTime < Mathf.Epsilon)
				return;

			Vector3 deltaPos;
			Vector3 deltaGravity = Physics.gravity * Time.deltaTime;
			_airVelocity += deltaGravity;

			if (_onGround)
			{
				deltaPos = _animator.deltaPosition;
				deltaPos.y -= 5f * Time.deltaTime;
			}
			else
			{
				deltaPos = _airVelocity * Time.deltaTime;
			}

			if (_firstAnimatorFrame)
			{
				// if Animator just started, Animator move character
				// so you need to zeroing movement
				deltaPos = new Vector3(0f, deltaPos.y, 0f);
				_firstAnimatorFrame = false;
			}

			UpdatePlayerPosition(deltaPos);

			// apply animator rotation
			transform.rotation *= _animator.deltaRotation;
			_jumpPressed = false;
		}
	}
}
