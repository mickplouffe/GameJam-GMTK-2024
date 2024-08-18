using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class TowerInstanceData
{
    public int damage;
    public float range;
    public float weight;
    public int currentCost;
    public TowerInstanceData(Tower towerData)
    {
        damage = towerData.damage;
        range = towerData.range;
        weight = towerData.weight;
        currentCost = towerData.cost;
    }
}

public class TowerController : MonoBehaviour
{
    public Tower towerData; // ScriptableObject reference
    public TowerInstanceData instanceData;

    private bool _hasUpgrade;

    [SerializeField] private WeightEventChannel weightEventChannel;
    [SerializeField] private TowerEventChannel towerEventChannel;
    
    public HexTile Tile { get; set; }
    
    [SerializeField] private float tiltAllowanceThreshold = 45.0f; // Angle at which the object starts sliding
    [SerializeField] private float slipSpeedMultiplier = 1f; // Speed at which the object slips

    [SerializeField] private TiltEventChannel tiltEventChannel;
    
    private bool isSliding; // Track whether the object is currently sliding
    private Vector3 tiltDirection; // Store the current tilt direction
    private float slipMagnitude; // Store the current sliding speed

    void Awake()
    {
        // Create a new instance-specific data object using the shared tower data
        instanceData = new TowerInstanceData(towerData);
    }

    private void Update()
    {
        // If the object is sliding, move it every frame
        if (isSliding)
            transform.position += tiltDirection * slipMagnitude * Time.deltaTime;
    }
    
    private void OnEnable()
    {
        tiltEventChannel.OnTiltChanged += HandleTiltChanged;
    }

    private void OnDisable()
    {
        tiltEventChannel.OnTiltChanged -= HandleTiltChanged;
    }
    
    private void HandleTiltChanged(float tiltAngle, Vector3 direction)
    {
        // // Check if the tilt angle exceeds the object's tolerance
        // if (tiltAngle > tiltAllowanceThreshold)
        // {
        //     // Start sliding if the threshold is exceeded
        //     if (!isSliding)
        //     {
        //         isSliding = true;
        //     }
        //     if (Tile != null)
        //         weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);
        //
        //     Vector3 offset = transform.position - HexGridManager.Instance.transform.position;
        //     tiltDirection = direction;
        //     // Calculate sliding speed based on distance from the center
        //     float distanceFromCenter = offset.magnitude;
        //     slipMagnitude = slipSpeedMultiplier * distanceFromCenter;
        //
        //     if (HasAvailableTileWhileSliding())
        //         weightEventChannel.RaiseWeightAdded(instanceData.weight, Tile);
        //     
        // }
        // else
        // {
        //     // Stop sliding if the angle stabilizes below the threshold
        //     if (HasAvailableTileAfterSliding() && isSliding)
        //     {
        //         towerEventChannel.RaiseSnapToNewTile(gameObject.transform, Tile);
        //         return;
        //     }
        //     
        //     if(isSliding && Tile != null)
        //         towerEventChannel.RaiseTowerDestroyed(gameObject.transform);
        //     
        //     isSliding = false;
        // }
    }

    private bool HasAvailableTileWhileSliding()
    {
        Tile = HexGridManager.Instance.GetTileAtPosition(new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z));
        return Tile != null;
    }
    
    private bool HasAvailableTileAfterSliding()
    {
        Tile = HexGridManager.Instance.GetTileAtPosition(new Vector3(transform.localPosition.x, 0.0f, transform.localPosition.z));
        if (Tile == null)
            return false;
        
        if (!Tile.HasTower())
            return true;
        
        List<HexTile> neighbours = HexGridManager.Instance.GetNeighbors(Tile.Q, Tile.R);
        foreach (var neighbour in neighbours.Where(neighbour => !neighbour.HasTower()))
        {
            Tile = neighbour;
            return true;
        }

        return false;
    }

    public bool CanUpgrade(UpgradeTowerAction upgradeTowerAction)
    {
        bool canUpgrade = false;

        canUpgrade |= (instanceData.range + upgradeTowerAction.weightModifier <= towerData.maxWeight);
        canUpgrade |= (instanceData.damage + upgradeTowerAction.damageModifier <= towerData.maxDamage);
        canUpgrade |= (instanceData.range + upgradeTowerAction.rangeModifier <= towerData.maxRange);

        return canUpgrade;
    }
    public void UpgradeTower(UpgradeTowerAction upgradeTowerAction)
    {
        instanceData.damage += upgradeTowerAction.damageModifier;
        instanceData.range += upgradeTowerAction.rangeModifier;
        instanceData.weight += upgradeTowerAction.weightModifier;
        instanceData.currentCost += upgradeTowerAction.costModifier;
        
        if(upgradeTowerAction.weightModifier != 0.0f)
            weightEventChannel.RaiseWeightAdded(upgradeTowerAction.weightModifier, Tile);
    }
}
