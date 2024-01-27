using GTAWeaponWheel.Scripts;
using scgGTAController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IThirdPerson))]
	public sealed class ThirdPersonControl : MonoBehaviour
    {
        [Header("Input")]
        InputActions input;

        [Header("Locomotion")]
        public float walkSpeed;
        public float jogSpeed;
        public float runSpeed;
        private IThirdPerson _character;
		private IRagdoll _ragdoll;
		private IDamageable _health;
		private bool _jumpPressed;
		private bool _crouchPressed;
        public float sprintAmount;
        public float sprintDecrement;
        private Vector2 moveInput;
        [HideInInspector] public bool sprintPressed;

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
            
            input = new InputActions();

            input.Player.Enable();

            //Movement
            input.Player.Move.performed += ctx =>
            {
                moveInput = ctx.ReadValue<Vector2>();
            };

            //Sprint input
            input.Player.Sprint.performed += ctx =>
            {
                if (GetComponent<PlayerInput>().currentControlScheme != "Gamepad")
                {
                    sprintPressed = true;
                }

                if (GetComponent<PlayerInput>().currentControlScheme == "Gamepad")
                {
                    sprintPressed = !sprintPressed;
                }
            };

            input.Player.Sprint.canceled += ctx =>
            {
                if (GetComponent<PlayerInput>().currentControlScheme != "Gamepad")
                {
                    sprintPressed = false;
                }
            };

            //Jump
            input.Player.Jump.performed += ctx =>
            {
                _jumpPressed = true;
            };

            input.Player.Jump.canceled += ctx =>
            {
                _jumpPressed = false;
            };

            //Crouch
            input.Player.Crouch.performed += ctx =>
            {
                _crouchPressed = true;
            };

            input.Player.Crouch.canceled += ctx =>
            {
                _crouchPressed = false;
            };

            //Crouch
            input.Player.Walk.performed += ctx =>
            {
                if (moveInput.magnitude != 0)
                    moveInput *= walkSpeed;
            };

            //Crouch
            input.Player.Walk.canceled += ctx =>
            {
                if (moveInput.magnitude != 0)
                    moveInput /= walkSpeed;
            };

            //Punch
            input.Player.Attack.performed += ctx =>
            {
                if (GetComponent<WeaponManager>().CurrentWeapon.weaponName == "Fists")
                {
                    GetComponent<Animator>().SetTrigger("Punch");
                }
            };
        }

		private void Update()
		{
			if (sprintAmount > 0)
				HudController.instance.uiStamina.value = sprintAmount / 100;
			else
				HudController.instance.uiStamina.value = 0;
        }

        private void FixedUpdate()
		{
			// read user input: movement
			float h = moveInput.x;
			float v = moveInput.y;
			
			// calculate move direction and magnitude to pass to character
			Vector3 camForward = new Vector3(_camTransform.forward.x, 0, _camTransform.forward.z).normalized;
            Vector3 move = (v * camForward + h * _camTransform.right) * jogSpeed;
            if (h != 0 && v != 0)
            {
                move *= .7f;
            }

			if (sprintPressed && sprintAmount > 0)
			{
				move *= runSpeed;

                if (sprintAmount > 0)
                    sprintAmount -= sprintDecrement * Time.deltaTime;
			}
			else
			{
                if (sprintAmount < 100)
                    sprintAmount += sprintDecrement * Time.deltaTime;
            }

            if (move.magnitude > 1)
				move.Normalize();

			ProcessDamage();

			// pass all parameters to the character control script
			_character.Move(move, _crouchPressed, _jumpPressed);
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
				NPCManager.instance.CheckAlert(transform);
            }
        }
	}
}