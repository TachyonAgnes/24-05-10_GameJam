using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class EnvironmentIlluminationSystem : MonoBehaviour
{

    [SerializeField] ProbeSystem ProbeInterpolationSystem;
    [SerializeField] WeatherManager WeatherInterpolationSystem;
    [SerializeField, Range(0f, 24.0f)] float CurrentTime;
    [SerializeField, Range(0f, 1.0f)] float RainFactor;
    [SerializeField, Range(0f, 1.0f)] float SnowFactor;
    [SerializeField, Range(0f, 1.0f)] float FogFactor;
    [SerializeField, Range(0f, 100.0f)] float TimeMultipler;
    [SerializeField] WeatherManager.CloudType CurrentCloudType;
    WeatherManager.CloudType _previousCloudType;

    public void UpdateTime() {
        CurrentTime = (CurrentTime + Time.deltaTime * TimeMultipler) % 24.0f;
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
            WeatherInterpolationSystem.SetRainIntensity(RainFactor);
            WeatherInterpolationSystem.SetSnowIntensity(SnowFactor);
            WeatherInterpolationSystem.SetFogIntensity(FogFactor);
        }
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

    [ContextMenu("Set Day Time Game Play")]
    public void SetDayTimeGamePlay(){
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        UpdateCloud();
        UpdateWeather();
    }
}
