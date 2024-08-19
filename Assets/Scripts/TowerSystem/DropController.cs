using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    [SerializeField] private Transform _selectedTower;
    [SerializeField] private LayerMask _groundLayerMask;
    [SerializeField] private float yOffset = 0.5f;
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;
        
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _groundLayerMask))
        {
            SnapTowerToTile(hit.transform);
        }
    }

    private void SnapTowerToTile(Transform tile)
    {
        Bounds towerBounds = _selectedTower.GetComponent<Collider>().bounds;
        Bounds tileBounds = _selectedTower.GetComponent<Collider>().bounds; // Keep this here in case 

        _selectedTower.position = tile.position + tile.up * (towerBounds.size.y + yOffset) * 0.5f;
    }
}
