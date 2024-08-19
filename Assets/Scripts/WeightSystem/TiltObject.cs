using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TiltObject : MonoBehaviour
{
    [FormerlySerializedAs("TiltMultiplier")]
    [Tooltip("The severity of the angle the platform will rotate to")]
    [SerializeField] float tiltMultiplier = 10f;

    [FormerlySerializedAs("tiltrate")]
    [Tooltip("The speed at which the platform will rotate to the desired position")]
    [SerializeField] float tiltRate;

    [FormerlySerializedAs("centerpointmarker")] [SerializeField] GameObject centerPointMarker;

    [HideInInspector] public List<WeightedObject> objects = new List<WeightedObject>();

    Quaternion _target;

    [FormerlySerializedAs("Slideangle")]
    [Tooltip("The angle at which the objects on the board will start to slide, ~340 is a good start")]
    [SerializeField] float slideAngle;

    [FormerlySerializedAs("Tiles")] public List<GameObject> tiles = new List<GameObject>();
    [FormerlySerializedAs("Enemies")] public List<GameObject> enemies = new List<GameObject>();

    [FormerlySerializedAs("tileweight")] [SerializeField] float tileWeight;
    [FormerlySerializedAs("enemyweight")] [SerializeField] float enemyWeight;

    [FormerlySerializedAs("SlideDirection")] public Vector3 slideDirection;
    
    
    // Added by Mick

    [SerializeField] private Vector3 _center;
    [SerializeField] GameObject centerPoint;

    
    private void Awake()
    {

    }
    
    
    void Update()
    { 
       
        transform.rotation = Quaternion.Slerp(transform.rotation, _target, Time.deltaTime * tiltRate);
        _center = GetCenter(this.transform);
        centerPoint.transform.position = _center;
    }
    
    Vector3 GetCenter(Transform obj)
    {
        Vector3 center = new Vector3();

        // Get all first level children
        List<Transform> children = new List<Transform>();
        foreach (Transform child in obj)
        {
            children.Add(child);
        }   
        
        // Get the center of all children
        foreach (Transform child in children)
        {
            center += child.position;
        }
        center /= obj.childCount;
    
        return center;
    }

    private void FixedUpdate()
    {
        Vector3 center = Vector3.zero;
        float totalweight = 0f;
        List<WeightedObject> objs = new List<WeightedObject>();

        foreach(WeightedObject wo in FindObjectsByType<WeightedObject>(FindObjectsSortMode.None) )
        {
            if (wo.falling == false)
            {
                objs.Add(wo);
            }
        }
        if (objs.Count > 0)
        {
            foreach (WeightedObject wo in objs)
            {
                center.x += wo.weight * (wo.transform.position.x - transform.position.x);
                //center.y += wo.weight * (wo.transform.position.y - transform.position.y);
                center.z += wo.weight * (wo.transform.position.z - transform.position.z);
                totalweight += wo.weight;
            }
            foreach (GameObject g in tiles)
            {
                center.x += tileWeight * (g.transform.position.x - transform.position.x);
                //center.y += wo.weight * (wo.transform.position.y - transform.position.y);
                center.z += tileWeight * (g.transform.position.z - transform.position.z);
                totalweight += tileWeight;
            }

            foreach (GameObject g in enemies)
            {
                //add weight for enemies
            }
            center = center / totalweight;
        }
        else
        {
            center = transform.position;
        }
       

        centerPointMarker.transform.position = center;

        float tiltAroundX = -transform.position.z + center.z;
        float tiltAroundZ = -transform.position.x + center.x;

        _target = Quaternion.Euler(tiltAroundX * tiltMultiplier, 0, -tiltAroundZ * tiltMultiplier);

        //Debug.Log(transform.eulerAngles.x);
        slideDirection = transform.position - center;
        
        if (Mathf.Abs(transform.eulerAngles.x) <= slideAngle || Mathf.Abs(transform.eulerAngles.z) <= slideAngle)
        {
            //Debug.Log("hi");
            foreach(WeightedObject wo in objs)
            {
                // wo.rb.constraints = RigidbodyConstraints.None;
                wo.transform.SetParent(null);
            }
        }
        else
        {
            //Debug.Log("ho");
            foreach (WeightedObject wo in objs)
            {
                // wo.rb.constraints = RigidbodyConstraints.FreezeAll;
                wo.transform.SetParent(this.transform);
            }
        }
    }
}
