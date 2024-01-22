using UnityEngine;
using System.Collections;


namespace KovSoft.RagdollTemplate.Scripts.Tools
{
	/// <summary>
	/// Game speed changer
	/// </summary>
	public class SpeedController : MonoBehaviour
	{
		float _fixedDeltaTime;
		float _timeScale;
		string text =
	@"Speed:
Press 1 - normal
Press 2 - 1/2
Press 3 - 1/5
Press 4 - 1/10";

		void Start()
		{
			_fixedDeltaTime = Time.fixedDeltaTime;
			_timeScale = Time.timeScale;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Time.fixedDeltaTime = _fixedDeltaTime;
				Time.timeScale = _timeScale;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Time.fixedDeltaTime = _fixedDeltaTime / 2;
				Time.timeScale = _timeScale / 2;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Time.fixedDeltaTime = _fixedDeltaTime / 5;
				Time.timeScale = _timeScale / 5;
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				Time.fixedDeltaTime = _fixedDeltaTime / 10;
				Time.timeScale = _timeScale / 10;
			}
		}

		void OnGUI()
		{
			GUI.skin.label.alignment = TextAnchor.UpperLeft;
			GUI.Label(new Rect(10, 10, 100, 100), text);
		}
	}
}
