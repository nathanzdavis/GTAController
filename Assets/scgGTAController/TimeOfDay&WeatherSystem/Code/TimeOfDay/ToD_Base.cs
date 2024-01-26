using UnityEngine;
using System.Collections;

/// <summary>
/// Base: Time of Day
/// </summary>
/// <!-- 
/// By: Tobias Johansson
/// Contact: tobias@johansson-tobias.com
/// Portfolio: http://www.johansson-tobias.com
/// -->
/// <remarks>
/// This is the main script for our Time of Day system. This handles the time and the updating of the sun. 
/// </remarks>
public class ToD_Base : MonoBehaviour 
{
    /********** ----- VARIABLES ----- **********/

    /// <summary>
    /// Does the user want to use a moon? 
    /// </summary>
    [SerializeField]
    private bool _bUseMoon = true;

    /// <summary>
    /// This is used to check if the user want to use weather effects or only the time of day
    /// *Use \link GetSet_bUseWeather \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    private bool _bUseWeather = true;

    /// <summary>
    /// This is where the user sets how long a full 24 hour day should be in seconds.
    /// *Use \link GetSet_bUseWeather \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    private float _fSecondInAFullDay = 60.0f;

    /// <summary>
    /// With this it is possible to change the speed for the time of day system. *This is only used for debuggning at the moment.\n
    /// *Use \link GetSet_fTimeMultiplier \endlink if you want to access or change this from another script.
    /// </summary>
    [SerializeField]
    private float _fTimeMultiplier = 1.0f;

    /// <summary>
    /// A day in the game goes from 0 to 1.\n
    /// *Use \link Get_fCurrentTimeOfDay \endlink if you want to see the current value of this.
    /// </summary>
    [SerializeField, Range(0, 1)]
    private float _fCurrentTimeOfDay;

    /// <summary>
    /// We use this CONST to re-count \link _fCurrentTimeOfDay \endlink to hours so it's easier for designer to set when they want the game and such in hours instead of in %. 
    /// </summary>
    private const float ONEHOURLENGTH = 1.0f / 24.0f;

    /// <summary>
    /// This is the INT we use so the designer can set the games starting time in hours from the editor\n
    /// *Use \link GetSet_iStartHour \endlink if you want to access or change this from another script.\n
    /// **This is only called once in the Start() so the game knows which time it should start at.
    /// </summary>
    [SerializeField]
    private int _iStartHour;

    private float _fStartingHour;

    /// <summary>
    /// This is the INT we use so the designer can set at which hour they want SUNRISE to start\n
    /// *Use \link GetSet_iSunriseStart \endlink if you want to access or change this from another script or during runtime.
    /// </summary>
    [SerializeField]
    private int _iSunriseStart;

    /// <summary>
    /// This is the INT we use so the designer can set at which hour they want DAY to start\n
    /// *Use \link GetSet_iDayStart \endlink if you want to access or change this from another script or during runtime.
    /// </summary>
    [SerializeField]
    private int _iDayStart;

    /// <summary>
    /// This is the INT we use so the designer can set at which hour they want SUNSET to start\n
    /// *Use \link GetSet_iSunsetStart \endlink if you want to access or change this from another script or during runtime.
    /// </summary>
    [SerializeField]
    private int _iSunsetStart;

    /// <summary>
    /// This is the INT we use so the designer can set at which hour they want NIGHT to start\n
    /// *Use \link GetSet_iNightStart \endlink if you want to access or change this from another script or during runtime.
    /// </summary>
    [SerializeField]
    private int _iNightStart;

    /// <summary>
    /// We use this varible to re-count the choosen hour into %
    /// </summary>
    private float _fStartingSunrise;

    /// <summary>
    /// We use this varible to re-count the choosen hour into %
    /// </summary>
    private float _fStartingDay;

    /// <summary>
    /// We use this varible to re-count the choosen hour into %
    /// </summary>
    private float _fStartingSunset;

    /// <summary>
    /// We use this varible to re-count the choosen hour into %
    /// </summary>
    private float _fStartingNight;

    /// <summary>
    /// I re-count \link Get_fCurrentTimeOfDay \endlink to hours and have \link Get_fCurrentHour \endlink to show it for when we debug the time of day. 
    /// </summary>
    private float _fCurrentHour;

    /// <summary>
    /// I re-count \link Get_fCurrentTimeOfDay \endlink to minutes and have \link _fCurrentMinute \endlink to show it for when we debug the time of day. 
    /// </summary>
    private float _fCurrentMinute;

    /// <summary>
    /// This is an INT we use to count how many game days the game has been played since the start.\n
    /// *This is at the moment set to 0 in Start(). If you want to save this for future reference this need to be moved as it will be re-set everytime the game starts otherwise.\n
    /// *Use \link Get_iAmountOfDaysPlayed \endlink if you want to see the current value of this.
    /// </summary>
    private int _iAmountOfDaysPlayed;

    /// <summary>
    /// Weather master is needed so the time of day can update days as this controls when the weather should change. 
    /// </summary>
    public GameObject gWeatherMaster;

    /// <summary>
    /// This needs to be a directional light so we have a light that covers the whole world. 
    /// </summary>
    public Light lSun;

    /// <summary>
    /// This needs to be a directional light so we have a light that covers the whole world. 
    /// </summary>
    public Light lMoon;

    /// <summary>
    /// We use this to control Sunrise, Day, Sunset and Night
    /// </summary>
    public enum Timeset
    {
        SUNRISE, 
        DAY,
        SUNSET,
        NIGHT
    };

    [HideInInspector]
    public Timeset enCurrTimeset;

    /********** ----- GETTERS AND SETTERS ----- **********/

    public float Get_fCurrentTimeOfDay { get { return _fCurrentTimeOfDay; } }
    public float Get_fCurrentHour { get { return _fCurrentHour; } }
    public float Get_fCurrentMinute { get { return _fCurrentMinute; } }
    public int Get_iAmountOfDaysPlayed { get { return _iAmountOfDaysPlayed; } }

    public bool GetSet_bUseMoon
    {
        get { return _bUseMoon; }
        set { _bUseMoon = value; }
    }

    public bool GetSet_bUseWeather
    {
        get { return _bUseWeather; }
        set { _bUseWeather = value; }
    }

    public float GetSet_fSecondInAFullDay
    {
        get { return _fSecondInAFullDay; }
        set { _fSecondInAFullDay = value; }
    }

    public float GetSet_fTimeMultiplier
    {
        get { return _fTimeMultiplier; }
        set { _fTimeMultiplier = value; }
    }

    public int GetSet_iStartHour
    {
        get { return _iStartHour; }
        set { _iStartHour = value; }
    }

    public int GetSet_iSunriseStart
    {
        get { return _iSunriseStart; }
        set { _iSunriseStart = value; }
    }

    public int GetSet_iDayStart
    {
        get { return _iDayStart; }
        set { _iDayStart = value; }
    }


    public int GetSet_iSunsetStart
    {
        get { return _iSunsetStart; }
        set { _iSunsetStart = value; }
    }


    public int GetSet_iNightStart
    {
        get { return _iNightStart; }
        set { _iNightStart = value; }
    }

    /// <summary>
    /// Unity function - See Unity documentation
    /// </summary>
    void Start()
    {
        _fStartingHour = ONEHOURLENGTH * (float)_iStartHour;
        _fCurrentTimeOfDay = _fStartingHour;

        _fStartingSunrise = ONEHOURLENGTH * (float)_iSunriseStart;
        _fStartingDay = ONEHOURLENGTH * (float)_iDayStart;
        _fStartingSunset = ONEHOURLENGTH * (float)_iSunsetStart;
        _fStartingNight = ONEHOURLENGTH * (float)_iNightStart;

        _iAmountOfDaysPlayed = 0;
        _fCurrentHour = 0.0f;
        _fCurrentMinute = 0.0f;
    }

    /// <summary>
    /// Unity function - See Unity documentation
    /// </summary>
    void Update()
    {
        UpdateSunAndMoon();
        UpdateTimeset();

        // Controls the speed of our "clock"
        _fCurrentTimeOfDay += (Time.deltaTime / _fSecondInAFullDay) * _fTimeMultiplier;

        // Digital time
        _fCurrentHour = 24 * _fCurrentTimeOfDay;
        _fCurrentMinute = 60 * (_fCurrentHour - Mathf.Floor(_fCurrentHour));

        // resets our time of day to 0 + adds a day to our amount of days played
        if (_fCurrentTimeOfDay >= 1.0f)
        {
            _fCurrentTimeOfDay = 0.0f;
            _iAmountOfDaysPlayed += 1;

            if (_bUseWeather == true)
                gWeatherMaster.GetComponent<Weather_Controller>().GetSet_iAmountOfDaysSinceLastWeather += 1;
        }
    }

    /// <summary>
    /// This is used inside Unitys Update() to update our SUN and MOON (if you have the MOON turned on). 
    /// </summary>
    void UpdateSunAndMoon()
    {
        // This rotates the sun 360 degree in X-axis according to our current time of day.
        lSun.transform.localRotation = Quaternion.Euler((_fCurrentTimeOfDay * 360) - 90, 170, 0);

        if (_bUseMoon == true)
            lMoon.transform.localRotation = Quaternion.Euler((_fCurrentTimeOfDay * 360) - 270, 170, 0);
    }



    void UpdateTimeset()
    {
        if (_fCurrentTimeOfDay >= _fStartingSunrise && _fCurrentTimeOfDay <= _fStartingDay && enCurrTimeset != Timeset.SUNRISE)
            SetCurrentTimeset(Timeset.SUNRISE);
        else if (_fCurrentTimeOfDay >= _fStartingDay && _fCurrentTimeOfDay <= _fStartingSunset && enCurrTimeset != Timeset.DAY)
            SetCurrentTimeset(Timeset.DAY);
        else if (_fCurrentTimeOfDay >= _fStartingSunset && _fCurrentTimeOfDay <= _fStartingNight && enCurrTimeset != Timeset.SUNSET)
            SetCurrentTimeset(Timeset.SUNSET);
        else if (_fCurrentTimeOfDay >= _fStartingNight || _fCurrentTimeOfDay <= _fStartingSunrise && enCurrTimeset != Timeset.NIGHT)
            SetCurrentTimeset(Timeset.NIGHT);
    }

    void SetCurrentTimeset(Timeset currentTime)
    {
        enCurrTimeset = currentTime;
    }

}

