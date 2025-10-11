using System;
using System.Collections;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Texture Skybox")]
    [SerializeField] Texture2D skyboxSunrise;
    [SerializeField] Texture2D skyboxDay;
    [SerializeField] Texture2D skyboxSunset;
    [SerializeField] Texture2D skyboxNight;
    
    [Header("Color Light")]
    [SerializeField] Gradient graddientNightToSunrise;
    [SerializeField] Gradient graddientSunriseToDay;
    [SerializeField] Gradient graddientDayToSunset;
    [SerializeField] Gradient graddientSunsetToNight;
    
    [SerializeField] private Light globalLight;
    
    [Header("Virtual Time")]
    [SerializeField] private int minutes;
    
    private Coroutine skyboxRoutine;
    private Coroutine lightRoutine;
    public int Minutes
    { get { return minutes; } set { minutes = value; OnMinutesChange(value); } }
    
    [SerializeField]private int hours;
    public int Hours{get {return  hours;} set { hours = value; OnHoursChange(value); } }

    [SerializeField]private int days;
    public int Days{get {return  days;} set { days = value; } }

    [SerializeField]private float tempSecond;

    [SerializeField] private float scaleTime = 50f;

    private void Start()
    {
        StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f));
        StartCoroutine(LerpLight(graddientNightToSunrise, 10f));
    }

    public void Update()
    {
        tempSecond += Time.deltaTime * scaleTime;
 
        if (tempSecond >= 1f)
        {
            int extraMinutes = Mathf.FloorToInt(tempSecond);
            Minutes += extraMinutes;
            tempSecond -= extraMinutes;
        }
    }
 
    private void OnMinutesChange(int value)
    {
        globalLight.transform.Rotate(Vector3.up, 360f / 1440f, Space.World);
        if (value >= 100)
        {
            Hours++;
            minutes = 0;
        }
        if (Hours >= 4)
        {
            Hours = 0;
            Days++;
        }
    }
 
    private void OnHoursChange(int value)
    {
        StopAllCoroutines();
        
        if (skyboxRoutine != null) StopCoroutine(skyboxRoutine);
        if (lightRoutine != null) StopCoroutine(lightRoutine);
        
        if (value == 0)
        {
            StartCoroutine(LerpSkybox(skyboxNight, skyboxSunrise, 10f));
            StartCoroutine(LerpLight(graddientNightToSunrise, 10f));
        }
        else if (value == 1)
        {
            StartCoroutine(LerpSkybox(skyboxSunrise, skyboxDay, 10f));
            StartCoroutine(LerpLight(graddientSunriseToDay, 10f));
        }
        else if (value == 2)
        {
            StartCoroutine(LerpSkybox(skyboxDay, skyboxSunset, 10f));
            StartCoroutine(LerpLight(graddientDayToSunset, 10f));
        }
        else if (value == 3)
        {
            StartCoroutine(LerpSkybox(skyboxSunset, skyboxNight, 10f));
            StartCoroutine(LerpLight(graddientSunsetToNight, 10f));
        }
    }
 
    private IEnumerator LerpSkybox(Texture2D a, Texture2D b, float time)
    {
        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", 0);
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            RenderSettings.skybox.SetFloat("_Blend", i / time);
            yield return null;
        }
        RenderSettings.skybox.SetTexture("_Texture1", b);
    }
 
    private IEnumerator LerpLight(Gradient lightGradient, float time)
    {
        for (float i = 0; i < time; i += Time.deltaTime)
        {
            globalLight.color = lightGradient.Evaluate(i / time);
            RenderSettings.fogColor = globalLight.color;
            yield return null;
        }
    }
}
