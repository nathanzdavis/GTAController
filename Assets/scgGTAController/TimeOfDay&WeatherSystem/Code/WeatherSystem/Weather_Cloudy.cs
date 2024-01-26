using UnityEngine;
using System.Collections;

public class Weather_Cloudy : Weather_Base
{
    /********** ----- VARIABLES ----- **********/

    /// <summary>
    /// If the user want to use sounds for this weather effect this is where we save the gameobject from where the sound should be played\n
    /// *Use \link GetSet_gSoundEffect \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    protected GameObject _gSoundEffect;

    /********** ----- GETTERS AND SETTERS ----- **********/

    public GameObject GetSet_gSoundEffect
    {
        get { return _gSoundEffect; }
        set { _gSoundEffect = value; }
    }

    private void Start()
    {
        clWeatherController = (Weather_Controller)this.GetComponent(typeof(Weather_Controller));

        if (_bUseMorningFog == false)
            _fFogMorningAmount = _fFogAmount;

        // Make sure we fade the sound in and out
        _fSoundVolumeIn = _fSoundVolume;
        _fSoundVolumeOut = _fSoundVolume;

        if (_bUsingSound == true)
        {
            if (_gSoundEffect != null)
            {
                if (_adAmbientSound != null)
                {
                    if (_gSoundEffect.GetComponent<AudioSource>() != null)
                    {
                        _bGotAudioSource = true;
                        _gSoundEffect.GetComponent<AudioSource>().clip = _adAmbientSound;
                        _gSoundEffect.GetComponent<AudioSource>().volume = 0.0f;
                        _gSoundEffect.GetComponent<AudioSource>().loop = true;
                    }
                    else
                    {
                        _gSoundEffect.AddComponent<AudioSource>();
                        _gSoundEffect.GetComponent<AudioSource>().clip = _adAmbientSound;
                        _gSoundEffect.GetComponent<AudioSource>().volume = 0.0f;
                        _gSoundEffect.GetComponent<AudioSource>().loop = true;
                        Debug.LogWarning("There was no AUDIOSOURCE on " + _gSoundEffect + " this is now added");
                        _bGotAudioSource = true;
                    }
                }
                else
                    Debug.Log("There is no AMBIENT SOUND attached to the WeatherController on type: " + clWeatherController.en_CurrWeather + " If you don't want to use Ambient sound on this weather type, set Using Ambient Sound to false!");
            }
            else
                Debug.Log("There is no SoundEffect Gameobject attached to the WeatherController on type: " + clWeatherController.en_CurrWeather + " If you don't want to use sound on this weather type, set Using Ambient Sound to false!");
        }
    }

    public override void Init()
    {
        base.Init();

        if (_bUsingSound == true)
            TurnOnSound(_gSoundEffect);
    }

    private void Update()
    {
        UpdateWeather();

        if (_bUseInit == true)
        {
            _fInitTimerStart += Time.deltaTime;

            if (_fInitTimerStart >= _fInitTimerEnd)
            {
                Init();
                _fInitTimerStart = 0.0f;
                _bUseInit = false;
            }
        }
    }

    public override void UpdateWeather()
    {

        if (_bUseDifferentFadeTimes == false)
            OneFadeTimeToRuleThemAll();
        else
            DifferentFadeTimes();
    }

    private void OneFadeTimeToRuleThemAll()
    {
        if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNRISE)
        {
            clWeatherController.UpdateAllWeather(_fSunrise_LightIntensity, _cSunrise_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunrise_SkyTintColor, _cSunrise_SkyGroundColor, _cCloudColor, _fFogMorningAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pNightParticle);
            clWeatherController.ActivateTimesetParticle(_pSunriseParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.DAY)
        {
            clWeatherController.UpdateAllWeather(_fDay_LightIntensity, _cDay_LightColor, 0.0f, _cNight_MoonLightColor,
                _cDay_SkyTintColor, _cDay_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
            clWeatherController.ActivateTimesetParticle(_pDayParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNSET)
        {
            clWeatherController.UpdateAllWeather(_fSunset_LightIntensity, _cSunset_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunset_SkyTintColor, _cSunset_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pDayParticle);
            clWeatherController.ActivateTimesetParticle(_pSunsetParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.NIGHT)
        {
            clWeatherController.UpdateAllWeather(_fNight_LightIntensity, _cNight_LightColor, _fNight_MoonLightIntensity,
                _cNight_MoonLightColor, _cNight_SkyTintColor, _cNight_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
            clWeatherController.ActivateTimesetParticle(_pNightParticle);
        }
    }

    private void DifferentFadeTimes()
    {
        if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNRISE)
        {
            clWeatherController.UpdateAllWeather(_fSunrise_LightIntensity, _cSunrise_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunrise_SkyTintColor, _cSunrise_SkyGroundColor, _cCloudColor, _fFogMorningAmount, _cFogColor, _fSunriseFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pNightParticle);
            clWeatherController.ActivateTimesetParticle(_pSunriseParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.DAY)
        {
            clWeatherController.UpdateAllWeather(_fDay_LightIntensity, _cDay_LightColor, 0.0f, _cNight_MoonLightColor,
                _cDay_SkyTintColor, _cDay_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fDayFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
            clWeatherController.ActivateTimesetParticle(_pDayParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.SUNSET)
        {
            clWeatherController.UpdateAllWeather(_fSunset_LightIntensity, _cSunset_LightColor, 0.0f, _cNight_MoonLightColor,
                _cSunset_SkyTintColor, _cSunset_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fSunsetFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pDayParticle);
            clWeatherController.ActivateTimesetParticle(_pSunsetParticle);
        }
        else if (clWeatherController.gTimeOfDay.GetComponent<ToD_Base>().enCurrTimeset == ToD_Base.Timeset.NIGHT)
        {
            clWeatherController.UpdateAllWeather(_fNight_LightIntensity, _cNight_LightColor, _fNight_MoonLightIntensity, _cNight_MoonLightColor,
                _cNight_SkyTintColor, _cNight_SkyGroundColor, _cCloudColor, _fFogAmount, _cFogColor, _fNightFadeTime);

            clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
            clWeatherController.ActivateTimesetParticle(_pNightParticle);
        }
    }

    public override void TurnOnSound(GameObject gameobject)
    {
        base.TurnOnSound(gameobject);

        // Check to tell that we should start fading the sound out when exiting this weather function
        _bTurnOffSoundAtExit = true;
    }

    public override void ExitWeatherEffect(GameObject gameobject)
    {
        clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
        clWeatherController.DeactivateTimesetParticle(_pDayParticle);
        clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
        clWeatherController.DeactivateTimesetParticle(_pNightParticle);

        if (_bTurnOffSoundAtExit == true)
        {
            if (_bUsingSound == true && _gSoundEffect != null)
                base.ExitWeatherEffect(gameobject);

            _bTurnOffSoundAtExit = false;
        }
    }
}

