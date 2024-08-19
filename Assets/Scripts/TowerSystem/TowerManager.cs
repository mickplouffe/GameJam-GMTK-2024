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

    public GameObject selectedTower;
    [SerializeField] private TowerController activeTowerSelected;
    [SerializeField] private LayerMask tilesLayerMask;
    [SerializeField] private LayerMask towersLayerMask;
    [SerializeField] private float yOffset = 0.5f;
    [SerializeField] private float sellCostPercentage = 0.1f;

    [SerializeField] private UIEventChannel uiEventChannel;
    [SerializeField] private WeightEventChannel weightEventChannel;
    [SerializeField] private CoinsEventChannel coinsEventChannel;
    [SerializeField] private TowerEventChannel towerEventChannel;
    
    [SerializeField] private float waitTimeBeforeCanSelect = 0.2f;
    private List<Transform> _activeTowers;

    [SerializeField] private bool _placementMode;

    public override void Awake()
    {
        base.Awake();
        _activeTowers = new List<Transform>();
    }

    private void OnEnable()
    {
        towerEventChannel.OnTowerDestroyed += UnregisterTower;
        towerEventChannel.OnSnapToNewTile += ForceSnapTowerToTile;
    }

    private void OnDisable()
    {
        towerEventChannel.OnTowerDestroyed += UnregisterTower;
        towerEventChannel.OnSnapToNewTile -= ForceSnapTowerToTile;
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
            if (!FindTransformBasedOnLayer(tilesLayerMask, out var hit) || !SnapTowerToTile(hit.point, true))
                selectedTower.transform.position = Vector3.up * 100.0f;
        }

        if (Input.GetMouseButtonDown(0) && !_placementMode)
        {
            if ( FindTransformBasedOnLayer(towersLayerMask, out var hit))
            {
                // Change the previously selected object the material
                if(activeTowerSelected)
                    activeTowerSelected.GetComponentInChildren<Renderer>().materials[0] = activeTowerSelected.towerData.defaultMaterial;
                
                activeTowerSelected = hit.transform.GetComponent<TowerController>();
                // TODO: Also add highlight material, for now just change the color
                activeTowerSelected.GetComponentInChildren<Renderer>().materials[0] = activeTowerSelected.towerData.selectedMaterial;
                
                // TODO: RaiseActivateBuildMenu
                uiEventChannel.RaiseActivateActionsMenu();
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                
                // Change the previously selected object the material
                if(activeTowerSelected)
                    activeTowerSelected.GetComponentInChildren<Renderer>().materials[0] = activeTowerSelected.towerData.defaultMaterial;

                activeTowerSelected = null;
                
                // TODO: RaiseActivateActionsMenu
                uiEventChannel.RaiseActivateBuildMenu();
            }
            // TODO: maybe deselect when user presses e.g. escape. If we de-select based on hit or no hit then there is 
            // the problem that when pressing one of the buttons it will first deselect the item and then do the action associated to the button which will not do anything in the end
        }
        
        if (Input.GetMouseButtonDown(0) && _placementMode)
        {
            if (FindTransformBasedOnLayer(tilesLayerMask, out var hit) && 
                SnapTowerToTile(hit.point) && 
                CoinsManager.Instance.CanBuy(selectedTower.GetComponent<TowerController>().instanceData.currentCost))
            {
                weightEventChannel.RaiseWeightAdded( selectedTower.GetComponent<TowerController>().instanceData.weight, 
                    selectedTower.GetComponent<TowerController>().Tile);
                
                RegisterTower(selectedTower.transform);
                selectedTower = null;
            }
            else
            {
                if(!CoinsManager.Instance.CanBuy(selectedTower.GetComponent<TowerController>().instanceData.currentCost))
                    uiEventChannel.RaiseCantBuy();
                
                Destroy(selectedTower);
            }
            _placementMode = false;
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

        if (upgradeAction == null || 
            !CoinsManager.Instance.CanBuy(upgradeAction.costModifier) ||
            activeTowerSelected == null || !activeTowerSelected.towerData.isUpgradable ||
            !activeTowerSelected.CanUpgrade(upgradeAction))
            return;

        activeTowerSelected.UpgradeTower(upgradeAction);

        coinsEventChannel.RaiseModifyCoins(-upgradeAction.costModifier);
    }

    public void HandleSellItem()
    {
        if (!activeTowerSelected)
            return;
        
        coinsEventChannel.RaiseModifyCoins(Mathf.CeilToInt(activeTowerSelected.instanceData.currentCost * sellCostPercentage));
        uiEventChannel.RaiseActivateBuildMenu();
        
        // Unregister selected tower
        weightEventChannel.RaiseWeightRemoved(activeTowerSelected.transform.GetComponent<TowerController>().instanceData.weight, 
            activeTowerSelected.transform.GetComponent<TowerController>().Tile);
        
        UnregisterTower(activeTowerSelected.transform);
        // Destroy selected tower
        Destroy(activeTowerSelected.gameObject);
    }
    
    private bool FindTransformBasedOnLayer(LayerMask layerMask, out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        return Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask,  QueryTriggerInteraction.Ignore) ? hit.transform : null;
    }

    private bool SnapTowerToTile(Vector3 hitPoint, bool preview = false)
    {
        Bounds towerBounds = selectedTower.GetComponent<Collider>().bounds;

        selectedTower.GetComponent<TowerController>().Tile = HexGridManager.Instance.GetTileAtWorldPosition(hitPoint);

        if (selectedTower.GetComponent<TowerController>().Tile == null)
            return false;

        if (!preview)
        {
            // Check if a tower already on the tile
            if (!selectedTower.GetComponent<TowerController>().Tile.HasTower())
                selectedTower.GetComponent<TowerController>().Tile.TowerObject = selectedTower.gameObject;
            else
            {
                return false;
            }
        }

        selectedTower.transform.position =
            selectedTower.GetComponent<TowerController>().Tile.TileObject.transform.position +
            selectedTower.GetComponent<TowerController>().Tile.TileObject.transform.up *
            (towerBounds.size.y + yOffset) * 0.5f;
        
        selectedTower.transform.rotation =
            selectedTower.GetComponent<TowerController>().Tile.TileObject.transform.rotation;
        
        selectedTower.transform.parent = HexGridManager.Instance.transform;

        return true;
    }
    
    private void ForceSnapTowerToTile(Transform tower, HexTile tile)
    {
        Bounds towerBounds = tower.GetComponent<Collider>().bounds;

        tower.position =
            tower.GetComponent<TowerController>().Tile.TileObject.transform.position +
            tower.GetComponent<TowerController>().Tile.TileObject.transform.up *
            (towerBounds.size.y + yOffset) * 0.5f;
        
        tower.GetComponent<TowerController>().Tile.TowerObject = tower.gameObject;
        
        tower.rotation =
            tower.GetComponent<TowerController>().Tile.TileObject.transform.rotation;
        
        tower.parent = HexGridManager.Instance.transform;
    }

    private void RegisterTower(Transform tower)
    {
        // Register new tower
        _activeTowers.Add(tower);
        
        coinsEventChannel.RaiseModifyCoins(-tower.GetComponent<TowerController>().instanceData.currentCost);
        // TODO: Send event to let tiles now a new tower was added
        // RaiseTowerAddedToTileEvent(Transform tower)
    }
    private void UnregisterTower(Transform tower)
    {
        // Register new tower
        _activeTowers.Remove(tower);
        
        if (tower.GetComponent<TowerController>().Tile != null)
        {
            tower.GetComponent<TowerController>().Tile.DetachTower();
            tower.GetComponent<TowerController>().Tile = null;
        }

        // TODO: Send event to let tiles now a new tower was removed
        // RaiseTowerAddedToTileEvent(Transform tower)
    }
}
