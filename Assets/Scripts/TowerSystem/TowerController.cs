using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CristiEventSystem.EventChannels;
using NaughtyAttributes;
using TowerSystem;
using UnityEngine;

[System.Serializable]
public class TowerInstanceData
{
    public int damage;
    public float range;
    public float weight;
    public int currentCost;
    public TowerInstanceData(Tower towerData)
    {
        damage = towerData.damage;
        range = towerData.range;
        weight = towerData.weight;
        currentCost = towerData.cost;
    }
}

[Serializable]
public struct TowerUpgrade
{
    [SerializeField] public GameObject visual;
    [SerializeField] public Transform shootingSpot;
}

[RequireComponent(typeof(LineRenderer))]
public class TowerController : MonoBehaviour
{
    public Tower towerData; // ScriptableObject reference
    public TowerInstanceData instanceData;

    private bool _hasUpgrade;

    [SerializeField] private WeightEventChannel weightEventChannel;
    [SerializeField] private TowerEventChannel towerEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    
    public HexTile Tile { get; set; }
    
    [SerializeField] private float tiltAllowanceThreshold = 45.0f; // Angle at which the object starts sliding
    [SerializeField] private float slipSpeedMultiplier = 1f; // Speed at which the object slips
    [SerializeField] private float enemyWidthOffset= 0.5f;

    [SerializeField] private LineRenderer _shootingRay;
    [SerializeField] private Transform towerShootingSpot;

    // LineRenderer animation settings
    [SerializeField] private float beamDuration = 0.2f;
    [SerializeField] private float beamWidthAnimationSpeed = 3.0f;
    [SerializeField] private AnimationCurve beamWidthCurve;
    [SerializeField] private Gradient beamColorGradient;
    private float _nextFireTime; // Time at which the tower can fire again
    
    [SerializeField] private TiltEventChannel tiltEventChannel;
    
    private PriorityQueue<Transform> _targets = new(); // Priority queue to store enemies by distance to central unit
    private Transform _currentTarget;
    
    private bool isSliding; // Track whether the object is currently sliding
    private bool _prevIsSliding;
    private Vector3 tiltDirection; // Store the current tilt direction
    private float slipMagnitude; // Store the current sliding speed

    [SerializeField] public AK.Wwise.Event towerPlaceSFX;
    [SerializeField] public AK.Wwise.Event towerSellSFX;
    [SerializeField] public AK.Wwise.Event towerUpgradeSFX;
    [SerializeField] public AK.Wwise.Event towerSlideSFX;
    [SerializeField] public AK.Wwise.Event towerSlideStopSFX;
    [SerializeField] public AK.Wwise.Event towerFallSFX;
    [SerializeField] public AK.Wwise.Event towerAttackSFX;

    [SerializeField] private bool canSlide = true;

    [SerializeField] private List<TowerUpgrade> towerUpgrades;

    private const int TOWER_UOGRADE_REQUIRED_SIZE = 3;
    
    void Awake()
    {
        // Create a new instance-specific data object using the shared tower data
        instanceData = new TowerInstanceData(towerData);
        _shootingRay = GetComponent<LineRenderer>();
        UpdateColliderRange();
        
        towerUpgrades[0].visual.SetActive(true);
        towerShootingSpot = towerUpgrades[0].shootingSpot;
    }

    [Button]
    public void UpdateColliderRange()
    {
        GetComponent<SphereCollider>().radius = instanceData.range;
    }

    private void OnEnable()
    {
        enemyEventChannel.OnWaveCompleted += HandleWaveComplete;
        tiltEventChannel.OnTiltChanged += HandleTiltChanged;
        gameManagerEventChannel.OnGameRestart += HandleGameRestart;
    }

    private void OnDisable()
    {
        tiltEventChannel.OnTiltChanged -= HandleTiltChanged;
        gameManagerEventChannel.OnGameRestart -= HandleGameRestart;
        enemyEventChannel.OnWaveCompleted -= HandleWaveComplete;

    }

    private void Update()
    {
        if (TowerManager.Instance.selectedTower == gameObject)
            return;
        
        if (_currentTarget == null || !IsTargetInRange(_currentTarget) && !towerData.isStatic)
        {
            GetNextTarget();
        }
    
        if (_currentTarget != null && Time.time >= _nextFireTime && !towerData.isStatic)
        {
            FireAtTarget();
            _nextFireTime = Time.time + 1.0f / towerData.fireRate;
        }
        
        if(canSlide)
            return;
        
        if (!isSliding) 
            return;
        
        // Calculate new position based on tilt direction and slip magnitude
        Vector3 newPosition = transform.position + tiltDirection * slipMagnitude * Time.deltaTime;
    
        // Get the tile at the new position
        HexTile newTile = HexGridManager.Instance.GetTileAtWorldPosition(newPosition);
    
        // If the object has moved to a new tile
        if (newTile != null && newTile != Tile)
        {
            // Remove weight from the previous tile
            if (Tile != null)
                weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);
    
            // Update the current tile to the new tile
            Tile = newTile;
    
            // Add weight to the new tile
            weightEventChannel.RaiseWeightAdded(instanceData.weight, Tile);
            // Move the object to the new position
        }
        else if(newTile == null)
        {
            StopSliding();
        }
        
        transform.position = newPosition;
    }

    private IEnumerator DrawShootingRay()
    {
        // Set up the LineRenderer positions
        _shootingRay.SetPosition(0, towerShootingSpot.position);
        _shootingRay.SetPosition(1, _currentTarget.position + _currentTarget.up * 2.0f);
        _shootingRay.widthCurve = beamWidthCurve;
        _shootingRay.colorGradient = beamColorGradient;
        _shootingRay.enabled = true;
    
        float elapsedTime = 0f;
    
        // Animate the line renderer over time
        while (elapsedTime < beamDuration)
        {
            elapsedTime += Time.deltaTime;
           _shootingRay.startWidth = Mathf.Lerp(_shootingRay.startWidth, 0.0f, Time.deltaTime * beamWidthAnimationSpeed);
            if(_currentTarget != null)
                _shootingRay.SetPosition(1, _currentTarget.position);
            // Optionally, animate positions or other properties here
            // Example: Flickering effect
            _shootingRay.startColor = beamColorGradient.Evaluate(elapsedTime / beamDuration);
            _shootingRay.endColor = beamColorGradient.Evaluate(elapsedTime / beamDuration);
    
            yield return null;
        }
    
        // Disable the LineRenderer after the duration
        _shootingRay.enabled = false;
    }
    private void FireAtTarget()
    {
    
        towerAttackSFX.Post(gameObject);
        StartCoroutine(DrawShootingRay());
        // Example: If using projectiles
        // GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // projectile.GetComponent<Projectile>().SetTarget(_currentTarget);
        
        EnemyController enemy = _currentTarget.GetComponent<EnemyController>();
        if (enemy != null)
            enemy.TakeDamage(instanceData.damage);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) 
            return;
        
        float distanceToCentralUnit = Vector3.Distance(other.transform.position, HexGridManager.Instance.transform.position);
        _targets.Enqueue(other.transform, distanceToCentralUnit);
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Removing the enemy from the priority queue is not straightforward
            // Instead, we re-evaluate the closest target when necessary.
        }
    }
    
    public void GetNextTarget()
    {
        _currentTarget = _targets.Count > 0 ? _targets.Dequeue() : null; // Get the closest enemy to the central unit
    }
    
    private bool IsTargetInRange(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) <= instanceData.range + enemyWidthOffset && target.gameObject.activeInHierarchy;
    }

    private void HandleWaveComplete()
    {
        _targets.Clear();
    }
    
    private void HandleGameRestart()
    {
        Tile?.DetachTower();
        isSliding = false;
        Destroy(gameObject);
    }
    
    private void HandleTiltChanged(float tiltAngle, Vector3 direction)
    {
        if (TowerManager.Instance.selectedTower == gameObject)
            return;
        
        if (tiltAngle > tiltAllowanceThreshold)
        {
            // towerSlideSFX.Post(gameObject);
            if(Tile != null)
                Tile.DetachTower();
            StartSliding(direction);
        }
        else if (isSliding)
        {
            StopSliding();
        }
    }
    
    private void StartSliding(Vector3 direction)
    {
        isSliding = true;
        tiltDirection = direction;
        slipMagnitude = slipSpeedMultiplier * (transform.position - HexGridManager.Instance.transform.position).magnitude;
    }
    
    private void StopSliding()
    {
        // towerSlideStopSFX.Post(gameObject);
        isSliding = false;
        slipMagnitude = 0f;
        tiltDirection = Vector3.zero;
        
        // Ensure the object is properly snapped to the current tile
        HexTile finalTile = HexGridManager.Instance.GetTileAtWorldPosition(transform.position);
        if (finalTile != null)
        {
            // Remove weight from the previous tile
            if (Tile != null)
                weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);
    
            // Update the current tile to the final tile
            Tile = finalTile;
    
            // Add weight to the final tile
            weightEventChannel.RaiseWeightAdded(instanceData.weight, Tile);
            towerEventChannel.RaiseSnapToNewTile(gameObject.transform, Tile);
        } else
        {
            towerFallSFX.Post(gameObject);
            
            if (Tile != null)
                weightEventChannel.RaiseWeightRemoved(instanceData.weight, Tile);
            
            // If no valid tile is found or object is off the grid, destroy it
            towerEventChannel.RaiseTowerDestroyed(gameObject.transform);
            Destroy(gameObject);
        }
    }

    public bool CanUpgrade(UpgradeTowerAction upgradeTowerAction, int upgradeIndex)
    {
        if (towerUpgrades.Count != TOWER_UOGRADE_REQUIRED_SIZE || upgradeIndex >= TOWER_UOGRADE_REQUIRED_SIZE)
        {
            Debug.LogError($"Number of upgrades in towerUpgrades does not match the required number of upgrades. {towerUpgrades.Count} != {TOWER_UOGRADE_REQUIRED_SIZE}");
            return false;
        }

        if (_hasUpgrade)
            return false;
        
        bool canUpgrade = false;

        canUpgrade |= (instanceData.range + upgradeTowerAction.weightModifier <= towerData.maxWeight);
        canUpgrade |= (instanceData.damage + upgradeTowerAction.damageModifier <= towerData.maxDamage);
        canUpgrade |= (instanceData.range + upgradeTowerAction.rangeModifier <= towerData.maxRange);

        return canUpgrade;
    }
    public void UpgradeTower(UpgradeTowerAction upgradeTowerAction, int upgradeIndex)
    {
        if (upgradeIndex >= towerUpgrades.Count)
            return;
        
        _hasUpgrade = true;

        towerUpgrades[0].visual.SetActive(false); // 0 is always the default starting tower visual
        towerUpgrades[upgradeIndex].visual.SetActive(true);
        towerShootingSpot = towerUpgrades[upgradeIndex].shootingSpot;
        
        towerUpgradeSFX.Post(gameObject);
        
        instanceData.damage += upgradeTowerAction.damageModifier;
        instanceData.range += upgradeTowerAction.rangeModifier;
        instanceData.weight += upgradeTowerAction.weightModifier;
        instanceData.currentCost += upgradeTowerAction.costModifier;
        
        if(upgradeTowerAction.weightModifier != 0.0f)
            weightEventChannel.RaiseWeightAdded(upgradeTowerAction.weightModifier, Tile);
        
        UpdateColliderRange();
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, instanceData.range);
        
        if(_currentTarget)
            Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
    }
}
