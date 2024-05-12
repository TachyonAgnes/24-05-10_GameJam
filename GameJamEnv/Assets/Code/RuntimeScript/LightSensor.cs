using System.Collections.Generic;
using UnityEngine;
using System;

public class LightSensor : MonoBehaviour
{

    // Tips: add pointLight or spotLight object Under the lightObject, Change layer to ArtificialLight
    // Tips: add sensor object under the player object, add this script and bind moonlight
    // Tips: add a sphere collider to the lightObject, make sure the sphere collider is big enough to cover the light source
    // Tips: add a sphere collider and rigidbody to the sensor object, make sure the sphere collider is big enough to cover the player object


    // for moonlight, by grabing moonlight's direction, we use inverse direction to check if the playerObj can touch the moonlight
    // for artificial light sources, check if and only if the playerObj has touched the collision box of the artificial light sources

    // for the light source, the type of object should be GameObject, since the most important information we need is position, rotation, and isActive

    public event Action<bool> OnExposeStatusChanged;


    public bool IsExposedToLight { get => isExposedToLight; }

    // to start, we need to grab the reference of moonlight
    [SerializeField] private GameObject sunLight;

    // exposed status editor variables
    [SerializeField] private bool isExposedToLight = false;
    [SerializeField] private bool isDebug = false;
    [SerializeField] private float changeFormTime = 0.5f;
    [SerializeField] private LayerMask lightObstacleLayermask;
    [SerializeField] private LayerMask artificalLightLayerMask;
    
    // exposed status private variables
    private bool lastRecordedStatus = false;
    private float timeInStatus = 0f;
    private bool isEventFired = false;

    // We need a list of artificial light sources
    private List<GameObject> artificalLightSource = new List<GameObject>();


    void Start(){ 
    // check if the moonlight is null
        if(sunLight == null){
            // if it is null, we need to find the sun in the scene
            sunLight = GameObject.Find("Sun").GetComponent<GameObject>();
        }

        EnemyKilledHandler.OnEnemyKilled += EnemyKilledHandler_OnEnemyKilled;
    }

    private void EnemyKilledHandler_OnEnemyKilled(object sender, EnemyKilledHandler.OnEnemyKilledEventArgs e)
    {
        // remove artifical light
        if (artificalLightSource.Contains(e._artificalLight))
        {
            artificalLightSource.Remove(e._artificalLight);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // check if the playerObj has touched the artificial light sources
        // if layer equals to the layer of artificial light sources
        if(col.gameObject.layer == LayerMask.NameToLayer("ArtificialLight")){
            // grab the reference of the artificial light sources
            artificalLightSource.Add(col.gameObject);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        // check if the playerObj has exited the artificial light sources
        // if layer equals to the layer of artificial light sources
        if(col.gameObject.layer == LayerMask.NameToLayer("ArtificialLight")){
            // grab the reference of the artificial light sources
            artificalLightSource.Remove(col.gameObject);
        }
    }

    // check if playerObject exposed to the artificial light
    private bool CheckExposedToIndividualArtificialLight(GameObject lightSource)
    {
        // if pointlight
        if (lightSource.GetComponent<Light>().type == UnityEngine.LightType.Point)
        {
            //// do a raycast to check if the playerObj is in the light source
            //RaycastHit hit;
            //if (Physics.Raycast(lightSource.transform.position, transform.position - lightSource.transform.position, out hit))
            //{
            //    // if the sensor is in the light source
            //    if (hit.collider.gameObject == this.gameObject)
            //    {
            //        return true;
            //    }
            //}

            // do a raycast to check if the playerObj can see the light source
            RaycastHit hit;
            Ray ray = new Ray(transform.position, lightSource.transform.position - transform.position);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, artificalLightLayerMask))
            {
                return true;

                //// if target is light source
                //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ArtificialLight"))
                //{
                //}
            }
        }

        // if spotlight
        if (lightSource.GetComponent<Light>().type == UnityEngine.LightType.Spot)
        {
            // make sure the playerObj is in the cone of the light source
            Vector3 toObject = transform.position - lightSource.transform.position;
            float angleToSpotlight = Vector3.Angle(lightSource.transform.forward, toObject);

            if (angleToSpotlight <= lightSource.GetComponent<Light>().spotAngle / 2)
            {
                //// do a raycast to check if the playerObj is in the light source
                //RaycastHit hit;
                //if (Physics.Raycast(lightSource.transform.position, transform.position - lightSource.transform.position, out hit))
                //{
                //    // make sure the playerObj is in the cone of the light source
                //    if (hit.collider.gameObject == this.gameObject)
                //    {
                //        return true;
                //    }
                //}
                // do a raycast to check if the playerObj can see the light source
                RaycastHit hit;
                Ray ray = new Ray(transform.position, lightSource.transform.position - transform.position);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, artificalLightLayerMask))
                {
                    return true;

                    //// if target is light source
                    //if (hit.collider.gameObject.layer == LayerMask.NameToLayer("ArtificialLight"))
                    //{
                    //}
                }

            }
        }
        return false;
    }

    private bool CheckExposedToArtificalLight()
    {
        // check if the playerObj is exposed to the artificial light sources
        foreach (GameObject lightSource in artificalLightSource)
        {
            if (CheckExposedToIndividualArtificialLight(lightSource))
            {
                return true;
            }
        }

        return false;
    }


    private bool CheckExposedToNaturalLight()
    {
        RaycastHit hit;

        return !Physics.Raycast(transform.position, -sunLight.transform.forward, out hit, Mathf.Infinity, lightObstacleLayermask);
    }


    private void HandleExposeStatusEvent()
    {
        if (isExposedToLight != lastRecordedStatus)
        {
            lastRecordedStatus = isExposedToLight;
            timeInStatus = 0f;
            isEventFired = false;
        }

        // only fire event when time in status exceeds the set change form time.
        timeInStatus += Time.deltaTime;
        if ( timeInStatus > changeFormTime && !isEventFired )
        {
            isEventFired = true;
            OnExposeStatusChanged?.Invoke(isExposedToLight);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckExposedToArtificalLight())
        {
            isExposedToLight = true;

            if (isDebug)
            {
                Debug.Log("is exposed to artifical light");
            }
        }
        else if (CheckExposedToNaturalLight())
        {
            isExposedToLight = true;

            if (isDebug)
            {
                Debug.Log("is exposed to SunLight");
            }
        }
        else
        {
            isExposedToLight = false;
        }

        HandleExposeStatusEvent();
    }
}
