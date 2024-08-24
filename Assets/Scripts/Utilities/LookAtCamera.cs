using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.forward = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
    }
}
