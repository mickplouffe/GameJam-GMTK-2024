using System.Collections;
using System.Collections.Generic;
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
    
    public HexTile Tile { get; set; }

    void Awake()
    {
        // Create a new instance-specific data object using the shared tower data
        instanceData = new TowerInstanceData(towerData);
    }

    public bool CanUpgrade(UpgradeTowerAction upgradeTowerAction)
    {
        bool canUpgrade = false;

        canUpgrade |= (instanceData.range + upgradeTowerAction.weightModifier < towerData.maxWeight);
        canUpgrade |= (instanceData.damage + upgradeTowerAction.damageModifier < towerData.maxDamage);
        canUpgrade |= (instanceData.range + upgradeTowerAction.rangeModifier < towerData.maxRange);
        canUpgrade |= !_hasUpgrade;

        return canUpgrade;
    }
    public void UpgradeTower(UpgradeTowerAction upgradeTowerAction)
    {
        instanceData.damage += upgradeTowerAction.damageModifier;
        instanceData.range += upgradeTowerAction.rangeModifier;
        instanceData.weight += upgradeTowerAction.weightModifier;
        instanceData.currentCost += upgradeTowerAction.costModifier;

        _hasUpgrade = true;
    }
}
