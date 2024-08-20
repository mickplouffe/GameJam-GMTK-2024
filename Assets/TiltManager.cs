using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class TiltManager : MonoBehaviourSingleton<TiltManager>
{
    [SerializeField] private WeightEventChannel weightEventChannel;
    [SerializeField] private TiltEventChannel tiltEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    
    [Range(0.0f, 5.0f)]
    [SerializeField] private float tiltAmount;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float currentTiltAngle;

    private Dictionary<HexTile, float> weightAtlas;
    
    public Vector3 CenterOfMass { get; set; }
    
    public Vector3 Torque { get; set; }
    
    public float SlideDirection { get; set; }

    public void Awake()
    {
        weightAtlas = new Dictionary<HexTile, float>();
    }

    private void Start()
    {
        CenterOfMass = Vector3.zero;
        UpdateCenterOfMass();
    }

    private void OnEnable()
    {
        weightEventChannel.OnWeightAdded += HandleWeightAdded;
        weightEventChannel.OnWeightRemoved += HandleWeightRemoved;
        gameManagerEventChannel.OnGameRestart += HandleGameRestart;
    }

    private void OnDisable()
    {
        weightEventChannel.OnWeightAdded -= HandleWeightAdded;
        weightEventChannel.OnWeightRemoved -= HandleWeightRemoved;
        gameManagerEventChannel.OnGameRestart -= HandleGameRestart;

    }
    
    private void Update()
    {
        TiltPlaneWithTorque();
    }

    private void HandleGameRestart()
    {
        weightAtlas.Clear();
        currentTiltAngle = 0.0f;
        Torque = Vector3.zero;
        CenterOfMass = Vector3.zero;
        HexGridManager.Instance.transform.rotation = Quaternion.identity;
    }
    private void UpdateCenterOfMass()
    {
        Vector3 totalWeightedPosition = Vector3.zero;
        float totalWeight = 0.0f;
        
        Vector3 netTorque = Vector3.zero;
        
        foreach (HexTile tile in weightAtlas.Keys)
        {
            totalWeightedPosition += tile.TileObject.transform.position * weightAtlas[tile];
            totalWeight += weightAtlas[tile];
            
            Vector3 offset = tile.TileObject.transform.position - HexGridManager.Instance.mainUnit.position;
            float distance = offset.magnitude;
            
            // Torque is perpendicular to the lever arm, so it's along the Y axis in this case
            Vector3 torque = Vector3.Cross(offset, Vector3.up) * (weightAtlas[tile] * distance);
            netTorque += torque;
        }

        if (totalWeight > 0.0f)
            CenterOfMass = totalWeightedPosition / totalWeight;
        else
            CenterOfMass = Vector3.zero;

        Torque = netTorque;
    }

    [Button]
    private void TiltPlane()
    {
        // Calculate tilt angles based on CoM
        float tiltX = -tiltAmount * CenterOfMass.x;
        float tiltZ = tiltAmount * CenterOfMass.z;

        // Apply tilt to the plane
        Quaternion tiltRotation = Quaternion.Euler(tiltX, 0, tiltZ);

        HexGridManager.Instance.transform.rotation = Quaternion.Slerp(HexGridManager.Instance.transform.rotation,
            tiltRotation * Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
        // HexGridManager.Instance.transform.rotation = t; 
    }

    private void TiltPlaneWithTorque()
    {
        // Net torque gives us the rotation axis and magnitude
        Vector3 tiltAxis = new Vector3(Torque.x, 0.0f, Torque.z).normalized;
        float tiltAngle = Torque.magnitude * tiltAmount;

        // Rotate the disk around the calculated axis
        Quaternion targetRotation = Quaternion.AngleAxis(-tiltAngle, tiltAxis);
        HexGridManager.Instance.hexGridTilt.transform.rotation = Quaternion.Slerp( HexGridManager.Instance.hexGridTilt.transform.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        currentTiltAngle = Mathf.Lerp(currentTiltAngle, tiltAngle, Time.deltaTime * rotationSpeed);
        
        // Calculate tilt direction based on Center of Mass
        Vector3 tiltDirection = Vector3.Cross(Torque.normalized, -HexGridManager.Instance.hexGridTilt.transform.up).normalized;
        tiltEventChannel.RaiseTiltChanged(Mathf.Abs(currentTiltAngle), tiltDirection);
    }
    
    private void HandleWeightAdded(float weight, HexTile hexTile)
    {
        if(weightAtlas.TryGetValue(hexTile, out _))
            weightAtlas[hexTile] += weight;
        else
            weightAtlas.Add(hexTile, weight);

        UpdateCenterOfMass();
    }

    private void HandleWeightRemoved(float weight, HexTile hexTile)
    {
        if (!weightAtlas.TryGetValue(hexTile, out _)) 
            return;
        
        weightAtlas[hexTile] = Mathf.Max(weightAtlas[hexTile] - weight, 0.0f);
        if (weightAtlas[hexTile] == 0.0f)
            weightAtlas.Remove(hexTile);
        UpdateCenterOfMass();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Vector3 tiltDirection = (CenterOfMass - Vector3.zero).normalized;
        Gizmos.DrawLine(Vector3.up * 5.0f, Vector3.up * 5.0f + tiltDirection * 2.0f);
    }
}
