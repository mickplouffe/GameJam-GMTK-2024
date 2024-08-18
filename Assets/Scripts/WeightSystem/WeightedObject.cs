using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedObject : MonoBehaviour
{
    public float weight  = 1.0f;
    [HideInInspector] public bool falling = false;
    [HideInInspector] public Rigidbody rb;
    TiltObject to;
    [SerializeField] LayerMask mask;
    [SerializeField] float falldistance;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        to = FindAnyObjectByType<TiltObject>();
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            //Debug.Log(rb.velocity.magnitude);
            if (rb.velocity.magnitude >= 0.5)
            {
                RaycastHit hit;
                
                if (Physics.Raycast(transform.position, new Vector3(to.gameObject.transform.eulerAngles.x, to.gameObject.transform.eulerAngles.y, to.gameObject.transform.eulerAngles.z), out hit, falldistance, mask))
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                    //Debug.Log("Did Hit");
                    falling = false;
                }
                else
                {
                    falling = true;
                }
            }
            else
            {
                falling = false ;
            }
        }
        
    }

}
