using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
[CustomEditor(typeof(Weather_Snow))]
public class WeatherSnow_Editor : Editor
{
    public bool bShowTips;
    public bool bUsingProcedural;

    private int iMinWidth = 30; // Was 30
    private int iMedWidth = 90; // Was 90
    private int iMaxWidth = 150; // Was 150

    public override void OnInspectorGUI()
    {
        DrawNewGUI();
        EditorUtility.SetDirty(target);
    }

    private void DrawNewGUI()
    {
        Weather_Snow cl = target as Weather_Snow;

        /* ----- TIPS SETTINGS ----- */
        GUILayout.BeginHorizontal();
        GUILayout.Label("Show more information");
        bShowTips = EditorGUILayout.Toggle(bShowTips, GUILayout.MaxWidth(iMinWidth));
        GUILayout.EndHorizontal();

        /* ----- PARTICLE SETTINGS ----- */
        EditorGUILayout.HelpBox(("Particle settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Add your snow particle prefab to this"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Snow particles (Gameobject): ");
        cl.GetSet_gPartSnow = EditorGUILayout.ObjectField("", cl.GetSet_gPartSnow, typeof(GameObject), true, GUILayout.MaxWidth(iMaxWidth)) as GameObject;
        GUILayout.EndHorizontal();

        /* ----- SOUND SETTINGS ----- */
        EditorGUILayout.HelpBox(("Sound settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("If you want a special sound to be played during this weather effect turn this on. \n\n*For weathereffects with particles we use the Particle Gameobject for the sound instead of having two gameobjects"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Weather sound:");
        cl.GetSet_bUsingSound = EditorGUILayout.Toggle(cl.GetSet_bUsingSound, GUILayout.MaxWidth(iMinWidth));
        GUILayout.EndHorizontal();

        if (cl.GetSet_bUsingSound == true)
        {
            if (bShowTips == true)
                EditorGUILayout.HelpBox(("Sound (AudioClip) - The audiofile with the sound you want to play \n\nThe fade timer is how fast you want the sound to fade in and out when it changes weather effect."), MessageType.Info, true);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Snow sound (AudioClip): ");
            cl.GetSet_adAmbientSound = EditorGUILayout.ObjectField("", cl.GetSet_adAmbientSound, typeof(AudioClip), true, GUILayout.MaxWidth(iMaxWidth)) as AudioClip;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sound to fade in and out (in sec.): ");
            cl.GetSet_fTimeToFadeSound = EditorGUILayout.FloatField("", cl.GetSet_fTimeToFadeSound, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Audio volume: ");
            cl.GetSet_fSoundVolume = EditorGUILayout.FloatField("", cl.GetSet_fSoundVolume, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();
        }

        /* ----- FADE TIME SETTINGS ----- */
        EditorGUILayout.HelpBox(("Fade effects settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set in seconds how long it should take for a timeset to reach it's finished settings (Sunrise, Day, Sunset, Night)"), MessageType.Info, true);


        GUILayout.BeginHorizontal();
        GUILayout.Label("Use different timeset fade times: ");
        cl.GetSet_bUseDifferentFadeTimes = EditorGUILayout.Toggle("", cl.GetSet_bUseDifferentFadeTimes, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        if (cl.GetSet_bUseDifferentFadeTimes == false)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Effects fading time (See \"more info.\"): ");
            cl.GetSet_fFadeTime = EditorGUILayout.FloatField("", cl.GetSet_fFadeTime, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sunrise fading time (See \"more info.\"): ");
            cl.GetSet_fSunriseFadeTime = EditorGUILayout.FloatField("", cl.GetSet_fSunriseFadeTime, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Day fading time (See \"more info.\"): ");
            cl.GetSet_fDayFadeTime = EditorGUILayout.FloatField("", cl.GetSet_fDayFadeTime, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sunset fading time (See \"more info.\"): ");
            cl.GetSet_fSunsetFadeTime = EditorGUILayout.FloatField("", cl.GetSet_fSunsetFadeTime, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Night fading time (See \"more info.\"): ");
            cl.GetSet_fNightFadeTime = EditorGUILayout.FloatField("", cl.GetSet_fNightFadeTime, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();
        }

        /* ----- INIT SETTINGS ----- */
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("INIT - This is the function that among other things starts the sound (if we have sound on) and particle effects (when we have one) here you decide how long before it should start"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Time before INIT starts: ");
        cl.GetSet_fInitTimerEnd = EditorGUILayout.FloatField("", cl.GetSet_fInitTimerEnd, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- TEMPRATURE SETTINGS ----- */
        EditorGUILayout.HelpBox(("Temperature settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Set what the lowest and highest temperature for this weather type to be."), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Lowest temperature: ");
        cl.GetSet_fLowestTemp = EditorGUILayout.FloatField("", cl.GetSet_fLowestTemp, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Highest temperature: ");
        cl.GetSet_fHighestTemp = EditorGUILayout.FloatField("", cl.GetSet_fHighestTemp, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- SUN SETTINGS ----- */
        /* ----- (SUN) LIGHT INTENSITY ----- */
        EditorGUILayout.HelpBox(("Directional light (SUN) intensity settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set how intense the light should be during the different timesets (Sunrise, Day, Sunset, Night)"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunrise (light) intensity: ");
        cl.GetSet_fSunriseLightIntensity = EditorGUILayout.FloatField("", cl.GetSet_fSunriseLightIntensity, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Day (light) intensity: ");
        cl.GetSet_fDay_LightIntensity = EditorGUILayout.FloatField("", cl.GetSet_fDay_LightIntensity, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunset (light) intensity: ");
        cl.GetSet_fSunset_LightIntensity = EditorGUILayout.FloatField("", cl.GetSet_fSunset_LightIntensity, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Night (light) intensity: ");
        cl.GetSet_fNight_LightIntensity = EditorGUILayout.FloatField("", cl.GetSet_fNight_LightIntensity, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- (SUN) LIGHT COLOR ----- */
        EditorGUILayout.HelpBox(("Directional light (SUN) color settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set what color the light should be during the different timesets (Sunrise, Day, Sunset, Night)"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunrise (light) color: ");
        cl.GetSet_cSunrise_LightColor = EditorGUILayout.ColorField("", cl.GetSet_cSunrise_LightColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Day (light) color: ");
        cl.GetSet_cDay_LightColor = EditorGUILayout.ColorField("", cl.GetSet_cDay_LightColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunset (light) color: ");
        cl.GetSet_cSunset_LightColor = EditorGUILayout.ColorField("", cl.GetSet_cSunset_LightColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Night (light) color: ");
        cl.GetSet_cNight_LightColor = EditorGUILayout.ColorField("", cl.GetSet_cNight_LightColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- MOON SETTINGS ----- */
        EditorGUILayout.HelpBox(("\n*MOON settings is only important to set if you have a moon, otherwise you can skip these settings \n*During Sunrise, Day and Sunset, the moon intensity is set to 0 so we get no light during these timesets.\n"), MessageType.Info, true);
        EditorGUILayout.HelpBox(("Directional light (MOON) intensity settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set how intense the light should be during the different timesets (Sunrise, Day, Sunset, Night)"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Night (moon light) intensity: ");
        cl.GetSet_fNight_MoonLightIntensity = EditorGUILayout.FloatField("", cl.GetSet_fNight_MoonLightIntensity, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(("Directional light (MOON) color settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set what color the light should be during the night (as that is the only time we have moonlight on!)"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Night (moon light) color: ");
        cl.GetSet_cNight_MoonLightColor = EditorGUILayout.ColorField("", cl.GetSet_cNight_MoonLightColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- SKY COLOR SETTINGS ----- */
        EditorGUILayout.HelpBox(("Skybox color settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set the different skybox colors during the different timesets (Sunrise, Day, Sunset, Night)"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunrise (skybox) color: ");
        cl.GetSet_cSunrise_SkyTintColor = EditorGUILayout.ColorField("", cl.GetSet_cSunrise_SkyTintColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Day (skybox) color: ");
        cl.GetSet_cDay_SkyTintColor = EditorGUILayout.ColorField("", cl.GetSet_cDay_SkyTintColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunset (skybox) color: ");
        cl.GetSet_cSunset_SkyTintColor = EditorGUILayout.ColorField("", cl.GetSet_cSunset_SkyTintColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Night (skybox) color: ");
        cl.GetSet_cNight_SkyTintColor = EditorGUILayout.ColorField("", cl.GetSet_cNight_SkyTintColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- SKY GROUND SETTINGS ----- */
        EditorGUILayout.HelpBox(("Sky ground settings *Only for procedural skyboxes"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Here you set the different skybox ground colors during the different timesets (Sunrise, Day, Sunset, Night) \n\n*Since we can't access other scripts in this you sadly have to choose here as well if you are using the procedural skybox material"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Using procedural skybox?");
        bUsingProcedural = EditorGUILayout.Toggle(bUsingProcedural, GUILayout.MaxWidth(iMinWidth));
        GUILayout.EndHorizontal();

        if (bUsingProcedural == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sunrise (ground) color: ");
            cl.GetSet_cSunrise_SkyGroundColor = EditorGUILayout.ColorField("", cl.GetSet_cSunrise_SkyGroundColor, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Day (ground) color: ");
            cl.GetSet_cDay_SkyGroundColor = EditorGUILayout.ColorField("", cl.GetSet_cDay_SkyGroundColor, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sunset (ground) color: ");
            cl.GetSet_cSunset_SkyGroundColor = EditorGUILayout.ColorField("", cl.GetSet_cSunset_SkyGroundColor, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Night (ground) color: ");
            cl.GetSet_cNight_SkyGroundColor = EditorGUILayout.ColorField("", cl.GetSet_cNight_SkyGroundColor, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();
        }

        /* ----- CLOUD SETTINGS ----- */
        EditorGUILayout.HelpBox(("Cloud settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("What color do you want the clouds to have during this weather effect."), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cloud color: ");
        cl.GetSet_cCloudColor = EditorGUILayout.ColorField("", cl.GetSet_cCloudColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- FOG SETTINGS ----- */
        EditorGUILayout.HelpBox(("Fog settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("In this setting you choose the amount of fog you want to have during the weather effect and which color the fog should be"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Fog amount: ");
        cl.GetSet_fFogAmount = EditorGUILayout.FloatField("", cl.GetSet_fFogAmount, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use different amount of fog during sunrise: ");
        cl.GetSet_bUseMorningFog = EditorGUILayout.Toggle("", cl.GetSet_bUseMorningFog, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        if (cl.GetSet_bUseMorningFog == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Morning fog amount: ");
            cl.GetSet_fFogMorning = EditorGUILayout.FloatField("", cl.GetSet_fFogMorning, GUILayout.MaxWidth(iMedWidth));
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Fog color: ");
        cl.GetSet_cFogColor = EditorGUILayout.ColorField("", cl.GetSet_cFogColor, GUILayout.MaxWidth(iMedWidth));
        GUILayout.EndHorizontal();

        /* ----- PARTICLE SETTINGS ----- */
        EditorGUILayout.HelpBox(("Particle settings"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("If you want different particles to be used during the timesets you attach these here."), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunrise particles (GameObject): ");
        cl.GetSet_pSunriseParticle = EditorGUILayout.ObjectField("", cl.GetSet_pSunriseParticle, typeof(GameObject), true, GUILayout.MaxWidth(iMaxWidth)) as GameObject;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Day particles (GameObject): ");
        cl.GetSet_pDayParticle = EditorGUILayout.ObjectField("", cl.GetSet_pDayParticle, typeof(GameObject), true, GUILayout.MaxWidth(iMaxWidth)) as GameObject;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sunset particles (GameObject): ");
        cl.GetSet_pSunsetParticle = EditorGUILayout.ObjectField("", cl.GetSet_pSunsetParticle, typeof(GameObject), true, GUILayout.MaxWidth(iMaxWidth)) as GameObject;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Night particles (GameObject): ");
        cl.GetSet_pNightParticle = EditorGUILayout.ObjectField("", cl.GetSet_pNightParticle, typeof(GameObject), true, GUILayout.MaxWidth(iMaxWidth)) as GameObject;
        GUILayout.EndHorizontal();
    }
}