using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask _tileLayerMask;
    
    [SerializeField] private float tilesPerSecond = 1.0f; // Movement speed in tiles per second
    private Vector3 _currentTargetPosition;     // The position of the target tile center
    private Vector3 _currentSourcePosition;     // The position of the target tile center
    private float _percentBetweenWaypoints;

    private bool isMoving = false;      // Whether the enemy is currently moving
    
    private float _checkRadius = 1.5f;

    private Vector3 _dirToTarget;

    private bool _reachedTarget;

    private void OnEnable()
    {
        _dirToTarget = (target.position - transform.position).normalized;
    }

    private void Start()
    {
        _dirToTarget = (target.position - transform.position).normalized;
        
        // Assuming the enemy starts at a specific tile's center
        _currentSourcePosition = GetClosestTilePosition(_dirToTarget);
        transform.position =  transform.position = new Vector3(_currentSourcePosition.x, transform.position.y, _currentSourcePosition.z);
        
        _currentTargetPosition = GetClosestTilePosition(_dirToTarget); // Implement this to get the starting tile's center
    }

    private void Update()
    {
        if(transform.position == target.position)
            return;
        
        // Ensure the enemy doesn't overshoot the target
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            transform.position = new Vector3(target.position.x, transform.position.y, target.position.z);
            return;
        }

        float distanceBetweenTiles = Vector3.Distance(_currentTargetPosition, _currentSourcePosition);
        _percentBetweenWaypoints += Time.deltaTime * tilesPerSecond / distanceBetweenTiles;
        _percentBetweenWaypoints = Mathf.Clamp01 (_percentBetweenWaypoints);
        
        Vector3 newPos = Vector3.Lerp(_currentSourcePosition, _currentTargetPosition, _percentBetweenWaypoints);

        if (_percentBetweenWaypoints >= 1.0f)
        {
            _percentBetweenWaypoints = 0.0f;
            _currentSourcePosition = _currentTargetPosition;
            _currentTargetPosition = GetNextTilePosition();
        }

        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }

    private Vector3 GetNextTilePosition()
    {
        return GetClosestTilePosition((target.position - transform.position).normalized);
    }

    private Vector3 GetClosestTilePosition(Vector3 direction)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _checkRadius, _tileLayerMask);
        Vector3 closestPosition = Vector3.zero;
        float closestDistance = Mathf.Infinity;
        float closestProjection = -Mathf.Infinity;
        foreach (var hitCollider in hitColliders)
        {
            float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
            Vector3 enemyToTile = (hitCollider.transform.position - transform.position).normalized;
            float dotEnemyTile = Vector3.Dot(direction, enemyToTile);
            
            if(dotEnemyTile < 0.7f)
                continue;

            if (distance > closestDistance) 
                continue;
            
            closestDistance = distance;
            closestPosition = hitCollider.transform.position;
            // closestProjection = Vector3.Dot(direction, enemyToTile);
        }

        return closestPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _checkRadius);
        Gizmos.DrawLine(transform.position, target.position);
    }
}
