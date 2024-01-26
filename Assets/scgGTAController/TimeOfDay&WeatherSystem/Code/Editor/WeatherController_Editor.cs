using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
[CustomEditor(typeof(Weather_Controller))]
public class WeatherController_Editor : Editor
{
    public bool bShowTips;

    private int iMinWidth = 30; // Was 30
    private int iMedWidth = 60; // Was 60
    private int iMaxWidth = 120; // Was 120

    public override void OnInspectorGUI()
    {
        DrawNewGUI();
        EditorUtility.SetDirty(target);
    }

    private void DrawNewGUI()
    {
        Weather_Controller cl = target as Weather_Controller;

        // SHOW MORE TIPS
        GUILayout.BeginHorizontal();
        GUILayout.Label("Show more information");
        bShowTips = EditorGUILayout.Toggle(bShowTips, GUILayout.MaxWidth(iMinWidth));
        GUILayout.EndHorizontal();

        // GAMEOBJECTS AND MATERIALS
        EditorGUILayout.HelpBox(("Add gameobjects, lights and materials"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("Time of Day: This should be the gameobject where you have the Time of day scripts on (See or use example in package) \n\nSkybox: Your skybox material (If you use a procedural skybox make sure to tick the option for this) \n\nCloud: The material you use for your clouds"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Time of day (Gameobject): ");
        cl.gTimeOfDay = EditorGUILayout.ObjectField("", cl.gTimeOfDay, typeof(GameObject), true, GUILayout.MaxWidth(iMaxWidth)) as GameObject;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Skybox (Material): ");
        cl.matSkybox = EditorGUILayout.ObjectField("", cl.matSkybox, typeof(Material), true, GUILayout.MaxWidth(iMaxWidth)) as Material;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Cloud (Material): ");
        cl.matClouds = EditorGUILayout.ObjectField("", cl.matClouds, typeof(Material), true, GUILayout.MaxWidth(iMaxWidth)) as Material;
        GUILayout.EndHorizontal();

        // STARTING WEATHER
        EditorGUILayout.HelpBox(("Weather settings:"), MessageType.None, true);
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("With this you pick if you want to use random weather when the game starts or if you want to choose which weather you want to begin with. \n\nRANDOM & NUMBEROFWEATHERTYPES = Random Weather\nSUN,RAIN and so on = The game begins with that weathertype"), MessageType.Info, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Starting weather: ");
        cl.en_CurrWeather = (Weather_Controller.WeatherType)EditorGUILayout.EnumPopup(cl.en_CurrWeather, GUILayout.MaxWidth(iMaxWidth));
        GUILayout.EndHorizontal();

        // PROCEDURAL SKYBOX
        if (bShowTips == true)
            EditorGUILayout.HelpBox(("If you are using the Procedural Unity Skybox make sure to check this box!"), MessageType.None, true);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Using procedural skybox");
        cl.GetSet_bUsingProceduralSkybox = EditorGUILayout.Toggle(cl.GetSet_bUsingProceduralSkybox, GUILayout.MaxWidth(iMinWidth));
        GUILayout.EndHorizontal();

        if (bShowTips == true)
            EditorGUILayout.HelpBox(("If you want the weather to change during the game make sure this option is ticked on \n\n*If you don't want the weather to change, make sure to choose your starting weather. If you haven't picked weathertype it defaults to SUN"), MessageType.Info, true);

        // RANDOM WEATHER CHANGE
        GUILayout.BeginHorizontal();
        GUILayout.Label("Use random weather: ");
        cl.GetSet_bUseRandomWeather = EditorGUILayout.Toggle(cl.GetSet_bUseRandomWeather, GUILayout.MaxWidth(iMinWidth));
        GUILayout.EndHorizontal();

        if (cl.GetSet_bUseRandomWeather == true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Use SUN: ");
            cl.GetSet_bUseSun = EditorGUILayout.Toggle(cl.GetSet_bUseSun, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use CLOUDY: ");
            cl.GetSet_bUseCloudy = EditorGUILayout.Toggle(cl.GetSet_bUseCloudy, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use RAIN: ");
            cl.GetSet_bUseRain = EditorGUILayout.Toggle(cl.GetSet_bUseRain, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use THUNDERSTORM: ");
            cl.GetSet_bUseThunderstorm = EditorGUILayout.Toggle(cl.GetSet_bUseThunderstorm, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use SNOW: ");
            cl.GetSet_bUseSnow = EditorGUILayout.Toggle(cl.GetSet_bUseSnow, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            if (bShowTips == true)
                EditorGUILayout.HelpBox(("If you want the weather to change after random amount of day, check this box to get more options"), MessageType.Info, true);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Random day weather change");
            cl.GetSet_bUseRandomDaysWeather = EditorGUILayout.Toggle(cl.GetSet_bUseRandomDaysWeather, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Set fog to sky color?");
            cl.setFogToSkyColor = EditorGUILayout.Toggle(cl.setFogToSkyColor, GUILayout.MaxWidth(iMinWidth));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Fog-sky blending render texture");
            cl.fogBlendingTexture = EditorGUILayout.ObjectField(cl.fogBlendingTexture, typeof(RenderTexture), true, GUILayout.MaxWidth(iMinWidth)) as RenderTexture;
            GUILayout.EndHorizontal();

            if (cl.GetSet_bUseRandomDaysWeather == true)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Minimum amount of days before change:");
                cl.GetSet_iMinAmountOfDaysToNewWeather = EditorGUILayout.IntField(cl.GetSet_iMinAmountOfDaysToNewWeather, GUILayout.MaxWidth(iMedWidth));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Maximum amount of days before change:");
                cl.GetSet_iMaxAmountOfDaysToNewWeather = EditorGUILayout.IntField(cl.GetSet_iMaxAmountOfDaysToNewWeather, GUILayout.MaxWidth(iMedWidth));
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Change weather every (days):");
                cl.GetSet_iChangeWeatherAfterDays = EditorGUILayout.IntField(cl.GetSet_iChangeWeatherAfterDays, GUILayout.MaxWidth(iMedWidth));
                GUILayout.EndHorizontal();
            }
        }
    }
}

