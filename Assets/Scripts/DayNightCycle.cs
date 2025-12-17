using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Texture Skybox")]
    [SerializeField] private Texture2D skyboxSunrise;
    [SerializeField] private Texture2D skyboxDay;
    [SerializeField] private Texture2D skyboxSunset;
    [SerializeField] private Texture2D skyboxNight;

    [Header("Color Light")]
    [SerializeField] private Gradient graddientNightToSunrise;
    [SerializeField] private Gradient graddientSunriseToDay;
    [SerializeField] private Gradient graddientDayToSunset;
    [SerializeField] private Gradient graddientSunsetToNight;
    
    [Header("Virtual Time")]
    [SerializeField] private int minutes;
    [SerializeField] private int hours;
    [SerializeField] private int days;

    private float tempSecond;

    [Header("Time Settings")]
    [SerializeField] private float scaleTime = 2000f;

    public int Minutes
    {
        get => minutes;
        set
        {
            minutes = value;
            OnMinutesChange(value);
        }
    }

    public int Hours
    {
        get => hours;
        set
        {
            hours = value;
            ApplyPhaseByTime();
        }
    }

    public int Days
    {
        get => days;
        set => days = value;
    }

    private void Start()
    {
        InitializeCycle();
    }

    private void Update()
    {
        tempSecond += Time.deltaTime * scaleTime;

        if (tempSecond >= 60f)
        {
            int extraMinutes = Mathf.FloorToInt(tempSecond / 60f);
            Minutes += extraMinutes;
            tempSecond -= extraMinutes * 60f;
        }
    }

    private void OnMinutesChange(int value)
    {
       //globalLight.transform.rotation = Quaternion.Euler(50f, 360f * (hours * 60f + minutes) / 1440f, 0f);

        if (value >= 60)
        {
            minutes = 0;
            Hours++;
        }

        if (Hours >= 24)
        {
            Hours = 0;
            Days++;
        }

        ApplyPhaseByTime();
    }

    private void ApplyPhaseByTime()
    {
        float totalMinutes = hours * 60f + minutes;
        float t;

        // 1️⃣ Sunset → Night : 18h → 22h (1080 → 1320)
        if (totalMinutes >= 1080 && totalMinutes < 1320)
        {
            t = (totalMinutes - 1080f) / (1320f - 1080f);
            SetSkyboxBlend(skyboxDay, skyboxSunset, t);
            SetLight(graddientDayToSunset, t);
        }
        // 2️⃣ Night → Sunrise : 22h → 5h (1320 → 1440 + 0 → 300)
        else if (totalMinutes >= 1320 || totalMinutes < 300)
        {
            float current = totalMinutes < 300 ? totalMinutes + 1440f : totalMinutes;
            t = (current - 1320f) / (300f + 1440f - 1320f);
            SetSkyboxBlend(skyboxSunset, skyboxNight, t);
            SetLight(graddientSunsetToNight, t);
        }
        // 3️⃣ Sunrise → Day : 5h → 8h (300 → 480)
        else if (totalMinutes >= 300 && totalMinutes < 480)
        {
            t = (totalMinutes - 300f) / (480f - 300f);
            SetSkyboxBlend(skyboxNight, skyboxSunrise, t);
            SetLight(graddientNightToSunrise, t);
        }
        // 4️⃣ Day : 8h → 18h (480 → 1080)
        else if (totalMinutes >= 480 && totalMinutes < 1080)
        {
            t = (totalMinutes - 480f) / (1080f - 480f);
            SetSkyboxBlend(skyboxSunrise, skyboxDay, t);
            SetLight(graddientSunriseToDay, t);
        }
    }

    private void SetSkyboxBlend(Texture2D a, Texture2D b, float t)
    {
        t = Mathf.Clamp01(t);
        t = Mathf.SmoothStep(0f, 1f, t);

        RenderSettings.skybox.SetTexture("_Texture1", a);
        RenderSettings.skybox.SetTexture("_Texture2", b);
        RenderSettings.skybox.SetFloat("_Blend", t);
    }

    private void SetLight(Gradient g, float t)
    {
        t = Mathf.Clamp01(t);
        Color c = g.Evaluate(t);
        RenderSettings.fogColor = c;
    }

    /// <summary>
    /// Initialise le cycle jour/nuit à l'heure actuelle sans glitch.
    /// </summary>
    public void InitializeCycle()
    {
        // Applique le bon skybox et la lumière pour l'heure actuelle
        ApplyPhaseByTime();

        // Reset timer pour Update
        tempSecond = 0f;
    }
}
