using UnityEngine;
using System.Collections;

public class Weather_Controller : MonoBehaviour 
{
    /********** ----- VARIABLES ----- **********/

    // We only have a GET for this as this is only used for debugging
    private bool _bChangeWeather;

    // Int with the new weather if we get a new type
    private int _iNewWeather;
    private bool _bStartWeatherChange;
    private float _fTimeChangeWeatherStart;
    private float _fTimeChangeWeatherEnd;

    // Procedural skybox
    [SerializeField]
    private bool _bUsingProceduralSkybox;

    // Weather change
    [SerializeField]
    private bool _bUseSun = true;

    [SerializeField]
    private bool _bUseCloudy = true;

    [SerializeField]
    private bool _bUseRain = true;

    [SerializeField]
    private bool _bUseThunderstorm = true;

    [SerializeField]
    private bool _bUseSnow = true;

    [SerializeField]
    private bool _bUseRandomWeather = true;

    [SerializeField]
    private bool _bUseRandomDaysWeather;

    private int _iAmountOfDaysToNewWeather;

    // * Does not need SerializeField as this won't change from editor
    private int _iAmountOfDaysSinceLastWeather;

    [SerializeField]
    private int _iChangeWeatherAfterDays = 4;

    [SerializeField]
    private int _iMinAmountOfDaysToNewWeather = 1;

    [SerializeField]
    private int _iMaxAmountOfDaysToNewWeather = 10;

    private float _fCurrTemp;

    public GameObject gTimeOfDay;
    public Material matClouds;
    public Material matSkybox;

    public bool setFogToSkyColor;
    public RenderTexture fogBlendingTexture;

    public enum WeatherType
    {
        RANDOM,
        SUN,
        CLOUDY,
        RAIN,
        THUNDERSTORM,
        SNOW,
        NUMBEROFWEATHERTYPES
    };

    public WeatherType en_CurrWeather;

    [HideInInspector]
    public WeatherType en_LastWeather;

    /********** ----- GETTERS AND SETTERS ----- **********/

    public bool Get_bChangeWeather { get { return _bChangeWeather; } }
    public bool Get_bStartWeatherChange { get { return _bStartWeatherChange; } }
    public float Get_fTimeChangeWeatherStart { get { return _fTimeChangeWeatherStart; } }
    public int Get_iAmountOfDaysToNewWeather { get { return _iAmountOfDaysToNewWeather; } }

    public bool GetSet_bUsingProceduralSkybox
    {
        get { return _bUsingProceduralSkybox; }
        set { _bUsingProceduralSkybox = value; }
    }

    public bool GetSet_bUseSun
    {
        get { return _bUseSun; }
        set { _bUseSun = value; }
    }

    public bool GetSet_bUseCloudy
    {
        get { return _bUseCloudy; }
        set { _bUseCloudy = value; }
    }

    public bool GetSet_bUseRain
    {
        get { return _bUseRain; }
        set { _bUseRain = value; }
    }

    public bool GetSet_bUseThunderstorm
    {
        get { return _bUseThunderstorm; }
        set { _bUseThunderstorm = value; }
    }

    public bool GetSet_bUseSnow
    {
        get { return _bUseSnow; }
        set { _bUseSnow = value; }
    }

    public bool GetSet_bUseRandomWeather
    {
        get { return _bUseRandomWeather; }
        set { _bUseRandomWeather = value; }
    }

    public bool GetSet_bUseRandomDaysWeather
    {
        get { return _bUseRandomDaysWeather; }
        set { _bUseRandomDaysWeather = value; }
    }

    public int GetSet_iChangeWeatherAfterDays
    {
        get { return _iChangeWeatherAfterDays; }
        set { _iChangeWeatherAfterDays = value; }
    }

    public int GetSet_iAmountOfDaysSinceLastWeather
    {
        get { return _iAmountOfDaysSinceLastWeather; }
        set { _iAmountOfDaysSinceLastWeather = value; }
    }

    public int GetSet_iMinAmountOfDaysToNewWeather
    {
        get { return _iMinAmountOfDaysToNewWeather; }
        set { _iMinAmountOfDaysToNewWeather = value; }
    }

    public int GetSet_iMaxAmountOfDaysToNewWeather
    {
        get { return _iMaxAmountOfDaysToNewWeather; }
        set { _iMaxAmountOfDaysToNewWeather = value; }
    }

    public float GetSet_fCurrTemp
    {
        get { return _fCurrTemp; }
        set { _fCurrTemp = value; }
    }

	void Start () 
    {
        _fTimeChangeWeatherStart = 0.0f;
        _fTimeChangeWeatherEnd = 5.0f;

        if (_bUseRandomDaysWeather == true)
            _iAmountOfDaysToNewWeather = Random.Range(_iMinAmountOfDaysToNewWeather, _iMaxAmountOfDaysToNewWeather);
        else
            _iAmountOfDaysToNewWeather = _iChangeWeatherAfterDays;

        if (en_CurrWeather == WeatherType.RANDOM && _bUseRandomWeather == true || en_CurrWeather == WeatherType.NUMBEROFWEATHERTYPES && _bUseRandomWeather == true)
            PickRandomWeather();
        else if (en_CurrWeather == WeatherType.RANDOM && _bUseRandomWeather == false || en_CurrWeather == WeatherType.NUMBEROFWEATHERTYPES && _bUseRandomWeather == false)
        {
            Debug.LogWarning("You haven't picked which weather to use, we default to SUN if the weather type is still set to \"RANDOM or NUMBEROFWEATHERTYPES\" and \"Use random weather\" is off");
            ExitCurrentWeather((int)WeatherType.SUN);
        }
        else
            EnterNewWeather((int)en_CurrWeather);
	}

	void Update () 
    {
        if (_bUseRandomWeather == true)
        {
            if (_bUseRandomDaysWeather == true)
            {
                if (_iAmountOfDaysSinceLastWeather >= _iAmountOfDaysToNewWeather)
                {
                    _bChangeWeather = true;
                    _iAmountOfDaysSinceLastWeather = 0;

                    _iAmountOfDaysToNewWeather = Random.Range(_iMinAmountOfDaysToNewWeather, _iMaxAmountOfDaysToNewWeather);
                }
            }
            else
            {
                if (_iAmountOfDaysSinceLastWeather >= _iChangeWeatherAfterDays)
                {
                    _bChangeWeather = true;
                    _iAmountOfDaysSinceLastWeather = 0;
                }
            }
        }

        if (_bChangeWeather == true)
            PickRandomWeather();

        if (_bStartWeatherChange == true)
            ExitCurrentWeather(_iNewWeather);
	}

    private void PickRandomWeather()
    {
        int Weather;
        Weather = Random.Range(1, (int)WeatherType.NUMBEROFWEATHERTYPES);

        if (Weather != (int)en_CurrWeather)
            CheckIfWeatherTypeIsOn(Weather);
        else
            Debug.Log("We got the same weather no change will happen!");

        // Set change weather bool so we don't try to change more then once!
        _bChangeWeather = false;
    }

    void CheckIfWeatherTypeIsOn(int NewWeatherType)
    {
        if (NewWeatherType == (int)WeatherType.SUN && _bUseSun != false)
        {
            _iNewWeather = NewWeatherType;
            _bStartWeatherChange = true;
        }
        else if (NewWeatherType == (int)WeatherType.CLOUDY && _bUseCloudy != false)
        {
            _iNewWeather = NewWeatherType;
            _bStartWeatherChange = true;
        }
        else if (NewWeatherType == (int)WeatherType.RAIN && _bUseRain != false)
        {
            _iNewWeather = NewWeatherType;
            _bStartWeatherChange = true;
        }
        else if (NewWeatherType == (int)WeatherType.THUNDERSTORM && _bUseThunderstorm != false)
        {
            _iNewWeather = NewWeatherType;
            _bStartWeatherChange = true;
        }
        else if (NewWeatherType == (int)WeatherType.SNOW && _bUseSnow != false)
        {
            _iNewWeather = NewWeatherType;
            _bStartWeatherChange = true;
        }
        else
        {
            Debug.Log("Weather type was not on, so we are trying again!");
            PickRandomWeather();
        }
    }

    void ChangeWeatherToSun()
    {
        en_CurrWeather = WeatherType.SUN;
        this.GetComponent<Weather_Sun>().enabled = true;
        this.GetComponent<Weather_Sun>().GetSet_bUseInit = true;
    }

    void ChangeWeatherToCloudy()
    {
        en_CurrWeather = WeatherType.CLOUDY;
        this.GetComponent<Weather_Cloudy>().enabled = true;
        this.GetComponent<Weather_Cloudy>().GetSet_bUseInit = true;
    }

    void ChangeWeatherToRain()
    {
        en_CurrWeather = WeatherType.RAIN;
        this.GetComponent<Weather_Rain>().enabled = true;
        this.GetComponent<Weather_Rain>().GetSet_bUseInit = true;
    }

    void ChangeWeatherToThunderstorm()
    {
        en_CurrWeather = WeatherType.THUNDERSTORM;
        this.GetComponent<Weather_Thunderstorm>().enabled = true;
        this.GetComponent<Weather_Thunderstorm>().GetSet_bUseInit = true;
    }

    void ChangeWeatherToSnow()
    {
        en_CurrWeather = WeatherType.SNOW;
        this.GetComponent<Weather_Snow>().enabled = true;
        this.GetComponent<Weather_Snow>().GetSet_bUseInit = true;
    }

    void ExitCurrentWeather(int NewWeatherType)
    {
        if (en_CurrWeather == WeatherType.RANDOM)
        {
            // Set our last weather type
            en_LastWeather = WeatherType.RANDOM;

            // Enter new weather type
            EnterNewWeather(NewWeatherType);

            // Reset values for changing weather
            _fTimeChangeWeatherStart = 0.0f;
            _bStartWeatherChange = false;
        }
        else if (en_CurrWeather == WeatherType.SUN)
        {
            en_LastWeather = WeatherType.SUN;
            _fTimeChangeWeatherStart += Time.deltaTime;

            // Call the exit function in the weather type
            this.GetComponent<Weather_Sun>().ExitWeatherEffect(this.GetComponent<Weather_Sun>().GetSet_gSoundEffect);

            // After we reached or set time, we start to change into the new weather type
            if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
            {
                this.GetComponent<Weather_Sun>().enabled = false;
                EnterNewWeather(NewWeatherType);

                _fTimeChangeWeatherStart = 0.0f;
                _bStartWeatherChange = false;
            }
        }
        else if (en_CurrWeather == WeatherType.CLOUDY)
        {
            en_LastWeather = WeatherType.CLOUDY;
            _fTimeChangeWeatherStart += Time.deltaTime;

            // Call the exit function in the weather type
            this.GetComponent<Weather_Cloudy>().ExitWeatherEffect(this.GetComponent<Weather_Cloudy>().GetSet_gSoundEffect);

            // After we reached or set time, we start to change into the new weather type
            if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
            {
                this.GetComponent<Weather_Cloudy>().enabled = false;
                EnterNewWeather(NewWeatherType);

                _fTimeChangeWeatherStart = 0.0f;
                _bStartWeatherChange = false;
            }
        }
        else if (en_CurrWeather == WeatherType.RAIN)
        {
            en_LastWeather = WeatherType.RAIN;
            _fTimeChangeWeatherStart += Time.deltaTime;

            // Call the exit function in the weather type
            this.GetComponent<Weather_Rain>().ExitWeatherEffect(this.GetComponent<Weather_Rain>().GetSet_gPartRain);

            // After we reached or set time, we start to change into the new weather type

            if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
            {
                this.GetComponent<Weather_Rain>().enabled = false;
                EnterNewWeather(NewWeatherType);

                _fTimeChangeWeatherStart = 0.0f;
                _bStartWeatherChange = false;
            }
        }
        else if (en_CurrWeather == WeatherType.THUNDERSTORM)
        {
            en_LastWeather = WeatherType.THUNDERSTORM;
            _fTimeChangeWeatherStart += Time.deltaTime;

            // Call the exit function in the weather type
            this.GetComponent<Weather_Thunderstorm>().ExitWeatherEffect(this.GetComponent<Weather_Thunderstorm>().GetSet_gPartRain);

            // After we reached or set time, we start to change into the new weather type
            if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
            {
                this.GetComponent<Weather_Thunderstorm>().enabled = false;
                EnterNewWeather(NewWeatherType);

                _fTimeChangeWeatherStart = 0.0f;
                _bStartWeatherChange = false;
            }
        }
        else if (en_CurrWeather == WeatherType.SNOW)
        {
            en_LastWeather = WeatherType.SNOW;
            _fTimeChangeWeatherStart += Time.deltaTime;

            // Call the exit function in the weather type
            this.GetComponent<Weather_Snow>().ExitWeatherEffect(this.GetComponent<Weather_Snow>().GetSet_gPartSnow);

            // After we reached or set time, we start to change into the new weather type
            if (_fTimeChangeWeatherStart >= _fTimeChangeWeatherEnd)
            {
                this.GetComponent<Weather_Snow>().enabled = false;
                EnterNewWeather(NewWeatherType);

                _fTimeChangeWeatherStart = 0.0f;
                _bStartWeatherChange = false;
            }
        }
    }

    private void EnterNewWeather(int NewWeather)
    {
        if (NewWeather == (int)WeatherType.SUN)
            ChangeWeatherToSun();
        else if (NewWeather == (int)WeatherType.CLOUDY)
            ChangeWeatherToCloudy();
        else if (NewWeather == (int)WeatherType.RAIN)
            ChangeWeatherToRain();
        else if (NewWeather == (int)WeatherType.THUNDERSTORM)
            ChangeWeatherToThunderstorm();
        else if (NewWeather == (int)WeatherType.SNOW)
            ChangeWeatherToSnow();
    }

    /// <summary>
    /// This is our function that controls all the changes to the weather. 
    /// </summary>
    /// <param name="sunIntensity">Sun intensity</param>
    /// <param name="sunLightColor">Sun light color</param>
    /// <param name="skyTint">Sky tint</param>
    /// <param name="skyGround">Sky ground *This is only used for the procedural skybox!</param>
    /// <param name="cloudColor">Cloud color</param>
    /// <param name="fogDensity">Fog density</param>
    /// <param name="fogColor">Fog color</param>
    /// <param name="fadeTime">Fade time</param>
    public void UpdateAllWeather(float sunIntensity, Color sunLightColor, float moonIntensity, Color moonLightColor, Color skyTint, Color skyGround, Color cloudColor, float fogDensity, Color fogColor, float fadeTime)
    {
        // Light (Directional light, "SUN") settings
        // * We call the sunlight from our TimeOfDay script as this controls most of the sun
        gTimeOfDay.GetComponent<ToD_Base>().lSun.intensity = Mathf.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lSun.intensity, sunIntensity, Time.deltaTime / fadeTime);
        gTimeOfDay.GetComponent<ToD_Base>().lSun.color = Color.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lSun.color, sunLightColor, Time.deltaTime / fadeTime);

        // Moon light
        if (gTimeOfDay.GetComponent<ToD_Base>().GetSet_bUseMoon == true)
        {
            gTimeOfDay.GetComponent<ToD_Base>().lMoon.intensity = Mathf.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lMoon.intensity, moonIntensity, Time.deltaTime / fadeTime);
            gTimeOfDay.GetComponent<ToD_Base>().lMoon.color = Color.Lerp(gTimeOfDay.GetComponent<ToD_Base>().lMoon.color, moonLightColor, Time.deltaTime / fadeTime);
        }

        // Skybox settings
        if (_bUsingProceduralSkybox == false)
            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), skyTint, Time.deltaTime / fadeTime));
        else
        {
            RenderSettings.skybox.SetColor("_SkyTint", Color.Lerp(RenderSettings.skybox.GetColor("_SkyTint"), skyTint, Time.deltaTime / fadeTime));
            RenderSettings.skybox.SetColor("_GroundColor", Color.Lerp(RenderSettings.skybox.GetColor("_GroundColor"), skyGround, Time.deltaTime / fadeTime));
        }

        // Cloud settings
        if (matClouds != null)
            matClouds.color = Color.Lerp(matClouds.color, cloudColor, Time.deltaTime / fadeTime);
        else
            Debug.LogWarning("We have no cloud material attached to:" + this.gameObject);

        // Fog settings
        //RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, fogDensity, Time.deltaTime / fadeTime);

        if (!setFogToSkyColor)
        {
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColor, Time.deltaTime / fadeTime);
        }
        else
        {
            // Set the fog color to match the sky color

            // Set up a temporary RenderTexture and read the middle pixel color
            RenderTexture.active = fogBlendingTexture;

            // Create a temporary Texture2D
            Texture2D tempTexture = new Texture2D(fogBlendingTexture.width, fogBlendingTexture.height);

            tempTexture.ReadPixels(new Rect(0, 0, fogBlendingTexture.width, fogBlendingTexture.height), 0, 0);
            tempTexture.Apply();

            // Calculate the middle pixel position
            int middleX = fogBlendingTexture.width / 2;
            int middleY = fogBlendingTexture.height / 2;

            // Read the pixel color from the middle of the Texture2D
            Color middlePixelColor = tempTexture.GetPixel(middleX, middleY + 15);

            // Define the desaturation amount (between 0 and 1, where 0 means no desaturation and 1 means fully desaturated)
            float desaturationAmount = 0.15f; // Adjust this value as needed

            // Desaturate the middlePixelColor
            float grayscaleValue = middlePixelColor.grayscale;
            middlePixelColor = Color.Lerp(middlePixelColor, new Color(grayscaleValue, grayscaleValue, grayscaleValue, middlePixelColor.a), desaturationAmount);

            // Destroy the temporary Texture2D
            Destroy(tempTexture);

            // Reset active RenderTexture
            RenderTexture.active = null;

            RenderSettings.fogColor = middlePixelColor;
        }
    }

    /// <summary>
    /// Here we activate the timeset particle (same structrue for deactivating particles)
    /// Steps:
    /// 1. Check if the "Parent" has a particle system
    /// 2. If the parent have a particle system turn that on
    /// 3. If there are more child particle systems turn these on
    /// else
    /// 2. If the parent is NOT a particle system ignore that and go straight to the child objects
    /// 3. Turn these particle emitters on
    /// </summary>
    /// <param name="CurrParticles">Particle system</param>
    public void ActivateTimesetParticle(GameObject CurrParticles)
    {
        if (CurrParticles != null)
        {
            if (CurrParticles.gameObject.GetComponent<ParticleSystem>() != false)
            {
                if (CurrParticles.gameObject.GetComponent<ParticleSystem>().emission.enabled == false)
                {
                    ParticleSystem.EmissionModule em = CurrParticles.gameObject.GetComponent<ParticleSystem>().emission;
                    em.enabled = true;

                    if (CurrParticles.transform.childCount != 0)
                    {
                        for (int iii = 0; iii < CurrParticles.transform.childCount; ++iii)
                        {
                            ParticleSystem.EmissionModule em2 = CurrParticles.transform.GetChild(iii).gameObject.GetComponent<ParticleSystem>().emission;
                            em2.enabled = true;
                        }
                    }
                }
            }
            else
            {
                if (CurrParticles.transform.childCount != 0)
                {
                    for (int iii = 0; iii < CurrParticles.transform.childCount; ++iii)
                    {
                        ParticleSystem.EmissionModule em = CurrParticles.transform.GetChild(iii).gameObject.GetComponent<ParticleSystem>().emission;
                        em.enabled = true;
                    }
                }
            }

        }
    }

    /// <summary>
    /// Here we turn off the timeset particle from the previous timeset
    /// </summary>
    /// <param name="CurrParticles">Particle system</param>
    public void DeactivateTimesetParticle(GameObject CurrParticles)
    {
        if (CurrParticles != null)
        {
            if (CurrParticles.gameObject.GetComponent<ParticleSystem>() != false)
            {
                if (CurrParticles.gameObject.GetComponent<ParticleSystem>().emission.enabled == true)
                {
                    ParticleSystem.EmissionModule em = CurrParticles.gameObject.GetComponent<ParticleSystem>().emission;
                    em.enabled = false;

                    if (CurrParticles.transform.childCount != 0)
                    {
                        for (int iii = 0; iii < CurrParticles.transform.childCount; ++iii)
                        {
                            ParticleSystem.EmissionModule em2 = CurrParticles.transform.GetChild(iii).gameObject.GetComponent<ParticleSystem>().emission;
                            em2.enabled = false;
                        }
                    }
                }
            }
            else
            {
                if (CurrParticles.transform.childCount != 0)
                {
                    for (int iii = 0; iii < CurrParticles.transform.childCount; ++iii)
                    {
                        ParticleSystem.EmissionModule em = CurrParticles.transform.GetChild(iii).gameObject.GetComponent<ParticleSystem>().emission;
                        em.enabled = false;
                    }
                }
            }

        }
    }

    public void UseWeatherTypeDebug(int WeatherType)
    {
        if (WeatherType == 0)
            _bChangeWeather = true;
        else
        {
            _iNewWeather = WeatherType;
            _bStartWeatherChange = true;
        }
    }
}

