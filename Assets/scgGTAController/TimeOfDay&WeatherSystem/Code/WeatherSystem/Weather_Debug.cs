using UnityEngine;
using System.Collections;

public class Weather_Debug : MonoBehaviour 
{
    private Weather_Controller _clWeatherController;
    private bool _bWeatherDebugOn;
    private bool _bMoreDebugInfo;

    public GUISkin guiDebugSkin;

	void Start () 
    {
        _clWeatherController = (Weather_Controller)this.GetComponent(typeof(Weather_Controller));
        _bWeatherDebugOn = false;
	}

    void Update()
    {
        BasicDebugControls();

        if (_bMoreDebugInfo == true)
            AdvancedDebugControls();
    }

    private void BasicDebugControls()
    {
        if (Input.GetKeyDown(KeyCode.O) && _bWeatherDebugOn == false)
            _bWeatherDebugOn = true;
        else if (Input.GetKeyDown(KeyCode.O) && _bWeatherDebugOn == true)
            _bWeatherDebugOn = false;
        else if (Input.GetKeyDown(KeyCode.H) && _bMoreDebugInfo == false)
            _bMoreDebugInfo = true;
        else if (Input.GetKeyDown(KeyCode.H) && _bMoreDebugInfo == true)
            _bMoreDebugInfo = false;
    }

    private void AdvancedDebugControls()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            _clWeatherController.UseWeatherTypeDebug(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            _clWeatherController.UseWeatherTypeDebug((int)Weather_Controller.WeatherType.SUN);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            _clWeatherController.UseWeatherTypeDebug((int)Weather_Controller.WeatherType.CLOUDY);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            _clWeatherController.UseWeatherTypeDebug((int)Weather_Controller.WeatherType.RAIN);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            _clWeatherController.UseWeatherTypeDebug((int)Weather_Controller.WeatherType.THUNDERSTORM);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            _clWeatherController.UseWeatherTypeDebug((int)Weather_Controller.WeatherType.SNOW);
    }

    void OnGUI()
    {
        if (guiDebugSkin != null)
            GUI.skin = guiDebugSkin;
        else
            Debug.Log("Missing Debug skin");

        if (_bWeatherDebugOn == true)
        {
            // Tells that debug mode is on
            GUI.color = Color.yellow;
            GUI.Label(new Rect(Screen.width / 2 - 120, 20, 240, 30), "Debugging: WEATHER SYSTEM");
            GUI.Label(new Rect(Screen.width / 2 - 225, 40, 450, 30), "Press H for more Debug information and controls");

            // What are we debugging
            GUI.color = Color.red;

            GUI.Label(new Rect(20, 60, 300, 30), "Current weather:");
            GUI.Label(new Rect(320, 60, 100, 30), _clWeatherController.en_CurrWeather.ToString());

            GUI.Label(new Rect(20, 90, 300, 30), "Last weather:");
            GUI.Label(new Rect(320, 90, 100, 30), _clWeatherController.en_LastWeather.ToString());

            // *F2 means we show 2 of the floats decimals
            GUI.Label(new Rect(20, 120, 300, 30), "Current temprature:");
            GUI.Label(new Rect(320, 120, 100, 30), _clWeatherController.GetSet_fCurrTemp.ToString("F2"));

            // Weather change
            GUI.Label(new Rect(20, 180, 300, 30), "Next weather change (days):");
            GUI.Label(new Rect(320, 180, 100, 30), _clWeatherController.Get_iAmountOfDaysToNewWeather.ToString());

            GUI.Label(new Rect(20, 210, 300, 30), "Days since last weather change:");
            GUI.Label(new Rect(320, 210, 100, 30), _clWeatherController.GetSet_iAmountOfDaysSinceLastWeather.ToString());

            GUI.Label(new Rect(20, 240, 300, 30), "Weather change:");
            GUI.Label(new Rect(320, 240, 100, 30), _clWeatherController.Get_bStartWeatherChange.ToString());

            GUI.Label(new Rect(20, 270, 300, 30), "Exiting Weather Timer:");
            GUI.Label(new Rect(320, 270, 100, 30), _clWeatherController.Get_fTimeChangeWeatherStart.ToString());

            if (_bMoreDebugInfo == true)
            {
                GUI.color = Color.blue;

                // More controls
                GUI.Label(new Rect(20, 320, 600, 30), "Press 1 to try getting a new RANDOM weather");
                GUI.Label(new Rect(20, 350, 600, 30), "Press 2 to get SUN");
                GUI.Label(new Rect(20, 380, 600, 30), "Press 3 to get CLOUDY");
                GUI.Label(new Rect(20, 410, 600, 30), "Press 4 to get RAIN");
                GUI.Label(new Rect(20, 440, 600, 30), "Press 5 to get THUNDERSTORM");
                GUI.Label(new Rect(20, 470, 600, 30), "Press 6 to get SNOW");
            }
        }
    }
}

