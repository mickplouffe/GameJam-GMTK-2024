using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
    public Transform target;  // The object the camera will rotate around
    public float rotationStep = 45.0f;

    private Vector3 offset;  // Initial offset from the target

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for CameraRotateAround script.");
            return;
        }

        // Calculate the initial offset
        offset = transform.position - target.position;
    }

    void Update()
    {
        // Check for arrow key input and rotate the camera by 45 degrees around the target
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            RotateCamera(Vector3.up, rotationStep);
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
            RotateCamera(Vector3.up, -rotationStep);
    }

    void RotateCamera(Vector3 axis, float angle)
    {
        // Rotate the offset vector
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
        offset = rotation * offset;

        // Update the camera's position and keep it looking at the target
        transform.position = target.position + offset;
        transform.LookAt(target);
    }
}
