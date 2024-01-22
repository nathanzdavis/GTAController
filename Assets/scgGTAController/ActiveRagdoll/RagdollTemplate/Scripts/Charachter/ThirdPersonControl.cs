using GTAWeaponWheel.Scripts;
using scgGTAController;
using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IThirdPerson))]
	public sealed class ThirdPersonControl : MonoBehaviour
    {
        [Header("Locomotion")]
        public float walkSpeed;
        public float jogSpeed;
        public float runSpeed;
        public bool sprintPressed;
        private IThirdPerson _character;
		private IRagdoll _ragdoll;
		private IDamageable _health;
		private bool _jumpPressed;
		private bool _crouch;
		public float sprintAmount;
        public float sprintDecrement;

        [Header("Punching")]
		public MeleeHitSensing punchSensing; 

        [Header("Camera")]
        private Transform _camTransform;

		[Header("States")]
		public string state;

        private void Start()
		{
			if (Camera.main == null)
				Debug.LogError("Error: no main camera found.");
			else
				_camTransform = Camera.main.transform;

			_character = GetComponent<IThirdPerson>();
			_health = GetComponent<IDamageable>();
			_ragdoll = GetComponent<IRagdoll>();
		}

		void Update()
		{
			// read user input: jump, fire and crouch

			if (!_jumpPressed)
			{
                _jumpPressed = Input.GetButtonDown("Jump");                
            }

            _crouch = Input.GetKey(KeyCode.C);

			if (Input.GetButtonDown("Fire1"))
			{
				if (GetComponent<WeaponManager>().CurrentWeapon.weaponName == "Fists")
				{
					GetComponent<Animator>().SetTrigger("Punch");
				}
			}

			if (sprintAmount > 0)
				HudController.instance.uiStamina.value = sprintAmount / 100;
			else
				HudController.instance.uiStamina.value = 0;
        }

        private void FixedUpdate()
		{
			// read user input: movement
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			
			// calculate move direction and magnitude to pass to character
			Vector3 camForward = new Vector3(_camTransform.forward.x, 0, _camTransform.forward.z).normalized;
            Vector3 move = (v * camForward + h * _camTransform.right) * jogSpeed;
            if (h != 0 && v != 0)
            {
                move *= .7f;
            }

			if (Input.GetButton("Sprint") && sprintAmount > 0)
			{
				move *= runSpeed;
				sprintPressed = true;

                if (sprintAmount > 0)
                    sprintAmount -= sprintDecrement * Time.deltaTime;
			}
			else
			{
                if (sprintAmount < 100)
                    sprintAmount += sprintDecrement * Time.deltaTime;
            }

            if (Input.GetButtonUp("Sprint"))
            {
                sprintPressed = false;
            }

            if (Input.GetButton("Walk")) move *= walkSpeed;
            if (move.magnitude > 1)
				move.Normalize();

			ProcessDamage();

			// pass all parameters to the character control script
			_character.Move(move, _crouch, _jumpPressed);
			_jumpPressed = false;

			// if ragdolled, add a little move
			if (_ragdoll != null && _ragdoll.IsRagdolled)
				_ragdoll.AddExtraMove(move * 100 * Time.deltaTime);
		}

        /// <summary>
        /// if health script attached, shot the character
        /// </summary>
        private void ProcessDamage()
		{
			if (_health == null)
				return;

			if (_jumpPressed && _health.IsDead())
			{
				_health.Health = 1f;
				_jumpPressed = false;
			}
		}

		public void CheckPunch()
		{
            if (punchSensing.targetPlayer)
			{
				punchSensing.Damage(punchSensing.targetPlayer);
				PoliceManager.instance.CheckAlertAllNearbyCops();
            }
        }
	}
}