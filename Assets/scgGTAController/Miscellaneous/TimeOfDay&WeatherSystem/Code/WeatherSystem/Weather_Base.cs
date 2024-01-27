using UnityEngine;
using System.Collections;

public class Weather_Base : MonoBehaviour 
{
    /********** ----- VARIABLES ----- **********/

    /********** GENERAL Settings **********/

    /// <summary>
    /// We use this so we can get access to the Weather controller from all our child scripts
    /// </summary>
    protected Weather_Controller clWeatherController;

    /// <summary>
    /// Choose if you want to use different fade time for the different times set or the same for all of them 
    /// </summary>
    [SerializeField]
    protected bool _bUseDifferentFadeTimes;

    /// <summary>
    /// How long do we want it to take for all our weather settings to change into the finished result\n
    /// *Use \link GetSet_fFadeTime \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fFadeTime = 5.0f;

    /// <summary>
    /// How long do we want it to take for our day settings to change into the finished result\n
    /// *Use \link GetSet_fSunriseFadeTime \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fSunriseFadeTime = 5.0f;

    /// <summary>
    /// How long do we want it to take for our day settings to change into the finished result\n
    /// *Use \link GetSet_fDayFadeTime \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fDayFadeTime = 5.0f;

    /// <summary>
    /// How long do we want it to take for our sunset settings to change into the finished result\n
    /// *Use \link GetSet_fSunsetFadeTime \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fSunsetFadeTime = 5.0f;

    /// <summary>
    /// How long do we want it to take for our night settings to change into the finished result\n
    /// *Use \link GetSet_fNightFadeTime \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fNightFadeTime = 5.0f;

    /// <summary>
    /// We turn this to true when we want to start the Init() function.\n
    /// *Use \link GetSet_bUseInit \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected bool _bUseInit;

    protected float _fInitTimerStart = 0.0f;

    /// <summary>
    /// This is the float where the designer choose when Init() should start\n
    /// *Use \link GetSet_fInitTimerEnd \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fInitTimerEnd = 5.0f;

    /********** SOUND Settings **********/

    /// <summary>
    /// I use this to make sure that the object that is supposed to play the sound got the AudioSource component. If it does, this will be set to true. 
    /// </summary>
    protected bool _bGotAudioSource;

    /// <summary>
    /// Does the user want this weather effect to have sound?\n
    /// *Use \link GetSet_bUsingSound \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected bool _bUsingSound;

    /// <summary>
    /// If the user want to use sound on the weather effect, this is where the AudioClip should be placed.\n
    /// *Use \link GetSet_adAmbientSound \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected AudioClip _adAmbientSound;

    /// <summary>
    /// We use as security so the sound only get's turned on and off once
    /// </summary>
    protected bool _bTurnOffSoundAtExit;

    /// <summary>
    /// How loud does the user want the sound to be att full volume.\n
    /// *Use \link GetSet_fSoundVolume \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fSoundVolume = 1.0f;

    /// <summary>
    /// We use these for the fading in and out of the sound
    /// </summary>
    protected float _fSoundVolumeIn;

    /// <summary>
    /// We use these for the fading in and out of the sound
    /// </summary>
    protected float _fSoundVolumeOut;

    /// <summary>
    /// How long should it take for the sound to reach the users set volume?\n
    /// *Use \link GetSet_fTimeToFadeSound \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fTimeToFadeSound = 5.0f;

    /********** TEMPRATURE Settings **********/

    // TEMPRATURE
    /// <summary>
    /// What's the lowest temprature we can have during this weather type?\n
    /// *Use \link GetSet_fLowestTemp \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fLowestTemp = 0.0f;

    /// <summary>
    /// What's the highest temprature we can have during this weather type?\n
    /// *Use \link GetSet_fHighestTemp \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fHighestTemp = 30.0f;

    /********** SUN (Light) Settings **********/

    // LIGHT
    /// <summary>
    /// Here we save the value for our SUN's light intensity during SUNRISE\n
    /// *Use \link GetSet_fSunriseLightIntensity \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fSunrise_LightIntensity = 0.5f;

    /// <summary>
    /// Here we save the value for our SUN's light intensity during DAY\n
    /// *Use \link GetSet_fDay_LightIntensity \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fDay_LightIntensity = 1.0f;

    /// <summary>
    /// Here we save the value for our SUN's light intensity during SUNSET\n
    /// *Use \link GetSet_fSunset_LightIntensity \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fSunset_LightIntensity = 0.5f;

    /// <summary>
    /// Here we save the value for our SUN's light intensity during NIGHT\n
    /// *Use \link GetSet_fNight_LightIntensity \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fNight_LightIntensity = 0.3f;

    // SUN LIGHT COLOR
    /// <summary>
    /// Here we save the value for our SUN's light color during SUNRISE\n
    /// *Use \link GetSet_cSunrise_LightColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cSunrise_LightColor = Color.yellow;

    /// <summary>
    /// Here we save the value for our SUN's light color during DAY\n
    /// *Use \link GetSet_cDay_LightColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cDay_LightColor = Color.cyan;

    /// <summary>
    /// Here we save the value for our SUN's light color during SUNSET\n
    /// *Use \link GetSet_cSunset_LightColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cSunset_LightColor = Color.red;

    /// <summary>
    /// Here we save the value for our SUN's light color during NIGHT\n
    /// *Use \link GetSet_cNight_LightColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cNight_LightColor = Color.grey;

    /********** MOON (Light) Settings **********/

    // Moon settings
    /// <summary>
    /// Here we save the value for our MOON's light intensity during NIGHT\n
    /// *Use \link GetSet_fNight_MoonLightIntensity \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fNight_MoonLightIntensity = 0.5f;

    /// <summary>
    /// Here we save the value for our MOON's light color during NIGHT\n
    /// *Use \link GetSet_cNight_MoonLightColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cNight_MoonLightColor = Color.grey;

    /********** SKYBOX Settings **********/

    // Skybox
    /// <summary>
    /// Here we save the value for our Skybox color during SUNRISE\n
    /// *Use \link GetSet_cSunrise_SkyTintColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cSunrise_SkyTintColor = Color.yellow;

    /// <summary>
    /// Here we save the value for our Skybox color during DAY\n
    /// *Use \link GetSet_cDay_SkyTintColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cDay_SkyTintColor = Color.cyan;

    /// <summary>
    /// Here we save the value for our Skybox color during SUNSET\n
    /// *Use \link GetSet_cSunset_SkyTintColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cSunset_SkyTintColor = Color.red;

    /// <summary>
    /// Here we save the value for our Skybox color during NIGHT\n
    /// *Use \link GetSet_cNight_SkyTintColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cNight_SkyTintColor = Color.grey;

    /* ***** ONLY USED IF WE HAVE A PROCEDURAL SKYBOX ***** */
    /// <summary>
    /// Here we save the value for our Skybox ground color during SUNRISE\n
    /// *Use \link GetSet_cSunrise_SkyGroundColor \endlink if you want to change it during runtime.\n
    /// **This is only used if the Procedural Shader is used for the Skybox
    /// </summary>
    [SerializeField]
    protected Color _cSunrise_SkyGroundColor = Color.yellow;

    /// <summary>
    /// Here we save the value for our Skybox ground color during DAY\n
    /// *Use \link GetSet_cDay_SkyGroundColor \endlink if you want to change it during runtime.\n
    /// **This is only used if the Procedural Shader is used for the Skybox
    /// </summary>
    [SerializeField]
    protected Color _cDay_SkyGroundColor = Color.cyan;

    /// <summary>
    /// Here we save the value for our Skybox ground color during SUNSET\n
    /// *Use \link GetSet_cSunset_SkyGroundColor \endlink if you want to change it during runtime.\n
    /// **This is only used if the Procedural Shader is used for the Skybox
    /// </summary>
    [SerializeField]
    protected Color _cSunset_SkyGroundColor = Color.red;

    /// <summary>
    /// Here we save the value for our Skybox ground color during NIGHT\n
    /// *Use \link GetSet_cNight_SkyGroundColor \endlink if you want to change it during runtime.\n
    /// **This is only used if the Procedural Shader is used for the Skybox
    /// </summary>
    [SerializeField]
    protected Color _cNight_SkyGroundColor = Color.grey;

    /********** CLOUD Settings **********/

    /// <summary>
    /// Here we save the value for our Cloud color during the different weather effects\n
    /// *Use \link GetSet_cCloudColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cCloudColor = Color.white;

    /********** FOG Settings **********/

    /// <summary>
    /// Here we save the value for our Fog amount during the different weather effects\n
    /// *Use \link GetSet_fFogAmount \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fFogAmount = 0.05f;

    /// <summary>
    /// Here we save the value if the user want to use more fog durning morning\n
    /// *Use \link GetSet_fFogMorning \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected float _fFogMorningAmount = 0.05f;

    /// <summary>
    /// Do we want to have extra fog in the morning\n
    /// *Use \link GetSet_bUseMorningFog \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected bool _bUseMorningFog = false;

    /// <summary>
    /// Here we save the value for our Fog amount during the different weather effects\n
    /// *Use \link GetSet_cFogColor \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected Color _cFogColor = Color.white;

    /********** TIMESET PARTICLES Settings **********/

    /// <summary>
    /// Here we add the SUNRISE particle\n
    /// *Use \link GetSet_pSunriseParticle \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected GameObject _pSunriseParticle = null;

    /// <summary>
    /// Here we add the SUNRISE particle\n
    /// *Use \link GetSet_pDayParticle \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected GameObject _pDayParticle = null;

    /// <summary>
    /// Here we add the SUNRISE particle\n
    /// *Use \link GetSet_pSunsetParticle \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected GameObject _pSunsetParticle = null;

    /// <summary>
    /// Here we add the SUNRISE particle\n
    /// *Use \link GetSet_pNightParticle \endlink if you want to change it during runtime.
    /// </summary>
    [SerializeField]
    protected GameObject _pNightParticle = null;

    /********** ----- GETTERS AND SETTERS ----- **********/

    /********** GENERAL **********/

    public bool GetSet_bUseDifferentFadeTimes
    {
        get { return _bUseDifferentFadeTimes; }
        set { _bUseDifferentFadeTimes = value; }
    }

    public float GetSet_fFadeTime
    {
        get { return _fFadeTime; }
        set { _fFadeTime = value; }
    }

    public float GetSet_fSunriseFadeTime
    {
        get { return _fSunriseFadeTime; }
        set { _fSunriseFadeTime = value; }
    }

    public float GetSet_fDayFadeTime
    {
        get { return _fDayFadeTime; }
        set { _fDayFadeTime = value; }
    }

    public float GetSet_fSunsetFadeTime
    {
        get { return _fSunsetFadeTime; }
        set { _fSunsetFadeTime = value; }
    }

    public float GetSet_fNightFadeTime
    {
        get { return _fNightFadeTime; }
        set { _fNightFadeTime = value; }
    }

    public bool GetSet_bUseInit
    {
        get { return _bUseInit; }
        set { _bUseInit = value; }
    }

    public float GetSet_fInitTimerEnd
    {
        get { return _fInitTimerEnd; }
        set { _fInitTimerEnd = value; }
    }

    /********** SOUND **********/

    public bool GetSet_bUsingSound
    {
        get { return _bUsingSound; }
        set { _bUsingSound = value; }
    }

    public AudioClip GetSet_adAmbientSound
    {
        get { return _adAmbientSound; }
        set { _adAmbientSound = value; }
    }

    public float GetSet_fSoundVolume
    {
        get { return _fSoundVolume; }
        set { _fSoundVolume = value; }
    }

    public float GetSet_fTimeToFadeSound
    {
        get { return _fTimeToFadeSound; }
        set { _fTimeToFadeSound = value; }
    }

    /********** TEMPRATURE **********/

    public float GetSet_fLowestTemp
    {
        get { return _fLowestTemp; }
        set { _fLowestTemp = value; }
    }

    public float GetSet_fHighestTemp
    {
        get { return _fHighestTemp; }
        set { _fHighestTemp = value; }
    }

    /********** SUN (Light) **********/

    public float GetSet_fSunriseLightIntensity
    {
        get { return _fSunrise_LightIntensity; }
        set { _fSunrise_LightIntensity = value; }
    }

    public float GetSet_fDay_LightIntensity
    {
        get { return _fDay_LightIntensity; }
        set { _fDay_LightIntensity = value; }
    }

    public float GetSet_fSunset_LightIntensity
    {
        get { return _fSunset_LightIntensity; }
        set { _fSunset_LightIntensity = value; }
    }

    public float GetSet_fNight_LightIntensity
    {
        get { return _fNight_LightIntensity; }
        set { _fNight_LightIntensity = value; }
    }

    public Color GetSet_cSunrise_LightColor
    {
        get { return _cSunrise_LightColor; }
        set { _cSunrise_LightColor = value; }
    }

    public Color GetSet_cDay_LightColor
    {
        get { return _cDay_LightColor; }
        set { _cDay_LightColor = value; }
    }

    public Color GetSet_cSunset_LightColor
    {
        get { return _cSunset_LightColor; }
        set { _cSunset_LightColor = value; }
    }

    public Color GetSet_cNight_LightColor
    {
        get { return _cNight_LightColor; }
        set { _cNight_LightColor = value; }
    }

    /********** MOON (Light) **********/

    public float GetSet_fNight_MoonLightIntensity
    {
        get { return _fNight_MoonLightIntensity; }
        set { _fNight_MoonLightIntensity = value; }
    }

    public Color GetSet_cNight_MoonLightColor
    {
        get { return _cNight_MoonLightColor; }
        set { _cNight_MoonLightColor = value; }
    }

    /********** SKYBOX **********/

    public Color GetSet_cSunrise_SkyTintColor
    {
        get { return _cSunrise_SkyTintColor; }
        set { _cSunrise_SkyTintColor = value; }
    }

    public Color GetSet_cDay_SkyTintColor
    {
        get { return _cDay_SkyTintColor; }
        set { _cDay_SkyTintColor = value; }
    }

    public Color GetSet_cSunset_SkyTintColor
    {
        get { return _cSunset_SkyTintColor; }
        set { _cSunset_SkyTintColor = value; }
    }

    public Color GetSet_cNight_SkyTintColor
    {
        get { return _cNight_SkyTintColor; }
        set { _cNight_SkyTintColor = value; }
    }

    public Color GetSet_cSunrise_SkyGroundColor
    {
        get { return _cSunrise_SkyGroundColor; }
        set { _cSunrise_SkyGroundColor = value; }
    }

    public Color GetSet_cDay_SkyGroundColor
    {
        get { return _cDay_SkyGroundColor; }
        set { _cDay_SkyGroundColor = value; }
    }

    public Color GetSet_cSunset_SkyGroundColor
    {
        get { return _cSunset_SkyGroundColor; }
        set { _cSunset_SkyGroundColor = value; }
    }

    public Color GetSet_cNight_SkyGroundColor
    {
        get { return _cNight_SkyGroundColor; }
        set { _cNight_SkyGroundColor = value; }
    }

    /********** CLOUD **********/

    public Color GetSet_cCloudColor
    {
        get { return _cCloudColor; }
        set { _cCloudColor = value; }
    }

    /********** FOG **********/

    public float GetSet_fFogAmount
    {
        get { return _fFogAmount; }
        set { _fFogAmount = value; }
    }

    public float GetSet_fFogMorning
    {
        get { return _fFogMorningAmount; }
        set { _fFogMorningAmount = value; }
    }

    public bool GetSet_bUseMorningFog
    {
        get { return _bUseMorningFog; }
        set { _bUseMorningFog = value; }
    }

    public Color GetSet_cFogColor
    {
        get { return _cFogColor; }
        set { _cFogColor = value; }
    }

    /********** PARTICLES **********/

    public GameObject GetSet_pSunriseParticle
    {
        get { return _pSunriseParticle; }
        set { _pSunriseParticle = value; }
    }
    public GameObject GetSet_pDayParticle
    {
        get { return _pDayParticle; }
        set { _pDayParticle = value; }
    }

    public GameObject GetSet_pSunsetParticle
    {
        get { return _pSunsetParticle; }
        set { _pSunsetParticle = value; }
    }

    public GameObject GetSet_pNightParticle
    {
        get { return _pNightParticle; }
        set { _pNightParticle = value; }
    }


    /********** ----- FUNCTIONS ----- **********/

    /// <summary>
    /// Our own initilation function we use as start only is called once in every function.
    /// </summary>
    public virtual void Init() 
    {
        float CurrentTemprature = Random.Range(_fLowestTemp, _fHighestTemp);
        this.GetComponent<Weather_Controller>().GetSet_fCurrTemp = CurrentTemprature;
    }
	/// <summary>
	/// Our weather update function. This is the function we call that update all our different weather changes, 
	/// lights, colors and so on. 
	/// </summary>
    public virtual void UpdateWeather() { }

    /// <summary>
    /// If we use a special weather sound this is the function were we turn it on. 
    /// </summary>
    public virtual void TurnOnSound(GameObject gameobject) 
    {
        if (gameobject != null)
        {
            if (gameobject.GetComponent<Weather_SoundFade>() != null && _bGotAudioSource == true)
            {
                gameobject.GetComponent<Weather_SoundFade>().FadeAudioIn(_fTimeToFadeSound, _fSoundVolumeIn);
                gameobject.GetComponent<AudioSource>().Play();
            }
            else if (gameobject.GetComponent<Weather_SoundFade>() == null && _bGotAudioSource == true)
            {
                Debug.LogError(gameobject.gameObject + " is missing the Weather_SoundFade component, we added it now");
                gameobject.gameObject.AddComponent<Weather_SoundFade>();
                gameobject.GetComponent<Weather_SoundFade>().FadeAudioIn(_fTimeToFadeSound, _fSoundVolumeIn);
                gameobject.GetComponent<AudioSource>().Play();
            }
            else
                Debug.Log("We got no audiosource for: " + gameobject);
        }
        else
            Debug.Log("There is no SoundEffect Gameobject attached to the WeatherController on type: " + clWeatherController.en_CurrWeather + " If you don't want to use sound on this weather type, set Using Ambient Sound to false!");
    }

	/// <summary>
	/// Exits the weather effect. 
	/// This is used so before we go into a new weather type we make sure to turn off everything and such that we 
	/// need in the current weather. 
	/// </summary>
    public virtual void ExitWeatherEffect(GameObject gameobject) 
    {
        // This is where we turn off the sound if we have it
        if (gameobject.GetComponent<Weather_SoundFade>() != null && _bGotAudioSource == true)
            gameobject.GetComponent<Weather_SoundFade>().FadeAudioOut(_fTimeToFadeSound, _fSoundVolumeOut);
        else
            Debug.LogError(gameobject + " is missing the Weather_SoundFade component!");
    }
}