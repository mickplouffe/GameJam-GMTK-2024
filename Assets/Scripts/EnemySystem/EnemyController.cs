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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public HexTile TargetTile { get; set; }
    public HexTile SourceTile { get; set; }

    [SerializeField] private float tilesPerSecond = 1.0f; // Movement speed in tiles per second
    [SerializeField] private EnemyEventChennl enemyEventChannel;
    [SerializeField] private WeightEventChannel weightEventChannel;
    [SerializeField] private CoinsEventChannel coinsEventChannel;

    private HexTile _currentTargetTile;
    private HexTile _currentSourceTile;
    private float _percentBetweenTiles;

    private bool _finishedSetup;

    [SerializeField] private float enemyWeight = 8.0f;
    [SerializeField] private int enemyKillCost = 5;

    private void OnEnable()
    {
        _finishedSetup = false;
    }

    private void OnDisable()
    {
        _finishedSetup = false;
    }

    public void SetupEnemy(HexTile sourceTile)
    {
        TargetTile = HexGridManager.Instance.GetTile(0,0);
        SourceTile = sourceTile;

        _currentSourceTile = SourceTile;
        _currentTargetTile = GetNextTargetPosition();
        transform.position = _currentSourceTile.TileObject.transform.position;
        
        weightEventChannel.RaiseWeightAdded(enemyWeight, _currentSourceTile);

        transform.position = _currentSourceTile.TileObject.transform.position +
                                  transform.up * GetComponent<Collider>().bounds.size.y * 0.5f;
        
        _finishedSetup = true;
    }

    private void Update()
    {
        if (!_finishedSetup)
            return;

        MoveTowardsTarget();

        transform.rotation = HexGridManager.Instance.transform.rotation;
    }

    private void MoveTowardsTarget()
    {
        float distanceBetweenTiles = Vector3.Distance(_currentTargetTile.TileObject.transform.position, _currentSourceTile.TileObject.transform.position);
        _percentBetweenTiles += Time.deltaTime * tilesPerSecond / distanceBetweenTiles;
        _percentBetweenTiles = Mathf.Clamp01(_percentBetweenTiles);

        Vector3 newPos = Vector3.Lerp(_currentSourceTile.TileObject.transform.position, _currentTargetTile.TileObject.transform.position, _percentBetweenTiles);

        if (_percentBetweenTiles >= 1.0f)
        {
            weightEventChannel.RaiseWeightRemoved(enemyWeight, _currentSourceTile);
            
            _percentBetweenTiles = 0.0f;
            _currentSourceTile = _currentTargetTile;
            _currentTargetTile = GetNextTargetPosition();
            
            weightEventChannel.RaiseWeightAdded(enemyWeight, _currentSourceTile);
            
            if (_currentTargetTile == TargetTile)
            {
                transform.position = new Vector3(TargetTile.TileObject.transform.position.x, transform.position.y, TargetTile.TileObject.transform.position.z);;
                enemyEventChannel.RaiseEnemyKilled(gameObject);

                weightEventChannel.RaiseWeightRemoved(enemyWeight, _currentSourceTile);
                coinsEventChannel.RaiseModifyCoins(enemyKillCost);

                return;
            }
        }

        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }

    private HexTile GetNextTargetPosition()
    {
        Vector3 targetDirection = (TargetTile.TileObject.transform.position - transform.position).normalized;
        List<HexTile> neighbours = HexGridManager.Instance.GetNeighbors(_currentSourceTile.Q, _currentSourceTile.R);
        // Find the closest neighbor in the direction of the target
        HexTile bestNeighbor = _currentSourceTile;
        float bestDot = -1f; // Start with the worst possible dot product
            
        foreach (var neighbor in neighbours)
        {
            Vector3 neighborWorldPosition = neighbor.TileObject.transform.position;
            Vector3 directionToNeighbor = (neighborWorldPosition - transform.position).normalized;

            // Calculate dot product to find the neighbor that aligns most with the target direction
            float dot = Vector3.Dot(targetDirection, directionToNeighbor);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestNeighbor = neighbor;
            }
        }

        return bestNeighbor;
    }
}
