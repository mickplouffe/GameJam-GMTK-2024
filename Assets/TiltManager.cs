using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class TiltManager : MonoBehaviourSingletonPersistent<TiltManager>
{
    [SerializeField] private WeightEventChannel weightEventChannel;
    [Range(0.0f, 5.0f)]
    [SerializeField] private float tiltAmount;
    [SerializeField] private float rotationSpeed;

    private Dictionary<HexTile, float> weightAtlas;
    
    public Vector3 CenterOfMass { get; set; }

    public override void Awake()
    {
        base.Awake();
        weightAtlas = new Dictionary<HexTile, float>();
    }

    private void Start()
    {
        CenterOfMass = HexGridManager.Instance.transform.position;
        UpdateCenterOfMass();
    }

    private void OnEnable()
    {
        weightEventChannel.OnWeightAdded += HandleWeightAdded;
        weightEventChannel.OnWeightRemoved += HandleWeightRemoved;
    }

    private void OnDisable()
    {
        weightEventChannel.OnWeightAdded -= HandleWeightAdded;
        weightEventChannel.OnWeightRemoved -= HandleWeightRemoved;
    }

    private void UpdateCenterOfMass()
    {
        Vector3 totalWeightedPosition = Vector3.zero;
        float totalWeight = 0.0f;
        
        foreach (HexTile tile in weightAtlas.Keys)
        {
            totalWeightedPosition += tile.TileObject.transform.position * weightAtlas[tile];
            totalWeight += weightAtlas[tile];
        }

        if (totalWeight > 0.0f)
            CenterOfMass = totalWeightedPosition / totalWeight;
        else
            CenterOfMass = Vector3.zero;
    }

    private void Update()
    {
        TiltPlane();
    }

    [Button]
    private void TiltPlane()
    {
        // Calculate tilt angles based on CoM
        float tiltX = tiltAmount * CenterOfMass.z;
        float tiltZ = -tiltAmount * CenterOfMass.x;

        // Apply tilt to the plane
        Quaternion tiltRotation = Quaternion.Euler(tiltX, 0, tiltZ);

        HexGridManager.Instance.transform.rotation = Quaternion.Slerp(HexGridManager.Instance.transform.rotation,
            tiltRotation * Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0), Time.deltaTime * rotationSpeed);
        // HexGridManager.Instance.transform.rotation = t; 
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
        UpdateCenterOfMass();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(Vector3.zero, CenterOfMass);
    }
}
