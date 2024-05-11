using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ProbeSystem : MonoBehaviour
{
    [SerializeField, Range(0f, 24.0f)] float CurrentTime;
    [SerializeField] AnimationCurve FromNightToMorning;
    [SerializeField] AnimationCurve FromMorningToNoon;
    [SerializeField] AnimationCurve FromNoonToAfternoon;
    [SerializeField] AnimationCurve FromAfternoonToNight;

    string NightProbeName = "Midnight";
    float MidnightFactor = 0.0f;
    string MorningProbeName = "Morning";
    float MorningFactor = 6.0f;
    string NoonProbeName = "Noon";
    float NoonFactor = 12.0f;
    string AfternoonProbeName = "Afternoon";
    float AfternoonFactor = 18.0f;
    float EndofDay = 24.0f;
    float SectionLength = 6.0f;

    [ContextMenu("Set Morning")]
    public void UpdateMorning() {
        SetCurrentTime(6.0f);
    }

    [ContextMenu("Set Afternoon")]
    public void UpdateAfternoon() {
        SetCurrentTime(18.0f);
    }

    [ContextMenu("Set Noon")]
    public void UpdateNoon() {
        SetCurrentTime(12.0f);
    }

    [ContextMenu("Set Night")]
    public void UpdateNight() {
        SetCurrentTime(0.0f);
    }

    public void SetCurrentTime(float time)
    {
        CurrentTime = time;
        UpdateProbeInterpolation();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void UpdateProbeInterpolation()
    {
        float factor = 0.0f;
        string targetName = NightProbeName;
        if (CurrentTime < MorningFactor)
        {
            ProbeReferenceVolume.instance.lightingScenario = NightProbeName;
            float timeFactor = (CurrentTime - MidnightFactor) / SectionLength;
            factor = FromNightToMorning.Evaluate(timeFactor);
            targetName = MorningProbeName;
        }
        else if (CurrentTime < NoonFactor)
        {
            ProbeReferenceVolume.instance.lightingScenario = MorningProbeName;
            float timeFactor = (CurrentTime - MorningFactor) / SectionLength;
            factor = FromMorningToNoon.Evaluate(timeFactor);
            targetName = NoonProbeName;
        }
        else if (CurrentTime < AfternoonFactor)
        {
            ProbeReferenceVolume.instance.lightingScenario = NoonProbeName;
            float timeFactor = (CurrentTime - NoonFactor) / SectionLength;
            factor = FromNoonToAfternoon.Evaluate(timeFactor);
            targetName = AfternoonProbeName;
        }
        else if (CurrentTime < EndofDay)
        {
            ProbeReferenceVolume.instance.lightingScenario = AfternoonProbeName;
            float timeFactor = (CurrentTime - AfternoonFactor) / SectionLength;
            factor = FromAfternoonToNight.Evaluate(timeFactor);
            targetName = NightProbeName;
        }
        Debug.Log("Target Name:" +  targetName + " the factor:" + factor);
        ProbeReferenceVolume.instance.BlendLightingScenario(targetName, factor);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateProbeInterpolation();
    }
}
