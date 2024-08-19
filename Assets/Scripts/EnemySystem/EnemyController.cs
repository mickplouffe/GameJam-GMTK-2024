using System;
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
    [SerializeField] private int startHealth;
    
    private int _currentHealth;

    private Bounds _colliderBounds;

    public GameObject Prefab { get; set; }
    
    private void OnEnable()
    {
        _finishedSetup = false;
        _currentHealth = startHealth;
    }

    private void OnDisable()
    {
        _finishedSetup = false;
    }

    private void Awake()
    {
        _currentHealth = startHealth;
        _colliderBounds = GetComponent<Collider>().bounds;
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
                                  transform.up * _colliderBounds.size.y * 0.5f;
        
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
                transform.position = new Vector3(TargetTile.TileObject.transform.position.x, transform.position.y, TargetTile.TileObject.transform.position.z);
                
                KillEnemy();
                
                return;
            }
        }
        
        
        transform.position = newPos;
        transform.position += transform.parent.up * _colliderBounds.size.y * 0.5f;
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

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if(_currentHealth <= 0)
            KillEnemy();
        
        Debug.Log($"Enemy {gameObject.name} has been wounded, currentHealth: {_currentHealth}");
    }

    public void KillEnemy()
    {
        _currentHealth = startHealth;
        if(_currentSourceTile != null)
            weightEventChannel.RaiseWeightRemoved(enemyWeight, _currentSourceTile);
        coinsEventChannel.RaiseModifyCoins(enemyKillCost);
        enemyEventChannel.RaiseEnemyKilled(gameObject);
        EnemyObjectPool.Instance.ReturnEnemyObject(Prefab, gameObject);
    }
}
