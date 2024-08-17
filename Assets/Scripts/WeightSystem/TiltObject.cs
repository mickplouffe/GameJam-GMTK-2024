using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TiltObject : MonoBehaviour
{
    [Tooltip("The severity of the angle the platform will rotate to")]
    [SerializeField] float TiltMultiplier = 10f;
    [Tooltip("The speed at which the platform will rotate to the desired position")]
    [SerializeField] float tiltrate;
    [SerializeField] GameObject centerpointmarker;
    [HideInInspector] public List<WeightedObject> objects = new List<WeightedObject>();
    Quaternion target;
    [SerializeField] float Slideangle;
    void Update()
    { 
       
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * tiltrate);
    }

    private void FixedUpdate()
    {
        Vector3 center = Vector3.zero;
        float totalweight = 0f;
        List<WeightedObject> objects = new List<WeightedObject>();
        foreach(WeightedObject wo in FindObjectsByType<WeightedObject>(FindObjectsSortMode.None) )
        {
            if (wo.falling == false)
            {
                objects.Add(wo);
            }
        }
        if (objects.Count > 0)
        {
            foreach (WeightedObject wo in objects)
            {

               
                    center.x += wo.weight * (wo.transform.position.x - transform.position.x);
                    //center.y += wo.weight * (wo.transform.position.y - transform.position.y);
                    center.z += wo.weight * (wo.transform.position.z - transform.position.z);
                    totalweight += wo.weight;
                
            }
            center = center / totalweight;
        }
        else
        {
            center = transform.position;
        }
       

        centerpointmarker.transform.position = center;

        float tiltAroundX = -transform.position.z + center.z;
        float tiltAroundZ = -transform.position.x + center.x;

        target = Quaternion.Euler(tiltAroundX * TiltMultiplier, 0, -tiltAroundZ * TiltMultiplier);
        
        Debug.Log(transform.eulerAngles.x);
        if (Mathf.Abs(transform.eulerAngles.x) <= Slideangle || Mathf.Abs(transform.eulerAngles.z) >= Slideangle)
        {
            //Debug.Log("hi");
            foreach(WeightedObject wo in objects)
            {
                wo.rb.constraints = RigidbodyConstraints.None;
                wo.transform.SetParent(null);
            }
        }
        else
        {
            //Debug.Log("ho");
            foreach (WeightedObject wo in objects)
            {
                wo.rb.constraints = RigidbodyConstraints.FreezeAll;
                wo.transform.SetParent(this.transform);
            }
        }
    }
}
