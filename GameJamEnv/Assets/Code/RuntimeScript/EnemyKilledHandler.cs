using System;
using DG.Tweening;
using UnityEngine;


public class EnemyKilledHandler : MonoBehaviour
{
    public static event EventHandler<OnEnemyKilledEventArgs> OnEnemyKilled;
    public class OnEnemyKilledEventArgs : EventArgs
    {
        public GameObject _artificalLight;
    }

    [SerializeField] private GameObject artificalLight;
    [SerializeField] private GameObject enemyPortalVFX;

    private const string DISSOLVE = "_Dissolve";
    private Material[] enemyMaterials;
    private float duration = 1f;

    private void Start()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        enemyMaterials = new Material[mr.materials.Length];

        // replace enemy materials with material instance.
        for(int i = 0; i < enemyMaterials.Length; i++)
        {
            enemyMaterials[i] = new Material(mr.materials[i]);
        }

        mr.materials = enemyMaterials;
    }


    public void KilledByShadow()
    {
        AnimateDissolveMaterial();

        Destroy(gameObject, duration);

        GameObject vfx = Instantiate(enemyPortalVFX);
        vfx.transform.position = transform.position + new Vector3(0, 0.1f, 0f);
        Destroy(vfx, duration);

        OnEnemyKilled?.Invoke(this, new OnEnemyKilledEventArgs() { _artificalLight = artificalLight });
    }

    public void KilledByDagger()
    {

    }


    private void AnimateDissolveMaterial()
    {
        for (int i = 0; i < enemyMaterials.Length; i++)
        {
            enemyMaterials[i].DOFloat(1f, DISSOLVE, duration);

        }
    }
}
