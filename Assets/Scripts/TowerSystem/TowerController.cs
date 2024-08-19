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
    private bool _prevIsSliding;
    private Vector3 tiltDirection; // Store the current tilt direction
    private float slipMagnitude; // Store the current sliding speed

    void Awake()
    {
        // Create a new instance-specific data object using the shared tower data
        instanceData = new TowerInstanceData(towerData);
    }

    private void OnEnable()
    {
        tiltEventChannel.OnTiltChanged += HandleTiltChanged;
    }

    private void OnDisable()
    {
        tiltEventChannel.OnTiltChanged -= HandleTiltChanged;
    }
    
    
private void Update()
{
    if (TowerManager.Instance.selectedTower == gameObject)
        return;
    
    if (!isSliding) 
        return;
    // Calculate new position based on tilt direction and slip magnitude
    Vector3 newPosition = transform.position + tiltDirection * slipMagnitude * Time.deltaTime;

    // Get the tile at the new position
    HexTile newTile = HexGridManager.Instance.GetTileAtWorldPosition(newPosition);

    // If the object has moved to a new tile
    if (newTile != null && newTile != Tile)
    {
        // Remove weight from the previous tile
        if (Tile != null)
            weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);

        // Update the current tile to the new tile
        Tile = newTile;

        // Add weight to the new tile
        weightEventChannel.RaiseWeightAdded(instanceData.weight, Tile);
        // Move the object to the new position
    }
    else if(newTile == null)
    {
        StopSliding();
    }
    
    transform.position = newPosition;

}

private void HandleTiltChanged(float tiltAngle, Vector3 direction)
{
    if (TowerManager.Instance.selectedTower == gameObject)
        return;
    if (tiltAngle > tiltAllowanceThreshold)
    {
        if(Tile != null)
            Tile.DetachTower();
        StartSliding(direction);
    }
    else if (isSliding)
    {
        StopSliding();
    }
}

private void StartSliding(Vector3 direction)
{
    isSliding = true;
    tiltDirection = direction;
    slipMagnitude = slipSpeedMultiplier * (transform.position - HexGridManager.Instance.transform.position).magnitude;
}

private void StopSliding()
{
    isSliding = false;
    slipMagnitude = 0f;
    tiltDirection = Vector3.zero;

    // Ensure the object is properly snapped to the current tile
    HexTile finalTile = HexGridManager.Instance.GetTileAtWorldPosition(transform.position);
    if (finalTile != null)
    {
        // Remove weight from the previous tile
        if (Tile != null)
            weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);

        // Update the current tile to the final tile
        Tile = finalTile;

        // Add weight to the final tile
        weightEventChannel.RaiseWeightAdded(instanceData.weight, Tile);
        towerEventChannel.RaiseSnapToNewTile(gameObject.transform, Tile);
    } else
    {
        if (Tile != null)
            weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);

            
        // If no valid tile is found or object is off the grid, destroy it
        towerEventChannel.RaiseTowerDestroyed(gameObject.transform);
        Destroy(gameObject);
    }
}

private bool ShouldStopSliding()
{
    // Stop sliding if the tilt is below the threshold and the object is relatively stable
    return tiltDirection.magnitude < 0.1f && slipMagnitude < 0.1f;
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
