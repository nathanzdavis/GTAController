using UnityEngine;
using System.Collections;

public class Weather_Thunderstorm : Weather_Base
{
    /********** ----- VARIABLES ----- **********/

    /********** GAMEOBJECTS Settings **********/
    
    // RAIN
    /// <summary>
    /// Here we save the particle effect that we use for the rain effect. This is also where the sound will play from\n
    /// *Use \link GetSet_gPartRain \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private GameObject _gPartRain;

    /// <summary>
    /// I create a new gameobject at run time for the lighting. This object is deleted when we change weather type. 
    /// </summary>
    private GameObject _gLighting;

    /********** GENERAL Settings **********/

    private float _fEndParticleTimerStart;
    private float _fEndParticleTimerEnd;

    /********** THUNDER Settings **********/

    /// <summary>
    /// This is our timer that counts up to for when lighting should strike. 
    /// </summary>
    private float _fTimerForNextLighting;

    /// <summary>
    /// float to save the randomised time on how often the lighting should strike. 
    /// </summary>
    private float _fTimeForNextLighting;

    /// <summary>
    /// Here we save the minimum time the user sets that a lightning should strike\n
    /// *Use \link GetSet_RandomThunderTimeMin \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fRandomThunderTimeMin = 30.0f;

    /// <summary>
    /// Here we save the maximum time the user sets that a lightning should strike\n
    /// *Use \link GetSet_RandomThunderTimeMin \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fRandomThunderTimeMax = 240.0f;

    /// <summary>
    /// Here we save the how strong the light of the lightning should be\n
    /// *Use \link GetSet_fLightningIntensity \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fLightningIntensity = 5.0f;

    /// <summary>
    /// Here we save how big of a range the light of the lightning should cover\n
    /// *Use \link GetSet_fLightningRange \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fLightningRange = 500.0f;

    /// <summary>
    /// Here we save for how long the light of the lightning should be on\n
    /// *Use \link GetSet_fTimeBeforeLightningLightTurnsOff \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fTimeBeforeLightningLightTurnsOff = 0.4f;

    /// <summary>
    /// Here we save the Audioclip we play when a lightning happens\n
    /// *Use \link GetSet_adThunderSound \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private AudioClip _adThunderSound;

    /// <summary>
    /// Here we save the volume for the lightning thunder strike sound\n
    /// *Use \link GetSet_fLightningVolume \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fLightningVolume = 1.0f;

    /// <summary>
    /// Here we save how far from the Weathermaster the lightning could strike, the minimum value\n
    /// *Use \link GetSet_fLightningRangeFromWeatherMasterMin \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fLightningRangeFromWeatherMasterMin = 10.0f;

    /// <summary>
    /// Here we save how far from the Weathermaster the lightning could strike, the maximum value\n
    /// *Use \link GetSet_fLightningRangeFromWeatherMasterMax \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fLightningRangeFromWeatherMasterMax = 10.0f;

    /// <summary>
    /// Here we save how high the lightning should strike from the weather master\n
    /// *Use \link GetSet_fLightningHeight \endlink if you want to access this.
    /// </summary>
    [SerializeField]
    private float _fLightningHeight = 50.0f;

    /********** ----- GETTERS AND SETTERS ----- **********/

    /********** GAMEOBJECTS Settings **********/
    
    public GameObject GetSet_gPartRain
    {
        get { return _gPartRain; }
        set { _gPartRain = value; }
    }

    /********** THUNDER **********/

    public float GetSet_RandomThunderTimeMin
    {
        get { return _fRandomThunderTimeMin; }
        set { _fRandomThunderTimeMin = value; }
    }

    public float GetSet_RandomThunderTimeMax
    {
        get { return _fRandomThunderTimeMax; }
        set { _fRandomThunderTimeMax = value; }
    }

    public float GetSet_fLightningIntensity
    {
        get { return _fLightningIntensity; }
        set { _fLightningIntensity = value; }
    }

    public float GetSet_fLightningRange
    {
        get { return _fLightningRange; }
        set { _fLightningRange = value; }
    }

    public float GetSet_fTimeBeforeLightningLightTurnsOff
    {
        get { return _fTimeBeforeLightningLightTurnsOff; }
        set { _fTimeBeforeLightningLightTurnsOff = value; }
    }

    public AudioClip GetSet_adThunderSound
    {
        get { return _adThunderSound; }
        set { _adThunderSound = value; }
    }

    public float GetSet_fLightningVolume
    {
        get { return _fLightningVolume; }
        set { _fLightningVolume = value; }
    }

    public float GetSet_fLightningRangeFromWeatherMasterMin
    {
        get { return _fLightningRangeFromWeatherMasterMin; }
        set { _fLightningRangeFromWeatherMasterMin = value; }
    }


    public float GetSet_fLightningRangeFromWeatherMasterMax
    {
        get { return _fLightningRangeFromWeatherMasterMax; }
        set { _fLightningRangeFromWeatherMasterMax = value; }
    }


    public float GetSet_fLightningHeight
    {
        get { return _fLightningHeight; }
        set { _fLightningHeight = value; }
    }

    public float Get_TimerForNextLighting { get { return _fTimerForNextLighting; } }
    public float Get_TimeBeforeNextLighting { get { return _fTimeForNextLighting; } }

    private void Start()
    {
        clWeatherController = (Weather_Controller)this.GetComponent(typeof(Weather_Controller));

        if (_bUseMorningFog == false)
            _fFogMorningAmount = _fFogAmount;

        // Make sure we fade the sound in and out
        _fSoundVolumeIn = _fSoundVolume;
        _fSoundVolumeOut = _fSoundVolume;

        // This timer makes sure that the rain stops falling and don't suddenly just disappears
        _fEndParticleTimerStart = 0.0f;
        _fEndParticleTimerEnd = 5.0f;

        if (_bUsingSound == true)
        {
            if (_adAmbientSound != null)
            {
                if (_gPartRain.GetComponent<AudioSource>() != null)
                {
                    _bGotAudioSource = true;
                    _gPartRain.GetComponent<AudioSource>().clip = _adAmbientSound;
                    _gPartRain.GetComponent<AudioSource>().volume = 0.0f;
                    _gPartRain.GetComponent<AudioSource>().loop = true;
                }
                else
                {
                    _gPartRain.AddComponent<AudioSource>();
                    _gPartRain.GetComponent<AudioSource>().clip = _adAmbientSound;
                    _gPartRain.GetComponent<AudioSource>().volume = 0.0f;
                    _gPartRain.GetComponent<AudioSource>().loop = true;
                    Debug.LogWarning("There was no AUDIOSOURCE on " + _gPartRain + " this is now added");

                    _bGotAudioSource = true;
                }
            }
            else
                Debug.Log("There is no AMBIENT SOUND attached to the WeatherController on type: " + clWeatherController.en_CurrWeather + " If you don't want to use Ambient sound on this weather type, set Using Ambient Sound to false!");
        }

        // Here we set values for this so we have starting values, they are hardcoded as we need them to be high!
        _fTimeForNextLighting = Random.Range(_fRandomThunderTimeMin + 500.0f, _fRandomThunderTimeMax + 1000.0f);
        _fTimerForNextLighting = 0.0f; // As we create a lighting here in the INIT(), we reset the timer after this 
    }

    public override void Init()
    {
        base.Init();
        TurnOnRain();

        // We turn on emission on the particle system as we always turn it off in the end
        if (_gPartRain != null)
        {
            ParticleSystem.EmissionModule em = _gPartRain.GetComponent<ParticleSystem>().emission;
            em.enabled = true;
        }

        // Create the lighting component and components needs for it
        _gLighting = new GameObject("ThunderLighting");
        _gLighting.AddComponent<Light>();
        _gLighting.AddComponent<AudioSource>();

        // Lighting (Light settings)
        _gLighting.GetComponent<Light>().range = _fLightningRange;
        _gLighting.GetComponent<Light>().intensity = 0.0f; // We start this at 0 as we want the light to blink like lighting does

        if (_gLighting == null)
            Instantiate(_gLighting, new Vector3(0, _fLightningHeight, 0), new Quaternion(0, 0, 0, 0));

        _fTimeForNextLighting = Random.Range(_fRandomThunderTimeMin, _fRandomThunderTimeMax);
        _fTimerForNextLighting = 0.0f; // As we create a lighting here in the INIT(), we reset the timer after this 
    }

    private void Update()
    {
        UpdateWeather();
        UpdateThunder();

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

    public void UpdateThunder()
    {
        _fTimerForNextLighting += Time.deltaTime;

        if (_fTimerForNextLighting >= _fTimeForNextLighting)
            Lightning();
    }

    private void Lightning()
    {
        if (_gLighting != null)
        {
            float fRandXPosition;
            fRandXPosition = Random.Range(_fLightningRangeFromWeatherMasterMin, _fLightningRangeFromWeatherMasterMax);

            float fRandZPosition;
            fRandZPosition = Random.Range(_fLightningRangeFromWeatherMasterMin, _fLightningRangeFromWeatherMasterMin);

            _gLighting.transform.position = new Vector3(fRandXPosition, _fLightningHeight, fRandZPosition);

            if (_bUsingSound == true)
            {
                if (_adThunderSound != null)
                {
                    _gLighting.GetComponent<AudioSource>().clip = _adThunderSound;
                    _gLighting.GetComponent<AudioSource>().Play();
                }
                else
                    Debug.Log("You have no thunder sound attached to: " + this.gameObject + " Is this by choice?");
            }

            StartCoroutine(ControlLightingLightOnOff());

            _fTimerForNextLighting = 0.0f;
            _fTimeForNextLighting = Random.Range(_fRandomThunderTimeMin, _fRandomThunderTimeMax);
        }
    }

    IEnumerator ControlLightingLightOnOff()
    {
        if (_gLighting != null)
        {
            _gLighting.GetComponent<Light>().intensity = _fLightningIntensity;
            yield return new WaitForSeconds(_fTimeBeforeLightningLightTurnsOff);
        }

        if (_gLighting != null)
        {
            _gLighting.GetComponent<Light>().intensity = 0.0f;
            yield return null;
        }
    }

    private void TurnOnRain()
    {
        if (_gPartRain != null)
        {
            if (_gPartRain.activeInHierarchy == false && _gPartRain != null)
            {
                _gPartRain.SetActive(true);

                if (_bUsingSound == true)
                    TurnOnSound(_gPartRain);
            }
        }
        else
            Debug.Log("We are missing rain particles on: " + this.gameObject + " For weather type: THUNDERSTORM");
    }

    public override void TurnOnSound(GameObject gameobject)
    {
        base.TurnOnSound(gameobject);

        // Check to tell that we should start fading the sound out when exiting this weather function
        _bTurnOffSoundAtExit = true;
    }

    /// <summary>
    /// How this works per rain type
    /// 1. I activate a timer since I want it to reset and not start again after deactivateing the particlesystem
    /// 2. Turn off emission so the rain stops falling before we turn off the particle
    /// 3. When exit start fading sound out if this is true
    /// 3.1. Set this to false again so we can turn on the sound again when this weather effect happens again
    /// 4. When this timer ends turn off the active rain
    /// 4.1. Set back start timer to 0.0f
    /// 4.2. Deactivate the particle system
    /// </summary>
    public override void ExitWeatherEffect(GameObject gameobject)
    {
        clWeatherController.DeactivateTimesetParticle(_pSunriseParticle);
        clWeatherController.DeactivateTimesetParticle(_pDayParticle);
        clWeatherController.DeactivateTimesetParticle(_pSunsetParticle);
        clWeatherController.DeactivateTimesetParticle(_pNightParticle);

        if (_gPartRain != null && _gPartRain.activeInHierarchy == true)
        {
            _fEndParticleTimerStart += Time.deltaTime;
            ParticleSystem.EmissionModule em = _gPartRain.GetComponent<ParticleSystem>().emission;
            em.enabled = false;

            _fTimerForNextLighting = 0.0f;
            _fTimeForNextLighting = 0.0f;

            if (_gLighting != null)
                Destroy(_gLighting);

            if (_bTurnOffSoundAtExit == true)
            {
                if (_bUsingSound == true)
                    base.ExitWeatherEffect(gameobject);

                _bTurnOffSoundAtExit = false;
            }

            if (_fEndParticleTimerStart > _fEndParticleTimerEnd)
            {
                _fEndParticleTimerStart = 0.0f;
                _gPartRain.SetActive(false);
            }
        }
    }
}