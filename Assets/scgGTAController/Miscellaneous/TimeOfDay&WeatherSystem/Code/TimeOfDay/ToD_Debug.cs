using UnityEngine;
using System.Collections;

/// <summary>
/// Debug: For our Time Of Day system
/// </summary>
/// <!-- 
/// By: Tobias Johansson
/// Contact: tobias@johansson-tobias.com
/// Portfolio: http://www.johansson-tobias.com
/// -->
/// <remarks>
/// This script handles all our debug information for our Time of Day system
/// I'm testing using P4 for Version Control
/// Working
/// </remarks>

public class ToD_Debug : MonoBehaviour 
{
    private ToD_Base _clToDBase;
    private bool _bTodDebugOn;
    private bool _bMoreDebugInfo;

    public GUISkin guiDebugSkin;

	void Start() 
    {
        _clToDBase = (ToD_Base)this.GetComponent(typeof(ToD_Base));
        _bTodDebugOn = false;
        _bMoreDebugInfo = false;
	}   

	void Update() 
    {
        BasicDebugControls();
	}

    private void BasicDebugControls()
    {
        // Debug information control
        if (Input.GetKeyDown(KeyCode.P) && _bTodDebugOn == false)
            _bTodDebugOn = true;
        else if (Input.GetKeyDown(KeyCode.P) && _bTodDebugOn == true)
            _bTodDebugOn = false;
        else if (Input.GetKeyDown(KeyCode.H) && _bMoreDebugInfo == false)
            _bMoreDebugInfo = true;
        else if (Input.GetKeyDown(KeyCode.H) && _bMoreDebugInfo == true)
            _bMoreDebugInfo = false;

        // Time of day speed control
        if (Input.GetKeyDown(KeyCode.Alpha0) && _clToDBase.GetSet_fTimeMultiplier <= 9.5f)
            _clToDBase.GetSet_fTimeMultiplier += 0.5f;
        else if (Input.GetKeyDown(KeyCode.Alpha9) && _clToDBase.GetSet_fTimeMultiplier >= 0.5f)
            _clToDBase.GetSet_fTimeMultiplier -= 0.5f;
        else if (Input.GetKeyDown(KeyCode.Alpha8) && _clToDBase.GetSet_fTimeMultiplier >= 0.5f)
            _clToDBase.GetSet_fTimeMultiplier = 1.0f;
    }

    void OnGUI()
    {
        if (guiDebugSkin != null)
            GUI.skin = guiDebugSkin;

        if (_bTodDebugOn == true)
        {
            // Tells that debug mode is on
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width / 2 - 120, 20, 240, 30), "Debugging: TIME OF DAY");
            GUI.Label(new Rect(Screen.width / 2 - 225, 40, 450, 30), "Press H for more Debug information and controls");

            // What are we debugging
            GUI.color = Color.red;

            // Current time *F0 means we show 0 of the floats decimals
            GUI.Label(new Rect(20, 60, 200, 30), "Current time:");
            GUI.Label(new Rect(220, 60, 25, 30), _clToDBase.Get_fCurrentHour.ToString("F1"));
            GUI.Label(new Rect(250, 60, 200, 30), ":");
            GUI.Label(new Rect(260, 60, 200, 30), _clToDBase.Get_fCurrentMinute.ToString("F0"));

            // Current timeset
            GUI.Label(new Rect(20, 90, 200, 30), "Timeset:");
            GUI.Label(new Rect(220, 90, 200, 30), _clToDBase.enCurrTimeset.ToString());

            // Current time of day speed
            GUI.Label(new Rect(20, 150, 200, 30), "Current ToD Speed:");
            GUI.Label(new Rect(220, 150, 200, 30), _clToDBase.GetSet_fTimeMultiplier.ToString());

            // How many days have we played
            GUI.Label(new Rect(20, 180, 200, 30), "Days played:");
            GUI.Label(new Rect(220, 180, 200, 30), _clToDBase.Get_iAmountOfDaysPlayed.ToString());

            if (_bMoreDebugInfo == true)
            {
                GUI.color = Color.blue;
               
                // More controls
                GUI.Label(new Rect(20, 240, 600, 30), "Press 0 to add 0.5f to the Time of Day speed");
                GUI.Label(new Rect(20, 270, 600, 30), "Press 9 to substract 0.5f off the Time of Day speed");
                GUI.Label(new Rect(20, 300, 600, 30), "Press 8 to reset Time of Day speed to 1.0f");
                GUI.Label(new Rect(20, 330, 600, 30), "Max speed is 10.0f, and Minimum speed is 0.0f");
            }
        }
    }
}

