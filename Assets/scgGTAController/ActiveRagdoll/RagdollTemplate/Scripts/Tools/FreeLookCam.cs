using KovSoft.RagdollTemplate.Scripts.Charachter;
using System;
using UnityEngine;

namespace KovSoft.RagdollTemplate.Scripts.Tools
{
	public sealed class FreeLookCam : MonoBehaviour
	{
		[SerializeField] private Transform _pivot;
		[SerializeField] private float _turnSmoothing = 0.1f;// How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
		[SerializeField] private float _tiltMax = 75f; // The maximum value of the x axis rotation of the pivot.
		[SerializeField] private float _tiltMin = 45f; // The minimum value of the x axis rotation of the pivot.
		[SerializeField] private float _maxDistanse = 3f;
		[SerializeField] private float _minDistanse = 0.7f;
		[Range(0f, 10f)] [SerializeField] private float _mouseSensitive = 1.5f;// How fast the rig will rotate from user input.

		private Transform _cameraPivot; // the point at which the camera pivots around
		private Transform _camera; // the point at which the camera pivots around
		private IRagdoll _ragdoll;		// ragdoll component of character
		private float _yawAngle; // The rig's y axis rotation.
		private float _pitchAngle; // The pivot's x axis rotation.
		private float _smoothX;
		private float _smoothY;
		private float _smoothXvelocity;
		private float _smoothYvelocity;

		// Use this for initialization
		void Start()
		{
			_camera = Camera.main.transform;
			if (_pivot == null)
			{
				Debug.LogError("CameraFree Missing pivot");
				return;
			}
			_cameraPivot = _camera.parent;
			_ragdoll = _pivot.GetComponentInParent<IRagdoll>();
		}

		/// <summary>
		/// I need this to assure that i set camera position only once per frame
		/// </summary>
		int _callCountChecker;
		void Update()
		{
			if (UpdateCameraPos(false))
				++_callCountChecker;
		}

		void LateUpdate()
		{
			if (UpdateCameraPos(true))
				++_callCountChecker;
		
			if (_callCountChecker != 1)
				throw new InvalidOperationException("There are invalid count of 'setting camera' calls. Count = " +
					_callCountChecker.ToString());
		
			_callCountChecker = 0;
		}

		bool UpdateCameraPos(bool lateUpdate)
		{
			// if ragdolled, you need to set position in "Update()" method
			// otherwise in "LateUpdate()"
			if (_ragdoll != null && _ragdoll.IsRagdolled)
			{
				if (lateUpdate)
					return false;
			}
			else if (!lateUpdate)
				return false;

			if (_pivot == null || Time.deltaTime < Mathf.Epsilon)
				return true;

			transform.position = _pivot.transform.position;
			HandleRotationMovement();
			HandleWalls();

			return true;
		}
		
		private void HandleRotationMovement()
		{
			// Read the user input
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");

			// smooth the user input
			if (_turnSmoothing > 0)
			{
				_smoothX = Mathf.SmoothDamp(_smoothX, x, ref _smoothXvelocity, _turnSmoothing);
				_smoothY = Mathf.SmoothDamp(_smoothY, y, ref _smoothYvelocity, _turnSmoothing);
			}
			else
			{
				_smoothX = x;
				_smoothY = y;
			}

			// Adjust the look angle by an amount proportional to the turn speed and horizontal input.
			_yawAngle += _smoothX * _mouseSensitive;

			// on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
			_pitchAngle -= _smoothY * _mouseSensitive;
			// and make sure the new value is within the tilt range
			_pitchAngle = Mathf.Clamp(_pitchAngle, -_tiltMin, _tiltMax);

			// Tilt input around X is applied to the pivot (the child of this object)
			_cameraPivot.localRotation = Quaternion.Euler(_pitchAngle, _yawAngle, 0f);
		}

		/// <summary>
		/// Restrict camera distance if wall between camera and character
		/// </summary>
		private void HandleWalls()
		{

			Vector3 dir = _camera.position - _cameraPivot.position;
			dir.Normalize();
			RaycastHit[] hits = Physics.SphereCastAll(_cameraPivot.position, 0.3f, dir, _maxDistanse);
			float d = _maxDistanse;
			for (int i = 0; i < hits.Length; ++i)
			{
				var hit = hits[i];
				if (hit.transform.root != _pivot.transform.root &&
				    d > hit.distance)
					d = hit.distance;
			}

			if (d < _minDistanse)
				d = _minDistanse;
			
			Debug.DrawRay(_cameraPivot.position, dir * d);
			_camera.position = _cameraPivot.position + dir * d;
		}
	}
}