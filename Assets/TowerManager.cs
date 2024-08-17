using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    UpgradeDamage,
    UpgradeRange,
    ReduceWeight
}
public class TowerManager : MonoBehaviourSingletonPersistent<TowerManager>
{
    [SerializeField] private Tower defaultTower;
    [SerializeField] private GameObject _selectedTower;
    [SerializeField] private GameObject _activeTowerSelected;
    [SerializeField] private LayerMask _tilesLayerMask;
    [SerializeField] private LayerMask _towersLayerMask;
    [SerializeField] private float yOffset = 0.5f;
    
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
            Destroy(_selectedTower.gameObject);
        }

        if (_placementMode)
        {
            Transform hitTile = FindTransformBasedOnLayer(_tilesLayerMask);
            if (hitTile)
                SnapTowerToTile(hitTile);
            else
                _selectedTower.transform.position = Vector3.up * 100.0f;
        }

        if (Input.GetMouseButtonDown(0) && _placementMode)
        {
            Transform hitTile = FindTransformBasedOnLayer(_tilesLayerMask);
            if (hitTile)
            {
                SnapTowerToTile(hitTile);
                RegisterTower(_selectedTower.transform);
                _selectedTower = null;
                _placementMode = false;
            }
            else
            {
                Destroy(_selectedTower);
            }
        }

        if (Input.GetMouseButtonDown(0) && !_placementMode)
        {
            Transform hitTower = FindTransformBasedOnLayer(_towersLayerMask);
            if (hitTower)
            {
                // Change the previously selected object the material
                if(_activeTowerSelected)
                    _activeTowerSelected.GetComponent<Renderer>().material = defaultTower.defaultMaterial;
                
                _activeTowerSelected = hitTower.gameObject;
                // TODO: Also add highlight material, for now just change the color
                _activeTowerSelected.GetComponent<Renderer>().material = defaultTower.selectedMaterial;
            }
            else if(_activeTowerSelected)
            {
                _activeTowerSelected.GetComponent<Renderer>().material = defaultTower.defaultMaterial;
                _activeTowerSelected = null;
            }
        }
    }

    public void HandleBuildTower()
    {
        if(_selectedTower)
            Destroy(_selectedTower.gameObject);
        
        // Instantiate the tower
        _selectedTower = Instantiate(defaultTower.prefab, Vector3.up * 100.0f, Quaternion.identity);
        _placementMode = true;
        // maybe change material?
    }

    public void HandleUpgrade(UpgradeType upgradeType)
    {
        switch (upgradeType)
        {
            case UpgradeType.UpgradeDamage:
                break;
            case UpgradeType.UpgradeRange:
                break;
            case UpgradeType.ReduceWeight:
                break;
        }
    }

    public void HandleSellItem()
    {
        if (!_activeTowerSelected)
            return;
        
        // Unregister selected tower
        UnregisterTower(_activeTowerSelected.transform);
        // Destroy selected tower
        Destroy(_activeTowerSelected);
        // TODO: Send event that player should receive part of money back
    }

    private Transform FindTransformBasedOnLayer(LayerMask layerMask)
    {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        return Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask) ? hit.transform : null;
    }
    
    private void SnapTowerToTile(Transform tile)
    {
        Bounds towerBounds = _selectedTower.GetComponent<Collider>().bounds;
        Bounds tileBounds = _selectedTower.GetComponent<Collider>().bounds; // Keep this here in case 

        _selectedTower.transform.position = tile.position + tile.up * (towerBounds.size.y + yOffset) * 0.5f;
        _selectedTower.transform.rotation = tile.rotation;
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
