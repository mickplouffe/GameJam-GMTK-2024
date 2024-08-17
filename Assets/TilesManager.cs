using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
struct HexTile
{
    public Transform tileTransform;
    public float weight;
    public HexTile(Transform tileTransform, float weight)
    {
        this.tileTransform = tileTransform;
        this.weight = weight;
    }
}
public class TilesManager : MonoBehaviour
{
    [SerializeField] private List<HexTile> _hexTiles;
    [SerializeField] private Transform _tileParent;
    [SerializeField] private Vector3 currentTilesNormal;
    

    private void Awake()
    {
        currentTilesNormal = Vector3.up;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            RotateTiles();
    }

    private Vector3 CalculateCenterOfMass()
    {
        Vector3 totalWeightedPosition = Vector3.zero;
        float totalWeight = 0f;

        foreach (HexTile tile in _hexTiles)
        {
            totalWeightedPosition += tile.tileTransform.position * tile.weight;
            totalWeight += tile.weight;
        }

        return totalWeightedPosition / totalWeight;
    }

    private void RotateTiles()
    {
        Vector3 centerOfMass = CalculateCenterOfMass();
        Quaternion newPlaneRotation = Quaternion.FromToRotation(currentTilesNormal, centerOfMass);
        currentTilesNormal = newPlaneRotation * currentTilesNormal;

       // foreach (var tile in _tiles)
        _tileParent.rotation = newPlaneRotation;
    }

    [Button]
    private void UpdateTiles()
    {
        _hexTiles =  GameObject
            .FindGameObjectsWithTag("HexTile")
            .Select(hexTile => new HexTile(hexTile.transform, 10))
            .ToList();
    }

    public Transform GetClosestTileToPosition(Vector3 position)
    {
        if (_hexTiles.Count == 0)
            return null;
        
        float minDistance = Mathf.Infinity;
        Transform closestTile = null;
        foreach (var hexTile in _hexTiles)
        {
            if(Vector3.Distance(hexTile.tileTransform.position, position) > minDistance)
                continue;

            minDistance = Vector3.Distance(hexTile.tileTransform.position, position);
            closestTile = hexTile.tileTransform;
        }

        return closestTile;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, currentTilesNormal * 2.0f);
    }
}
