using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TiltObject : MonoBehaviour
{
    [SerializeField] float TiltMultiplier = 10f;
    [SerializeField] GameObject centerpointmarker;
    [SerializeField] float tiltrate;
   
    void Update()
    { 
        Vector3 center = Vector3.zero;
        float totalweight = 0f;

        foreach (WeightedObject wo in FindObjectsByType<WeightedObject>(FindObjectsSortMode.None))
        {

            center.x += wo.weight * (wo.transform.position.x - transform.position.x);
            center.y += wo.weight * (wo.transform.position.y - transform.position.y);
            center.z += wo.weight * (wo.transform.position.z - transform.position.z);
            totalweight += wo.weight;
        }
        center = center / totalweight;

        centerpointmarker.transform.position = center;

        float tiltAroundX = -transform.position.z + center.z;
        float tiltAroundZ = -transform.position.x + center.x;

        Quaternion target = Quaternion.Euler(tiltAroundX * TiltMultiplier, 0, -tiltAroundZ * TiltMultiplier);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * tiltrate);
    }
}
