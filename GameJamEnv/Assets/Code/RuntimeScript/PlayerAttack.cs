using StarterAssets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LightSensor lightSensor;
    [SerializeField] private StarterAssetsInputs playerInput;
    [SerializeField] private LayerMask enemyLayerMask;

    [Space]
    [SerializeField] private AudioClip shadowAttackAudioClip;
    private List<EnemyKilledHandler> enemiesInRange = new List<EnemyKilledHandler>();

    private Coroutine afterKillEnvVFXCoroutine;
    private Coroutine bloodMoonCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if((enemyLayerMask.value & 1 << other.gameObject.layer) != 0)
        {
            if(other.gameObject.TryGetComponent(out EnemyKilledHandler enemy))
            {
                enemiesInRange.Add(enemy);
            }
            Debug.Log(enemiesInRange.Count);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((enemyLayerMask.value & 1 << other.gameObject.layer) != 0)
        {

            if(other.gameObject.TryGetComponent(out EnemyKilledHandler enemy))
            {
                if (enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Remove(enemy);
                }
            }
            Debug.Log(enemiesInRange.Count);

        }
    }

    private void Start()
    {
        playerInput.OnPlayerAttack += PlayerInput_OnPlayerAttack;
        playerInput.OnPlayerUltimate += PlayerInput_OnPlayerUltimate;
    }



    private void OnDestroy()
    {
        playerInput.OnPlayerAttack -= PlayerInput_OnPlayerAttack;
        playerInput.OnPlayerUltimate -= PlayerInput_OnPlayerUltimate;

    }



    private void PlayerInput_OnPlayerUltimate()
    {
        if (bloodMoonCoroutine != null)
        {
            StopCoroutine(bloodMoonCoroutine);
        }

        bloodMoonCoroutine = StartCoroutine(BloodMoonCoroutine());
    }


    private void PlayerInput_OnPlayerAttack()
    {
        if(enemiesInRange.Count > 0)
        {
            EnemyKilledHandler enemyToKill = enemiesInRange[0];

            if (lightSensor.IsExposedToLight)
            {
                
            }
            else
            {
                enemyToKill.KilledByShadow();

                // play sfx
                AudioSource.PlayClipAtPoint(shadowAttackAudioClip, transform.position, 1);


                // change weather
                if(afterKillEnvVFXCoroutine != null)
                {
                    StopCoroutine(afterKillEnvVFXCoroutine);
                }

                afterKillEnvVFXCoroutine = StartCoroutine(AfterKillEnvVFXCoroutine());
            }

            enemiesInRange.Remove(enemyToKill);
        }
    }



    private IEnumerator AfterKillEnvVFXCoroutine()
    {
        EnvironmentIlluminationSystem.Instance.SetRainny();
        yield return new WaitForSeconds(10f);
        EnvironmentIlluminationSystem.Instance.SetNormal();
    }



    private IEnumerator BloodMoonCoroutine()
    {
        EnvironmentIlluminationSystem.Instance.SetNightTime();
        yield return new WaitForSeconds(10f);
        EnvironmentIlluminationSystem.Instance.SetDayTime();
    }
}
