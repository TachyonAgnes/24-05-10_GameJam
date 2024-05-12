using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private LightSensor lightSensor;
    [SerializeField] private StarterAssetsInputs playerInput;
    [SerializeField] private LayerMask enemyLayerMask;

    private List<EnemyKilledHandler> enemiesInRange = new List<EnemyKilledHandler>();

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
    }

    private void OnDestroy()
    {
        playerInput.OnPlayerAttack -= PlayerInput_OnPlayerAttack;
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
            }

            enemiesInRange.Remove(enemyToKill);
        }
    }
}
