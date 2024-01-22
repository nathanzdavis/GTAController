using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using scgGTAController;

namespace KovSoft.RagdollTemplate.Scripts.Charachter
{
	[RequireComponent(typeof(IRagdollCharacter))]
	public sealed class Ragdoll : MonoBehaviour, IRagdoll
	{
		// animator parameters:
		//readonly int _animatorForward = Animator.StringToHash("Forward");
		//readonly int _animatorTurn = Animator.StringToHash("Turn");

		// animator animations:
		[SerializeField]
		string _animationGetUpFromBelly = "GetUp.GetUpFromBelly";
		[SerializeField]
		string _animationGetUpFromBack = "GetUp.GetUpFromBack";

		Animator _anim;
		IRagdollCharacter _RagdollCharacter;

		// parameters for control character moving while it is ragdolled
		private const float AirSpeed = 5f; // determines the max speed of the character while airborne

		// parameters needed to control ragdolling
		RagdollState _state = RagdollState.Animated;
		float _ragdollingEndTime;   //A helper variable to store the time when we transitioned from ragdolled to blendToAnim state
		const float RagdollToMecanimBlendTime = 0.5f;   //How long do we blend when transitioning from ragdolled to animated
		readonly List<RigidComponent> _rigids = new List<RigidComponent>();
		readonly List<TransformComponent> _transforms = new List<TransformComponent>();
		Transform _hipsTransform;
		Rigidbody _hipsTransformRigid;
		Vector3 _storedHipsPosition;
		Vector3 _storedHipsPositionPrivAnim;
		Vector3 _storedHipsPositionPrivBlend;

		#region implementation of IRagdoll

		public bool Raycast(Ray ray, out RaycastHit hit, float distance)
		{
			var hits = Physics.RaycastAll(ray, distance);
			
			for (int i = 0; i < hits.Length; ++i)
			{
				var h = hits[i];
				if (h.transform != transform && h.transform.root == transform.root)
				{
					hit = h;
					return true;
				}
			}

			// if no hits found, return false
			hit = new RaycastHit();
			return false;
		}

		public bool IsRagdolled
		{
			get
			{
				return
					_state == RagdollState.Ragdolled ||
					_state == RagdollState.WaitStablePosition;
			}
			set
			{
				if (value)
					RagdollIn();
				else
					RagdollOut();
			}
		}

		public void AddExtraMove(Vector3 move)
		{
			if (IsRagdolled)
			{
				Vector3 airMove = new Vector3(move.x * AirSpeed, 0f, move.z * AirSpeed);
				foreach (var rigid in _rigids)
					rigid.RigidBody.AddForce(airMove / 100f, ForceMode.VelocityChange);
			}
		}
		#endregion

		void Start()
		{
			_anim = GetComponent<Animator>();
			_hipsTransform = _anim.GetBoneTransform(HumanBodyBones.Hips);
			_hipsTransformRigid = _hipsTransform.GetComponent<Rigidbody>();
			_RagdollCharacter = GetComponent<IRagdollCharacter>();


			//Get all the rigid bodies that belong to the ragdoll
			Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();

			foreach (Rigidbody rigid in rigidBodies)
			{
				if (rigid.transform == transform)
					continue;

				RigidComponent rigidCompontnt = new RigidComponent(rigid);
				_rigids.Add(rigidCompontnt);
			}

			// disable ragdoll by default
			ActivateRagdollParts(false);

			//Find all the transforms in the character, assuming that this script is attached to the root
			//For each of the transforms, create a BodyPart instance and store the transform
			foreach (var t in GetComponentsInChildren<Transform>())
			{
				var trComp = new TransformComponent(t);
				_transforms.Add(trComp);
			}
		}

		void FixedUpdate()
		{
			if (_state == RagdollState.WaitStablePosition &&
				_hipsTransformRigid.velocity.magnitude < 0.1f)
			{
				GetUp();
			}

			if (_state == RagdollState.Animated &&
				_RagdollCharacter.CharacterVelocity.y < -10f)
			{
				// kill and resuscitate will force character to enter in Ragdoll 
				RagdollIn();
				RagdollOut();
			}
		}

		void LateUpdate()
		{
			if (_state != RagdollState.BlendToAnim)
				return;

			float ragdollBlendAmount = 1f - Mathf.InverseLerp(
				_ragdollingEndTime,
				_ragdollingEndTime + RagdollToMecanimBlendTime,
				Time.time);

			// In LateUpdate(), Mecanim has already updated the body pose according to the animations.
			// To enable smooth transitioning from a ragdoll to animation, we lerp the position of the hips
			// and slerp all the rotations towards the ones stored when ending the ragdolling

			if (_storedHipsPositionPrivBlend != _hipsTransform.position)
			{
				_storedHipsPositionPrivAnim = _hipsTransform.position;
			}
			_storedHipsPositionPrivBlend = Vector3.Lerp(_storedHipsPositionPrivAnim, _storedHipsPosition, ragdollBlendAmount);
			_hipsTransform.position = _storedHipsPositionPrivBlend;

			foreach (TransformComponent trComp in _transforms)
			{
				//rotation is interpolated for all body parts
				if (trComp.PrivRotation != trComp.Transform.localRotation)
				{
					trComp.PrivRotation = Quaternion.Slerp(trComp.Transform.localRotation, trComp.StoredRotation, ragdollBlendAmount);
					trComp.Transform.localRotation = trComp.PrivRotation;
				}

				//position is interpolated for all body parts
				if (trComp.PrivPosition != trComp.Transform.localPosition)
				{
					trComp.PrivPosition = Vector3.Slerp(trComp.Transform.localPosition, trComp.StoredPosition, ragdollBlendAmount);
					trComp.Transform.localPosition = trComp.PrivPosition;
				}
			}

			//if the ragdoll blend amount has decreased to zero, move to animated state
			if (Mathf.Abs(ragdollBlendAmount) < Mathf.Epsilon)
			{
				_state = RagdollState.Animated;
			}
		}

		/// <summary>
		/// Prevents jittering (as a result of applying joint limits) of bone and smoothly translate rigid from animated mode to ragdoll
		/// </summary>
		/// <param name="rigid"></param>
		/// <returns></returns>
		static IEnumerator FixTransformAndEnableJoint(RigidComponent joint)
		{
			if (joint.Joint == null || !joint.Joint.autoConfigureConnectedAnchor)
				yield break;

			SoftJointLimit highTwistLimit = new SoftJointLimit();
			SoftJointLimit lowTwistLimit = new SoftJointLimit();
			SoftJointLimit swing1Limit = new SoftJointLimit();
			SoftJointLimit swing2Limit = new SoftJointLimit();

			SoftJointLimit curHighTwistLimit = highTwistLimit = joint.Joint.highTwistLimit;
			SoftJointLimit curLowTwistLimit = lowTwistLimit = joint.Joint.lowTwistLimit;
			SoftJointLimit curSwing1Limit = swing1Limit = joint.Joint.swing1Limit;
			SoftJointLimit curSwing2Limit = swing2Limit = joint.Joint.swing2Limit;
			
			float aTime = 0.3f;
			Vector3 startConPosition = joint.Joint.connectedBody.transform.InverseTransformVector(joint.Joint.transform.position - joint.Joint.connectedBody.transform.position);

			joint.Joint.autoConfigureConnectedAnchor = false;
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
			{
				Vector3 newConPosition = Vector3.Lerp(startConPosition, joint.ConnectedAnchorDefault, t);
				joint.Joint.connectedAnchor = newConPosition;
					
				curHighTwistLimit.limit = Mathf.Lerp(177, highTwistLimit.limit, t);
				curLowTwistLimit.limit = Mathf.Lerp(-177, lowTwistLimit.limit, t);
				curSwing1Limit.limit = Mathf.Lerp(177, swing1Limit.limit, t);
				curSwing2Limit.limit = Mathf.Lerp(177, swing2Limit.limit, t);

				joint.Joint.highTwistLimit = curHighTwistLimit;
				joint.Joint.lowTwistLimit = curLowTwistLimit;
				joint.Joint.swing1Limit = curSwing1Limit;
				joint.Joint.swing2Limit = curSwing2Limit;

						
				yield return null;
			}
			joint.Joint.connectedAnchor = joint.ConnectedAnchorDefault;
			yield return new WaitForFixedUpdate();
			joint.Joint.autoConfigureConnectedAnchor = true;


			joint.Joint.highTwistLimit = highTwistLimit;
			joint.Joint.lowTwistLimit = lowTwistLimit;
			joint.Joint.swing1Limit = swing1Limit;
			joint.Joint.swing2Limit = swing2Limit;
		}

		/// <summary>
		/// Ragdoll character
		/// </summary>
		private void RagdollIn()
		{
			//Transition from animated to ragdolled

			ActivateRagdollParts(true);     // allow the ragdoll RigidBodies to react to the environment
			_anim.enabled = false;      // disable animation
			_state = RagdollState.Ragdolled;
			ApplyVelocity(_RagdollCharacter.CharacterVelocity);

            if (GetComponentInChildren<Adjuster>())
            {
                Adjuster adj = GetComponentInChildren<Adjuster>();
                adj.useLeftHandIK = false;
            }

		}

		/// <summary>
		/// Smoothly translate to animator's bone positions when character stops falling
		/// </summary>
		private void RagdollOut()
		{
			if (_state == RagdollState.Ragdolled)
				_state = RagdollState.WaitStablePosition;
		}

		private void GetUp()
		{
			//Transition from ragdolled to animated through the blendToAnim state
			_ragdollingEndTime = Time.time;     //store the state change time
			//_anim.SetFloat(_animatorForward, 0f);
			//_anim.SetFloat(_animatorTurn, 0f);
			_anim.enabled = true;               //enable animation
			_state = RagdollState.BlendToAnim;
			_storedHipsPositionPrivAnim = Vector3.zero;
			_storedHipsPositionPrivBlend = Vector3.zero;

			_storedHipsPosition = _hipsTransform.position;

			// get distanse to floor
			Vector3 shiftPos = _hipsTransform.position - transform.position;
			shiftPos.y = GetDistanceToFloor(shiftPos.y);

			// shift and rotate character node without children
			MoveNodeWithoutChildren(shiftPos);

			//Store the ragdolled position for blending
			foreach (TransformComponent trComp in _transforms)
			{
				trComp.StoredRotation = trComp.Transform.localRotation;
				trComp.PrivRotation = trComp.Transform.localRotation;

				trComp.StoredPosition = trComp.Transform.localPosition;
				trComp.PrivPosition = trComp.Transform.localPosition;
			}

			//Initiate the get up animation
			string getUpAnim = CheckIfLieOnBack() ? _animationGetUpFromBack : _animationGetUpFromBelly;
			_anim.Play(getUpAnim, 0, 0);	// you have to set time to 0, or if your animation will interrupt, next time animation starts from previous position
			ActivateRagdollParts(false);    // disable gravity on ragdollParts.

            if (GetComponentInChildren<Adjuster>())
            {
                Adjuster adj = GetComponentInChildren<Adjuster>();
                adj.useLeftHandIK = true;
            }
        }

		private float GetDistanceToFloor(float currentY)
		{
			RaycastHit[] hits = Physics.RaycastAll(new Ray(_hipsTransform.position, Vector3.down));
			float distFromFloor = float.MinValue;

			foreach (RaycastHit hit in hits)
				if (!hit.transform.IsChildOf(transform))
					distFromFloor = Mathf.Max(distFromFloor, hit.point.y);

			if (Mathf.Abs(distFromFloor - float.MinValue) > Mathf.Epsilon)
				currentY = distFromFloor - transform.position.y;

			return currentY;
		}

		private void MoveNodeWithoutChildren(Vector3 shiftPos)
		{
			Vector3 ragdollDirection = GetRagdollDirection();

			// shift character node position without children
			_hipsTransform.position -= shiftPos;
			transform.position += shiftPos;

			// rotate character node without children
			Vector3 forward = transform.forward;
			transform.rotation = Quaternion.FromToRotation(forward, ragdollDirection) * transform.rotation;
			_hipsTransform.rotation = Quaternion.FromToRotation(ragdollDirection, forward) * _hipsTransform.rotation;
		}

		private bool CheckIfLieOnBack()
		{
			var left = _anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg).position;
			var right = _anim.GetBoneTransform(HumanBodyBones.RightUpperLeg).position;
			var hipsPos = _hipsTransform.position;

			left -= hipsPos;
			left.y = 0f;
			right -= hipsPos;
			right.y = 0f;

			var q = Quaternion.FromToRotation(left, Vector3.right);
			var t = q * right;

			return t.z < 0f;
		}

		private Vector3 GetRagdollDirection()
		{
			Vector3 ragdolledFeetPosition = (
				_anim.GetBoneTransform(HumanBodyBones.Hips).position);// +
																	  //_anim.GetBoneTransform(HumanBodyBones.RightToes).position) * 0.5f;
			Vector3 ragdolledHeadPosition = _anim.GetBoneTransform(HumanBodyBones.Head).position;
			Vector3 ragdollDirection = ragdolledFeetPosition - ragdolledHeadPosition;
			ragdollDirection.y = 0;
			ragdollDirection = ragdollDirection.normalized;

			if (CheckIfLieOnBack())
				return ragdollDirection;
			else
				return -ragdollDirection;
		}

		/// <summary>
		/// Apply velocity 'predieVelocity' to to each rigid of character
		/// </summary>
		private void ApplyVelocity(Vector3 predieVelocity)
		{
			foreach (var rigid in _rigids)
				rigid.RigidBody.velocity = predieVelocity;
		}

		private void ActivateRagdollParts(bool activate)
		{
			_RagdollCharacter.CharacterEnable(!activate);

			//_hipsTransform.GetComponentInChildren<Collider>()
			foreach (var rigid in _rigids)
			{
				Collider partColider = rigid.RigidBody.GetComponent<Collider>();

				// fix for RagdollHelper (bone collider - BoneHelper.cs)
				if (partColider == null)
				{
					const string colliderNodeSufix = "_ColliderRotator";
					string childName = rigid.RigidBody.name + colliderNodeSufix;
					Transform transform = rigid.RigidBody.transform.Find(childName);
					partColider = transform.GetComponent<Collider>();
				}

				partColider.isTrigger = !activate;

				if (activate)
				{
					rigid.RigidBody.isKinematic = false;
					StartCoroutine(FixTransformAndEnableJoint(rigid));
				}
				else
					rigid.RigidBody.isKinematic = true;
			}

			//if (activate)
			//	foreach (var joint in GetComponentsInChildren<CharacterJoint>())
			//	{
			//		var jointTransform = joint.transform;
			//		var pivot = joint.connectedBody.transform;
			//		jointTransform.position = pivot.position;
			//		jointTransform.Translate(joint.connectedAnchor, pivot);
			//	}
		}

		//Declare a class that will hold useful information for each body part
		sealed class TransformComponent
		{
			public readonly Transform Transform;
			public Quaternion PrivRotation;
			public Quaternion StoredRotation;

			public Vector3 PrivPosition;
			public Vector3 StoredPosition;

			public TransformComponent(Transform t)
			{
				Transform = t;
			}
		}

		struct RigidComponent
		{
			public readonly Rigidbody RigidBody;
			public readonly CharacterJoint Joint;
			public readonly Vector3 ConnectedAnchorDefault;

			public RigidComponent(Rigidbody rigid)
			{
				RigidBody = rigid;
				Joint = rigid.GetComponent<CharacterJoint>();
				if (Joint != null)
					ConnectedAnchorDefault = Joint.connectedAnchor;
				else
					ConnectedAnchorDefault = Vector3.zero;
			}
		}

		//Possible states of the ragdoll
		enum RagdollState
		{
			/// <summary>
			/// Mecanim is fully in control
			/// </summary>
			Animated,
			/// <summary>
			/// Mecanim turned off, but when stable position will be found, the transition to Animated will heppend
			/// </summary>
			WaitStablePosition,
			/// <summary>
			/// Mecanim turned off, physics controls the ragdoll
			/// </summary>
			Ragdolled,
			/// <summary>
			/// Mecanim in control, but LateUpdate() is used to partially blend in the last ragdolled pose
			/// </summary>
			BlendToAnim,
		}
	}
}
