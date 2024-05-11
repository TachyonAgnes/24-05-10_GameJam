using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static WeatherManager;

public class WeatherManager : MonoBehaviour {
    public enum CloudType {
        Sparse = 0,
        Cloudy = 1,
        Storm = 2,
        Overcast = 3,
    }

    //differemt weather has different states
    class WeatherState {
        public float InitialIntensity;
        public float CurrentIntensity;
        public float TargetIntensity;

        bool _transitionInProgress = false;
        float _transitionProgress;
        float _transitionTime;
        WeatherElementConfig Config;

        public void SwitchToNewPreset(WeatherElementConfig config, float transitionTime)
        {
            // transfer the previous intensity as our starting point
            InitialIntensity = CurrentIntensity;

            // pick a new intensity
            TargetIntensity = config.GetRandomIntensity();

            // store the config
            Config = config;

            // setup the transition
            _transitionProgress = transitionTime > 0f ? 0f : 1f;
            _transitionTime = transitionTime;
            _transitionInProgress = true;
        }

        public float Tick()
        {
            // if there's no transition then do nothing
            if (!_transitionInProgress)
                return CurrentIntensity;

            // need to update progress?
            if (_transitionProgress < 1f)
                _transitionProgress += Time.deltaTime / _transitionTime;

            // update intensity?
            CurrentIntensity = Mathf.Lerp(InitialIntensity, TargetIntensity, _transitionProgress);

            // transition finished?
            if (_transitionProgress >= 1f)
            {
                InitialIntensity = CurrentIntensity;
                TargetIntensity = Config.GetRandomIntensity();
                _transitionProgress = 0f;
                _transitionTime = Config.GetFluctuationTime();
                _transitionInProgress = _transitionTime > 0f;
            }

            return CurrentIntensity;
        }
    }

    [SerializeField, Range(0f, 1f)] float RainIntensity;
    [SerializeField, Range(0f, 1f)] float SnowIntensity;
    [SerializeField, Range(0f, 1f)] float HailIntensity;
    [SerializeField, Range(0f, 1f)] float FogIntensity;
    [SerializeField, Range(0f, 0.1f)] float StarSkyMoveSpeed;
    [SerializeField, Range(0f, 1000.0f)] float CloudMoveSpeed;
    [SerializeField, Range(0f, 360.0f)] float CloudMoveDirection;
    [SerializeField]  CloudType CurrentCloudType;

    [SerializeField] float MinFogAttenuationDistance = 10f;
    [SerializeField] float MaxFogAttenuationDistance = 50f;

    [SerializeField] VisualEffect RainVFX;
    [SerializeField] VisualEffect SnowVFX;
    [SerializeField] VisualEffect HailVFX;
    [SerializeField] Volume FogVolume;
    [SerializeField] Volume CloudVolume;
    [SerializeField] Volume SkyAndFogVolume;

    [Header("Default Preset")]
    [SerializeField] WeatherPreset DefaultWeather;

    [Header("Time Of Day")]
    [SerializeField] float TimeMultiplier = 90f;
    [SerializeField] float HoursPerDay = 24f;
    [SerializeField] float StartTimeInHours = 4f;
    [SerializeField] float SunriseTime = 6f;
    [SerializeField] float SunsetTime = 18f;
    [SerializeField] Transform SunAndMoonSystem;
    [SerializeField] HDAdditionalLightData SunLightData;
    [SerializeField] HDAdditionalLightData MoonLightData;
    [SerializeField, Range(0f, 24.0f)] float CurrentTime;


    [Header("Transition Debug")]
    [SerializeField] bool DEBUG_PerformTransition;
    [SerializeField] WeatherPreset DEBUG_TargetPreset;
    [SerializeField] float DEBUG_TransitionTime;

    //public parameters
    public float CurrentTimeInHours => _currentTimeInSeconds / 3600f;
    public bool IsDay => CurrentTimeInHours >= SunriseTime && CurrentTimeInHours <= SunsetTime;
    public bool IsNight => !IsDay;
    public float DayLength => SunsetTime - SunriseTime;
    public float NightLength => HoursPerDay - DayLength;
    public float CurrentFluctuation = 0f;

    //temp variables
    float _currentTimeInSeconds = 0f;
    float _previousRainIntensity;
    float _previousHailIntensity;
    float _previousSnowIntensity;
    float _previousFogIntensity;
    Fog _cachedFogComponent;
    VolumetricClouds _cachedCloudComponent;
    PhysicallyBasedSky _cachedPhysicalSkyComponent;
    WeatherPreset _currentWeather;
    //weather state
    WeatherState _stateRain = new WeatherState();
    WeatherState _stateHail = new WeatherState();
    WeatherState _stateSnow = new WeatherState();
    WeatherState _stateFog = new WeatherState();
    //weather parameters
    float _initialFluctuation = 0f;
    float _targetFluctuation = 0f;
    float _fluctuationTime = 0f;
    float _fluctuationProgress = 0f;
    bool _fluctuationInProgress = false;
    float[] _cloudyDensityMultiplier = { 0.155f, 0.334f, 0.592f , 1.0f};
    float[] _cloudyShapeFactor = { 0.988f, 0.8731f, 0.827f, 0.71f};

    float _previousCloudDensityMutiplier;
    float _previousCloudShapeFactor;
    float _cloudTypeChangeSpeed = 1.0f;
    float _cloudInterpolationFactor = 0.0f;


    //The function jump to target time
    [ContextMenu("Set Morning")]
    public void UpdateTimeMorning() {
        UpdateTime(6);
    }

    [ContextMenu("Set Midnight")]
    public void UpdateTimeMidNight() {
        UpdateTime(24);
    }

    [ContextMenu("Set Noon")]
    public void UpdateTimeNoon() {
        UpdateTime(12);
    }

    [ContextMenu("Set Afternoon")]
    public void UpdateTimeAfternoon() {
        UpdateTime(18);
    }

    [ContextMenu("Set Afternoon2")]
    public void UpdateTimeAfternoon2() {
        UpdateTime(15);
    }


    public void SetCurrentTime(float time) {
        CurrentTime = time;
    }

    public void SetCloudMoveSpeed(float speed) {
        if (_cachedCloudComponent == null) {
            VolumeProfile profile = CloudVolume.sharedProfile;
            profile.TryGet<VolumetricClouds>(out _cachedCloudComponent);
        }
        CloudMoveSpeed = speed;
        _cachedCloudComponent.globalWindSpeed = new WindSpeedParameter(CloudMoveSpeed);
    }

    public void SetCloudMoveDirection(float angle) {
        if (_cachedCloudComponent == null) {
            VolumeProfile profile = CloudVolume.sharedProfile;
            profile.TryGet<VolumetricClouds>(out _cachedCloudComponent);
        }
        CloudMoveDirection = angle;
        _cachedCloudComponent.orientation = new WindOrientationParameter(CloudMoveDirection);
    }
    
    public void UpdateStarTexture() {
        VolumeProfile profile = SkyAndFogVolume.sharedProfile;
        if(_cachedPhysicalSkyComponent == null) {
            profile.TryGet<PhysicallyBasedSky>(out _cachedPhysicalSkyComponent);
        }
        Vector3 resVec = _cachedPhysicalSkyComponent.spaceRotation.value + new Vector3(1.0f, 0.0f, 0.0f) * StarSkyMoveSpeed;
        if (resVec.x > 360.0f)
            resVec.x = 0.0f;
        _cachedPhysicalSkyComponent.spaceRotation.SetValue(new Vector3Parameter(resVec));
    }

    public void SetCloudType(CloudType cloudType) {
        CurrentCloudType  = cloudType;
        if (_cachedCloudComponent == null) {
            VolumeProfile profile = CloudVolume.sharedProfile;
            profile.TryGet<VolumetricClouds>(out _cachedCloudComponent);
        }

        _previousCloudDensityMutiplier = _cachedCloudComponent.densityMultiplier.value;
        _previousCloudShapeFactor = _cachedCloudComponent.shapeFactor.value;
        _cloudInterpolationFactor = 0.0f;
    }

    public void SetRainIntensity(float intensity) {
        RainIntensity = intensity;
    }

    public void SetSnowIntensity(float intensity) {
        SnowIntensity = intensity;
    }

    public void SetFogIntensity(float intensity) {
        FogIntensity = intensity;
    }

    public void UpdateCloudType() {
        if (Mathf.Abs(1.0f - _cloudInterpolationFactor) < 0.00001f)
            return;

        float targetDensityMultiplier = _cloudyDensityMultiplier[(int)CurrentCloudType];
        float targetShapeFactor = _cloudyShapeFactor[(int)CurrentCloudType];
        float interpolatedDensityMultiplier = Mathf.Lerp(_previousCloudDensityMutiplier, targetDensityMultiplier, _cloudInterpolationFactor);
        float interpolatedShapeFactor = Mathf.Lerp(_previousCloudShapeFactor, targetShapeFactor, _cloudInterpolationFactor);
        _cloudInterpolationFactor += Time.deltaTime * _cloudTypeChangeSpeed;

        if (_cachedCloudComponent == null) {
            VolumeProfile profile = CloudVolume.sharedProfile;
            profile.TryGet<VolumetricClouds>(out _cachedCloudComponent);
        }
        
        _cachedCloudComponent.densityMultiplier.value = interpolatedDensityMultiplier;
        _cachedCloudComponent.shapeFactor.value = interpolatedShapeFactor;
    }


    void UpdateTime(float currentTime) {
        _currentTimeInSeconds = (currentTime * 3600f) % (HoursPerDay * 3600f);

        float sunAndMoonAngle = 0f;
        // update the shadows
        if (IsDay)
        {
            MoonLightData.EnableShadows(false);
            SunLightData.EnableShadows(true);

            sunAndMoonAngle = 180f * Mathf.InverseLerp(SunriseTime, SunsetTime, CurrentTimeInHours);
        }
        else
        {
            SunLightData.EnableShadows(false);
            MoonLightData.EnableShadows(true);

            float hoursIntoNight = 0f;
            if (CurrentTimeInHours > SunsetTime)
                hoursIntoNight = CurrentTimeInHours - SunsetTime;
            else
                hoursIntoNight = CurrentTimeInHours + (HoursPerDay - SunsetTime);

            sunAndMoonAngle = 180f + 180f * (hoursIntoNight / NightLength);
        }

        SunAndMoonSystem.eulerAngles = new Vector3(sunAndMoonAngle, 0f, 0f);
    }

    void UpdateCloud() {
        SetCloudMoveDirection(CloudMoveDirection);
        SetCloudMoveSpeed(CloudMoveSpeed);
    }

    void UpdateWeatherTransition() {
        // is there a fluctuation?
        if (_fluctuationInProgress)
        {
            _fluctuationProgress += Time.deltaTime / _fluctuationTime;

            CurrentFluctuation = Mathf.Lerp(_initialFluctuation, _targetFluctuation, _fluctuationProgress);

            // fluctuation finished? start a new one
            if (_fluctuationProgress >= 1f)
            {
                _initialFluctuation = CurrentFluctuation;
                _targetFluctuation = _currentWeather.GetRandomFluctuation();
                _fluctuationTime = _currentWeather.GetFluctuationTime();
                _fluctuationProgress = 0f;
            }
        }
        
        RainIntensity = Mathf.Clamp01(CurrentFluctuation + _stateRain.Tick());
        HailIntensity = Mathf.Clamp01(CurrentFluctuation + _stateHail.Tick());
        SnowIntensity = Mathf.Clamp01(CurrentFluctuation + _stateSnow.Tick());
        FogIntensity  = Mathf.Clamp01(CurrentFluctuation + _stateFog.Tick());
    }

    public void ChangeWeather(WeatherPreset newWeather, float transitionTime) {
        _currentWeather = newWeather;

        _stateRain.SwitchToNewPreset(newWeather.Rain, transitionTime);
        _stateHail.SwitchToNewPreset(newWeather.Hail, transitionTime);
        _stateSnow.SwitchToNewPreset(newWeather.Snow, transitionTime);
        _stateFog.SwitchToNewPreset(newWeather.Fog, transitionTime);

        _cachedCloudComponent.cloudPreset = newWeather.CloudPreset;
        _cachedCloudComponent.sunLightDimmer.value = newWeather.SunLightDimmer;
        _cachedCloudComponent.ambientLightProbeDimmer.value = newWeather.AmbientLightDimmer;

        // setup for the fluctuation
        _initialFluctuation = CurrentFluctuation;
        _targetFluctuation = _currentWeather.GetRandomFluctuation();
        _fluctuationTime = _currentWeather.GetFluctuationTime();
        _fluctuationProgress = 0f;
        _fluctuationInProgress = _fluctuationTime > 0f;

        // no fluctuation happening - reset the modifiers
        if (!_fluctuationInProgress)
            CurrentFluctuation = _initialFluctuation = _targetFluctuation = 0f;
    }

    // Start is called before the first frame update
    void Start() {
        //initialize the weather effect
        RainVFX.SetFloat("Intensity", RainIntensity);
        HailVFX.SetFloat("Intensity", HailIntensity);
        SnowVFX.SetFloat("Intensity", SnowIntensity);
        FogVolume.weight = FogIntensity;

        //initialize the fog volume
        FogVolume.sharedProfile.TryGet<Fog>(out _cachedFogComponent);
        if (_cachedFogComponent != null) {
            _cachedFogComponent.meanFreePath.Override(Mathf.Lerp(MaxFogAttenuationDistance,
                                                               MinFogAttenuationDistance,
                                                               FogIntensity));
        }

        //initialize the sky volume
        SkyAndFogVolume.sharedProfile.TryGet<PhysicallyBasedSky>(out _cachedPhysicalSkyComponent);

        //initialize the cloud volume
        CloudVolume.sharedProfile.TryGet<VolumetricClouds>(out _cachedCloudComponent);
        CloudVolume.weight = 1f;
        if (_cachedCloudComponent != null) {
            _cachedCloudComponent.cloudPreset = VolumetricClouds.CloudPresets.Custom;
            _cachedCloudComponent.sunLightDimmer.Override(1f);
            _cachedCloudComponent.ambientLightProbeDimmer.Override(1f);
        }

        //initialize the cloud moving parameters
        SetCloudMoveSpeed(CloudMoveSpeed);
        SetCloudMoveDirection(CloudMoveDirection);
        SetCloudType(CloudType.Cloudy);
        ChangeWeather(DefaultWeather, 0f);
    }

    // Update is called once per frame
    void Update() {
        if (DEBUG_PerformTransition) {
            DEBUG_PerformTransition = false;
            ChangeWeather(DEBUG_TargetPreset, DEBUG_TransitionTime);
        }

        UpdateTime(CurrentTime);
        UpdateStarTexture();
        UpdateCloudType();

        if (RainIntensity != _previousRainIntensity) {
            _previousRainIntensity = RainIntensity;
            RainVFX.SetFloat("Intensity", RainIntensity);
        }
        if (HailIntensity != _previousHailIntensity) {
            _previousHailIntensity = HailIntensity;
            HailVFX.SetFloat("Intensity", HailIntensity);
        }
        if (SnowIntensity != _previousSnowIntensity) {
            _previousSnowIntensity = SnowIntensity;
            SnowVFX.SetFloat("Intensity", SnowIntensity);
        }
        if (FogIntensity != _previousFogIntensity) {
            _previousFogIntensity = FogIntensity;
            FogVolume.weight = FogIntensity;
            if (_cachedFogComponent != null)
                _cachedFogComponent.meanFreePath.value = Mathf.Lerp(MaxFogAttenuationDistance,
                                                                   MinFogAttenuationDistance,
                                                                   FogIntensity);
        }
    }
}
