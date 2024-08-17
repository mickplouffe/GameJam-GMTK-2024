using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct Tile
{ 
    public Transform tileTransform;
    public float weight;
}
public class TilesManager : MonoBehaviour
{
    [SerializeField] private List<Tile> _tiles;
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

        foreach (Tile tile in _tiles)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, currentTilesNormal * 2.0f);
    }
}
