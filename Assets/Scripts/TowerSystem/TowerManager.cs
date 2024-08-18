using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : MonoBehaviourSingletonPersistent<TowerManager>
{
    [SerializeField] private GameObject defaultTowerPrefab;
    [SerializeField] private GameObject rockTowerPrefab;
    
    [SerializeField] private UpgradeTowerAction upgradeDamageAction;
    [SerializeField] private UpgradeTowerAction upgradeRangeAction;
    [SerializeField] private UpgradeTowerAction upgradeWeightAction;

    [SerializeField] private GameObject selectedTower;
    [SerializeField] private TowerController activeTowerSelected;
    [SerializeField] private LayerMask tilesLayerMask;
    [SerializeField] private LayerMask towersLayerMask;
    [SerializeField] private float yOffset = 0.5f;

    [SerializeField] private int coins = 10;

    [SerializeField] private UIEventChannel uiEventChannel;
    
    private List<Transform> _activeTowers;

    private bool _placementMode;

    public override void Awake()
    {
        base.Awake();
        _activeTowers = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) && _placementMode)
        {
            _placementMode = false;
            Destroy(selectedTower.gameObject);
        }

        if (_placementMode)
        {
            Transform hitTile = FindTransformBasedOnLayer(tilesLayerMask);
            if (hitTile)
                SnapTowerToTile(hitTile);
            else if(selectedTower)
                selectedTower.transform.position = Vector3.up * 100.0f;
        }

        if (Input.GetMouseButtonDown(0) && _placementMode)
        {
            Transform hitTile = FindTransformBasedOnLayer(tilesLayerMask);
            if (hitTile)
            {
                SnapTowerToTile(hitTile);
                RegisterTower(selectedTower.transform);
                selectedTower = null;
                _placementMode = false;
            }
            else
            {
                Destroy(selectedTower);
            }
        }

        if (Input.GetMouseButtonDown(0) && !_placementMode)
        {
            Transform hitTower = FindTransformBasedOnLayer(towersLayerMask);
            if (hitTower)
            {
                // Change the previously selected object the material
                if(activeTowerSelected)
                    activeTowerSelected.GetComponent<Renderer>().material = activeTowerSelected.towerData.defaultMaterial;
                
                activeTowerSelected = hitTower.GetComponent<TowerController>();
                // TODO: Also add highlight material, for now just change the color
                activeTowerSelected.GetComponent<Renderer>().material = activeTowerSelected.towerData.selectedMaterial;
                
                // TODO: RaiseActivateBuildMenu
                uiEventChannel.RaiseActivateActionsMenu();
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                
                // Change the previously selected object the material
                if(activeTowerSelected)
                    activeTowerSelected.GetComponent<Renderer>().material = activeTowerSelected.towerData.defaultMaterial;

                activeTowerSelected = null;
                
                
                // TODO: RaiseActivateActionsMenu
                uiEventChannel.RaiseActivateBuildMenu();
            }
            // TODO: maybe deselect when user presses e.g. escape. If we de-select based on hit or no hit then there is 
            // the problem that when pressing one of the buttons it will first deselect the item and then do the action associated to the button which will not do anything in the end
        }
    }

    public void HandleBuildTower(int towerType)
    {
        if(selectedTower)
            Destroy(selectedTower.gameObject);

        // Instantiate the tower
        switch (towerType)
        {
            case 0: // Default Tower
                selectedTower = Instantiate(defaultTowerPrefab, Vector3.up * 100.0f, Quaternion.identity);
                break;
            case 1: // Rock Tower
                selectedTower = Instantiate(rockTowerPrefab, Vector3.up * 100.0f, Quaternion.identity);
                break;
        }
        _placementMode = true;
        // maybe change material?
    }

    public void HandleUpgrade(int upgradeType)
    {
        UpgradeTowerAction upgradeAction = null;

        switch (upgradeType)
        {
            case 0: // Upgrade Damage
                upgradeAction = upgradeDamageAction;
                break;
            case 1: // Upgrade Range
                upgradeAction = upgradeRangeAction;
                break;
            case 2: // ReduceWeight
                upgradeAction = upgradeWeightAction;
                break;
            // Add more cases if needed
        }

        if (upgradeAction == null || !CanAfford(upgradeAction.costModifier) || activeTowerSelected == null || !activeTowerSelected.towerData.isUpgradable || !activeTowerSelected.CanUpgrade(upgradeAction)) 
            return;
        
        activeTowerSelected.UpgradeTower(upgradeAction);
        
        SpendCoins(upgradeAction.costModifier);
    }

    public void HandleSellItem()
    {
        if (!activeTowerSelected)
            return;
        
        AddCoins((int)(activeTowerSelected.instanceData.currentCost * 0.8f));
        // Unregister selected tower
        UnregisterTower(activeTowerSelected.transform);
        // Destroy selected tower
        Destroy(activeTowerSelected.gameObject);
        
        uiEventChannel.RaiseActivateBuildMenu();
        // TODO: Send event that player should receive part of money back

    }
    
    private bool CanAfford(int cost)
    {
        return cost <= coins;
    }

    private void SpendCoins(int cost)
    {
        coins -= cost;
    }
    
    private void AddCoins(int newCoins)
    {
        coins += newCoins;
    }

    private Transform FindTransformBasedOnLayer(LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        return Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask) ? hit.transform : null;
    }
    
    private void SnapTowerToTile(Transform tile)
    {
        Bounds towerBounds = selectedTower.GetComponent<Collider>().bounds;
        Bounds tileBounds = selectedTower.GetComponent<Collider>().bounds; // Keep this here in case 

        selectedTower.transform.position = tile.position + tile.up * (towerBounds.size.y + yOffset) * 0.5f;
        selectedTower.transform.rotation = tile.rotation;
    }

    private void RegisterTower(Transform tower)
    {
        // Register new tower
        _activeTowers.Add(tower);
        
        // TODO: Send event to let tiles now a new tower was added
        // RaiseTowerAddedToTileEvent(Transform tower)
    }
    
    private void UnregisterTower(Transform tower)
    {
        // Register new tower
        _activeTowers.Remove(tower);
        // TODO: Send event to let tiles now a new tower was removed
        // RaiseTowerAddedToTileEvent(Transform tower)
    }
}
