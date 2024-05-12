using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;


public class PlayerShadowSwitcher : MonoBehaviour
{
    private const string DISSOLVE_OFFSET = "_DissolveOffest";
    private const string HIDE = "Hide";

    [SerializeField] private Animator animator;
    [SerializeField] private LightSensor lightSensor;
    [SerializeField] private List<Material> playerMaterials = new List<Material>();
    [SerializeField] private float dissolveOffsetY_Exposed;
    [SerializeField] private float dissolveOffsetY_Hidden;


    [Space]
    [SerializeField] private GameObject shadowPlayerIndicator;

    [Space]
    [SerializeField] private GameObject hideVFXPrefab;

    [Space]
    [SerializeField] private AudioClip[] hideInShadowAudioClips;
    [SerializeField] private AudioClip exposedInLightAudioClip;



    private void Awake()
    {
        shadowPlayerIndicator.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lightSensor.OnExposeStatusChanged += LightSensor_OnExposeStatusChanged;
    }

    private void OnDestroy()
    {
        lightSensor.OnExposeStatusChanged -= LightSensor_OnExposeStatusChanged;
    }

    private void LightSensor_OnExposeStatusChanged(bool isExposed)
    {
        AnimateDissolveOffset(isExposed);

        if (!isExposed)
        {
            shadowPlayerIndicator.SetActive(true);
            animator.SetTrigger(HIDE);
            SpawnVFX();
        }
        else
        {
            shadowPlayerIndicator.SetActive(false);
        }

        PlaySFX(isExposed);
    }

    private void SpawnVFX()
    {
        GameObject vfx = Instantiate(hideVFXPrefab);
        vfx.transform.position = transform.position + new Vector3(0, 0.1f, 0f) + transform.forward * 0.5f;
        vfx.transform.rotation = Quaternion.Euler(90f, 0, 0);

        Destroy(vfx, 0.8f);
    }


    private void PlaySFX(bool isExposed)
    {
        if(isExposed)
        {
            AudioSource.PlayClipAtPoint(exposedInLightAudioClip, transform.position, 0.5f);
        }
        else
        {
            var index = UnityEngine.Random.Range(0, hideInShadowAudioClips.Length);
            AudioSource.PlayClipAtPoint(hideInShadowAudioClips[index], transform.position, 0.5f);
        }
    }


    private void AnimateDissolveOffset(bool isExposed)
    {
        for (int i = 0; i < playerMaterials.Count; i++)
        {
            Vector4 currValue = playerMaterials[i].GetVector(DISSOLVE_OFFSET);

            float targetY = isExposed ? dissolveOffsetY_Exposed : dissolveOffsetY_Hidden;
            Vector4 targetValue = new Vector4(currValue.x, targetY, currValue.z, 0);

            playerMaterials[i].DOVector(targetValue, DISSOLVE_OFFSET, 1f);
        }
    }
}
