using UnityEngine;

public class EnvironmentIlluminationSystem : MonoBehaviour
{
    public static EnvironmentIlluminationSystem Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            Instance = null;
        }
        else
        {
            Instance = this;
        }
    }

    [SerializeField] ProbeSystem ProbeInterpolationSystem;
    [SerializeField] WeatherManager WeatherInterpolationSystem;
    [SerializeField, Range(0f, 24.0f)] float CurrentTime;
    [SerializeField, Range(0f, 1.0f)] float RainFactor;
    [SerializeField, Range(0f, 1.0f)] float SnowFactor; 
    [SerializeField, Range(0f, 1.0f)] float FogFactor;
    [SerializeField, Range(0f, 100.0f)] float TimeMultipler;
    [SerializeField] WeatherManager.CloudType CurrentCloudType;
    WeatherManager.CloudType _previousCloudType;

    [Header("Time Transition")]
    [SerializeField, Range(-1f, 24.0f)] float TargetTime = -1f;  // Target time for interpolation
    [SerializeField, Range(0f, 15.0f)] float TimeTransitionDuration = 10f;  // Duration of the transition in seconds
    private float timeTransitionProgress = 0f;  // Progress of the time transition

    [Header("Weather Transition")]
    [SerializeField, Range(0f, 1.0f)] float TargetRainFactor;
    [SerializeField, Range(0f, 1.0f)] float TargetFogFactor;
    [SerializeField, Range(0f, 15.0f)] float WeatherTransitionDuration = 10f;


    public void UpdateTime()
    {
        if (TargetTime >= 0f)
        {
            if (timeTransitionProgress < 1f)
            {
                // Smoothly interpolate towards the target time
                CurrentTime = Mathf.Lerp(CurrentTime, TargetTime, timeTransitionProgress);
                timeTransitionProgress += Time.deltaTime / TimeTransitionDuration;
            }
            else
            {
                // Finalize the time setting
                CurrentTime = TargetTime;
                TargetTime = -1f;
            }
        }
        //else
        //{
        //    // Regular time update
        //    CurrentTime = (CurrentTime + Time.deltaTime * TimeMultipler) % 24.0f;
        //}

        if (ProbeInterpolationSystem != null)
            ProbeInterpolationSystem.SetCurrentTime(CurrentTime);
        if (WeatherInterpolationSystem != null)
            WeatherInterpolationSystem.SetCurrentTime(CurrentTime);
    }


    public void UpdateCloud() {
        if(CurrentCloudType != _previousCloudType) {
            _previousCloudType = CurrentCloudType;
            WeatherInterpolationSystem.SetCloudType(CurrentCloudType);
        }
    }

    public void UpdateWeather() {
        if (WeatherInterpolationSystem != null) {
            // Calculate the interpolation step based on the transition duration
            float interpolationStep = Time.deltaTime / WeatherTransitionDuration;

            // Ensure the interpolation step is correctly clamped
            interpolationStep = Mathf.Clamp(interpolationStep, 0f, 1f);

            RainFactor = Mathf.Lerp(RainFactor, TargetRainFactor, interpolationStep);
            FogFactor = Mathf.Lerp(FogFactor, TargetFogFactor, interpolationStep);

            WeatherInterpolationSystem.SetRainIntensity(RainFactor);
            WeatherInterpolationSystem.SetSnowIntensity(SnowFactor);
            WeatherInterpolationSystem.SetFogIntensity(FogFactor);
        }
    }

    [ContextMenu("SetDayTime")]
    public void SetDayTime(){
        SetTargetTime(16f);
        CurrentCloudType = WeatherManager.CloudType.Cloudy;
    }

    [ContextMenu("SetNightTime")]
    public void SetNightTime()
    {
        SetTargetTime(20f);
        CurrentCloudType = WeatherManager.CloudType.Storm;
    }

    [ContextMenu("SetWeatherRainny")]
    public void SetRainny()
    {
        TargetFogFactor = 1f;
        TargetRainFactor = 1f;
    }

    [ContextMenu("SetWeatherNormal")]
    public void SetNormal()
    {
        TargetFogFactor = 0f;
        TargetRainFactor = 0f;
    }


    public void SetTargetTime(float newTargetTime)
    {
        TargetTime = newTargetTime % 24.0f;  // Ensure target time is within 0 to 24
        timeTransitionProgress = 0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(ProbeInterpolationSystem != null)
            ProbeInterpolationSystem.SetCurrentTime(12);
        if(WeatherInterpolationSystem != null)
            WeatherInterpolationSystem.SetCurrentTime(12);
        _previousCloudType = CurrentCloudType;
    }



    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        UpdateCloud();
        UpdateWeather();
    }
}