using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraRigManager : MonoBehaviourSingleton<CameraRigManager>
{
    private CinemachineFreeLook _freeLook;
    [SerializeField] private float distance = 10f;
    [SerializeField] private float distanceOffset = 1f;
    [SerializeField] private float topRig;
    [SerializeField] private float middleRig;
    [SerializeField] private float bottomRig;
    
    // Start is called before the first frame update
    private void Awake()
    {
        if (!_freeLook)
        {
            _freeLook = GetComponent<CinemachineFreeLook>();

        }
    }

    private void Update()
    {
        // If right mouse button is pressed, set isMovingCamera to true
        if (Input.GetMouseButton(1))
        {
            _freeLook.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X");
            _freeLook.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y");
        }else
        {
            // Disable the Cinemachine FreeLook AxisControl script
            _freeLook.m_XAxis.m_InputAxisValue = 0;
            _freeLook.m_YAxis.m_InputAxisValue = 0;
        }
        
        // Multiply the Cinemachine orbits "radius" by the distance
        // Lerping to new distance
        _freeLook.m_Orbits[0].m_Radius = Mathf.Lerp(_freeLook.m_Orbits[0].m_Radius, topRig * (distance/10), Time.deltaTime);
        _freeLook.m_Orbits[1].m_Radius = Mathf.Lerp(_freeLook.m_Orbits[1].m_Radius, middleRig * (distance/10), Time.deltaTime);
        _freeLook.m_Orbits[2].m_Radius = Mathf.Lerp(_freeLook.m_Orbits[2].m_Radius, bottomRig * (distance/10), Time.deltaTime);
       
        distance = HexGridManager.Instance.gridSpan * distanceOffset;
    }

}
