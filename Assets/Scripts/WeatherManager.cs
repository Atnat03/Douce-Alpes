using System;
using UnityEngine;
using System.Collections;

public class WeatherManager : MonoBehaviour
{
    public enum DayMoment
    {
        Morning,
        Day1,
        Day2,
        Night
    }

    [Header("Skyboxes")]
    [SerializeField] private Texture2D skyboxMorning;
    [SerializeField] private Texture2D skyboxDay1;
    [SerializeField] private Texture2D skyboxDay2;
    [SerializeField] private Texture2D skyboxNight;

    [Header("Light Gradients")]
    [SerializeField] private Gradient morningGradient;
    [SerializeField] private Gradient day1Gradient;
    [SerializeField] private Gradient day2Gradient;
    [SerializeField] private Gradient nightGradient;
    
    [SerializeField] private float morningIntensity;
    [SerializeField] private float dayIntensity;
    [SerializeField] private float nightIntensity;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 1.5f;

    [SerializeField] private DayMoment currentMoment = DayMoment.Morning;

    private bool isTransitioning;
    
    [Header("Sun Light")]
    [SerializeField] private Light directionalLight;
    
    [SerializeField] private GameObject cameraNightVolume;

    [Header("VFX")] 
    [SerializeField] private GameObject vfxLucioles;

    private void Start()
    {
        ApplyMomentInstant(currentMoment);
    }

    private void Update()
    {
        cameraNightVolume.SetActive(currentMoment == DayMoment.Night);
        vfxLucioles.SetActive(currentMoment == DayMoment.Night);
    }

    public void NextMoment()
    {
        if (isTransitioning) return;

        DayMoment next = GetNextMoment(currentMoment);
        StartCoroutine(TransitionToMoment(currentMoment, next));
        currentMoment = next;
    }

    private DayMoment GetNextMoment(DayMoment moment)
    {
        return moment switch
        {
            DayMoment.Morning => DayMoment.Day1,
            DayMoment.Day1 => DayMoment.Day2,
            DayMoment.Day2 => DayMoment.Night,
            DayMoment.Night => DayMoment.Morning,
            _ => DayMoment.Morning
        };
    }

    private IEnumerator TransitionToMoment(DayMoment from, DayMoment to)
    {
        isTransitioning = true;

        Texture2D fromSky = GetSkybox(from);
        Texture2D toSky = GetSkybox(to);

        Gradient fromGrad = GetGradient(from);
        Gradient toGrad = GetGradient(to);

        float fromInt = GetIntensity(from);
        float toInt = GetIntensity(to);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionDuration;
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            // Skybox
            RenderSettings.skybox.SetTexture("_Texture1", fromSky);
            RenderSettings.skybox.SetTexture("_Texture2", toSky);
            RenderSettings.skybox.SetFloat("_Blend", smoothT);

            Color fromColor = fromGrad.Evaluate(0.5f);
            Color toColor = toGrad.Evaluate(0.5f);

            RenderSettings.fogColor = Color.Lerp(fromColor, toColor, smoothT);
            directionalLight.color = Color.Lerp(fromColor, toColor, smoothT);
            directionalLight.intensity = Mathf.Lerp(fromInt, toInt, smoothT);
            
            yield return null;
        }

        ApplyMomentInstant(to);
        isTransitioning = false;
    }

    private void ApplyMomentInstant(DayMoment moment)
    {
        RenderSettings.skybox.SetTexture("_Texture1", GetSkybox(moment));
        RenderSettings.skybox.SetTexture("_Texture2", GetSkybox(moment));
        RenderSettings.skybox.SetFloat("_Blend", 0f);

        Color c = GetGradient(moment).Evaluate(0.5f);
        RenderSettings.fogColor = c;
        directionalLight.color = c;
        directionalLight.intensity = GetIntensity(moment);
    }


    private Texture2D GetSkybox(DayMoment moment)
    {
        return moment switch
        {
            DayMoment.Morning => skyboxMorning,
            DayMoment.Day1 => skyboxDay1,
            DayMoment.Day2 => skyboxDay2,
            DayMoment.Night => skyboxNight,
            _ => skyboxMorning
        };
    }
    
    private float GetIntensity(DayMoment moment)
    {
        return moment switch
        {
            DayMoment.Morning => morningIntensity,
            DayMoment.Day1 => dayIntensity,
            DayMoment.Day2 => dayIntensity,
            DayMoment.Night => nightIntensity,
            _ => dayIntensity
        };
    }
    
    private Gradient GetGradient(DayMoment moment)
    {
        return moment switch
        {
            DayMoment.Morning => morningGradient,
            DayMoment.Day1 => day1Gradient,
            DayMoment.Day2 => day2Gradient,
            DayMoment.Night => nightGradient,
            _ => morningGradient
        };
    }

    public DayMoment GetCurrentDayMoment()
    {
        return currentMoment;
    }
}
