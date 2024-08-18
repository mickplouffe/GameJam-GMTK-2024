// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class EnemyController : MonoBehaviour
// {
//     public Transform Target { get; set; }
//     public Transform Source { get; set; }
//
//     [SerializeField] private LayerMask _tileLayerMask;
//     
//     [SerializeField] private float tilesPerSecond = 1.0f; // Movement speed in tiles per second
//
//     [SerializeField] private EnemyEventChennl enemyEventChannel;
//     
//     private Vector3 _currentTargetPosition;     // The position of the target tile center
//     private Vector3 _currentSourcePosition;     // The position of the target tile center
//     private float _percentBetweenWaypoints;
//
//     private float _checkRadius = 1.5f;
//
//     private Vector3 _dirToTarget = Vector3.zero;
//
//     private bool _reachedTarget;
//     private bool _finishedSetup;
//     
//     private void OnEnable()
//     {
//         _finishedSetup = false;
//     }
//
//     private void OnDisable()
//     {
//         _finishedSetup = false;
//     }
//     
//     public void SetupEnemy(Transform source, Transform target)
//     {
//         Target = target;
//         Source = source;
//         
//         // Assuming the enemy starts at a specific tile's center
//         _currentSourcePosition = Source.position;
//
//         _dirToTarget = (Target.position - transform.position).normalized;
//         _currentTargetPosition = GetClosestTilePosition(_dirToTarget); // Implement this to get the starting tile's center
//         _finishedSetup = true;
//     }
//
//     private void Update()
//     {
//         if (!_finishedSetup)
//             return;
//         
//         // Ensure the enemy doesn't overshoot the target
//         if (Vector3.Distance(transform.position, Target.position) < 0.1f)
//         {
//             transform.position = new Vector3(Target.position.x, transform.position.y, Target.position.z);
//             enemyEventChannel.RaiseEnemyKilled(gameObject);
//             return;
//         }
//
//         float distanceBetweenTiles = Vector3.Distance(_currentTargetPosition, _currentSourcePosition);
//         _percentBetweenWaypoints += Time.deltaTime * tilesPerSecond / distanceBetweenTiles;
//         _percentBetweenWaypoints = Mathf.Clamp01 (_percentBetweenWaypoints);
//         
//         Vector3 newPos = Vector3.Lerp(_currentSourcePosition, _currentTargetPosition, _percentBetweenWaypoints);
//
//         if (_percentBetweenWaypoints >= 1.0f)
//         {
//             _percentBetweenWaypoints = 0.0f;
//             _currentSourcePosition = _currentTargetPosition;
//             _currentTargetPosition = GetNextTilePosition();
//
//             _reachedTarget = true;
//         }
//
//         transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
//     }
//
//     private Vector3 GetNextTilePosition()
//     {
//         return GetClosestTilePosition((Target.position - transform.position).normalized);
//     }
//
//     private Vector3 GetClosestTilePosition(Vector3 direction)
//     {
//         Collider[] hitColliders = Physics.OverlapSphere(transform.position, _checkRadius, _tileLayerMask);
//         Vector3 closestPosition = Vector3.zero;
//         float closestDistance = Mathf.Infinity;
//         float closestProjection = -Mathf.Infinity;
//         foreach (var hitCollider in hitColliders)
//         {
//             float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
//             Vector3 enemyToTile = (hitCollider.transform.position - transform.position).normalized;
//             float dotEnemyTile = Vector3.Dot(direction, enemyToTile);
//             
//             if(dotEnemyTile < 0.7f)
//                 continue;
//
//             if (distance > closestDistance) 
//                 continue;
//             
//             closestDistance = distance;
//             closestPosition = hitCollider.transform.position;
//             // closestProjection = Vector3.Dot(direction, enemyToTile);
//         }
//
//         return closestPosition;
//     }
//
//     private void OnDrawGizmos()
//     {
//         Gizmos.color = Color.cyan;
//         Gizmos.DrawWireSphere(transform.position, _checkRadius);
//         Gizmos.DrawLine(transform.position, Target.position);
//     }
// }

using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public HexTileController TargetTile { get; set; }
    public HexTileController SourceTile { get; set; }

    [SerializeField] private float tilesPerSecond = 1.0f; // Movement speed in tiles per second
    [SerializeField] private EnemyEventChennl enemyEventChannel;

    private HexTileController _currentTargetTile;
    private HexTileController _currentSourceTile;
    private float _percentBetweenTiles;

    private bool _finishedSetup;

    private void OnEnable()
    {
        _finishedSetup = false;
    }

    private void OnDisable()
    {
        _finishedSetup = false;
    }

    public void SetupEnemy(HexTileController sourceTile)
    {
        TargetTile = TileManager.Instance.GetTargetTile();
        SourceTile = sourceTile;

        _currentSourceTile = SourceTile;
        _currentTargetTile = TileManager.Instance.GetNextTile(_currentSourceTile, GetDirectionToTile(TargetTile));
        transform.position = _currentSourceTile.transform.position;

        _finishedSetup = true;
    }

    private void Update()
    {
        if (!_finishedSetup)
            return;

        MoveTowardsTarget();

    }

    private void MoveTowardsTarget()
    {
        float distanceBetweenTiles = Vector3.Distance(_currentTargetTile.transform.position, _currentSourceTile.transform.position);
        _percentBetweenTiles += Time.deltaTime * tilesPerSecond / distanceBetweenTiles;
        _percentBetweenTiles = Mathf.Clamp01(_percentBetweenTiles);

        Vector3 newPos = Vector3.Lerp(_currentSourceTile.transform.position, _currentTargetTile.transform.position, _percentBetweenTiles);

        if (_percentBetweenTiles >= 1.0f)
        {
            _percentBetweenTiles = 0.0f;
            _currentSourceTile = _currentTargetTile;
            _currentTargetTile = TileManager.Instance.GetNextTile(_currentSourceTile, GetDirectionToTile(TargetTile));
            
            if (_currentSourceTile == TargetTile)
            {
                transform.position = TargetTile.transform.position;
                enemyEventChannel.RaiseEnemyKilled(gameObject);
                return;
            }
        }

        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }

    private Vector3 GetDirectionToTile(HexTileController targetTile)
    {
        Vector3 direction = (targetTile.transform.position - transform.position).normalized;
        return new Vector3(Mathf.RoundToInt(direction.x), 0, Mathf.RoundToInt(direction.z));
    }
}
