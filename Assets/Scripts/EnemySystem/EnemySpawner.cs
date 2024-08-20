using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviourSingleton<EnemySpawner>
{
    [SerializeField] public WaveConfig[] waves;
    private List<HexTile> _allPossibleSpawnPoints;

    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;
    
    [SerializeField] private AK.Wwise.Event waveStartedSFX;
    [SerializeField] private AK.Wwise.Event waveEndedSFX;

    [SerializeField] private AK.Wwise.Event waveStartEvent;
    [SerializeField] private AK.Wwise.Event betweenWavesEvent;

    public int CurrentWaveIndex { get; set; }
    private List<GameObject> _activeEnemies = new();

    private void OnEnable()
    {
        enemyEventChannel.OnEnemyKilled += HandleEnemyDestroyed;
        enemyEventChannel.OnStartNextWave += () => StartCoroutine(StartNextWave());
        gameManagerEventChannel.OnGameRestart += HandleGameRestart;
    }

    private void HandleGameRestart()
    {
        // Stop all coroutines currently running on this MonoBehaviour
        StopAllCoroutines();
        
        CurrentWaveIndex = 0;
        _activeEnemies.Clear();
    }

    private void OnDisable()
    {
        enemyEventChannel.OnEnemyKilled -= HandleEnemyDestroyed;
        enemyEventChannel.OnStartNextWave -= () => StartCoroutine(StartNextWave());
        //gameManagerEventChannel.OnGameRestart -= HandleGameRestart;
    }

    public IEnumerator StartNextWave()
    {
        waveStartEvent.Post(gameObject);
        waveStartedSFX.Post(gameObject);
        yield return StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        if (CurrentWaveIndex >= waves.Length) 
            yield break;
        
        var currentWave = waves[CurrentWaveIndex];
        _allPossibleSpawnPoints = HexGridManager.Instance.GetRandomEdgeTiles(currentWave.numberOfSpawnPoints);

        foreach (var spawnPoint in _allPossibleSpawnPoints)
            enemyEventChannel.RaiseWaveStart(spawnPoint.TileObject.GetComponent<HexTileController>(), currentWave.waveDelay);
        
        yield return new WaitForSeconds(currentWave.waveDelay);

        yield return StartCoroutine(SpawnEnemiesInWave(currentWave));

        // Wait until all enemies from the current wave are destroyed
        yield return new WaitUntil(() => _activeEnemies.Count == 0);
        
        CurrentWaveIndex++;

        waveEndedSFX.Post(gameObject);
        betweenWavesEvent.Post(gameObject);
        enemyEventChannel.RaiseWaveCompleted();
    }

    private IEnumerator SpawnEnemiesInWave(WaveConfig waveConfig)
    {
        for (int i = 0; i < waveConfig.enemyPrefabs.Length; i++)
        {
            for (int j = 0; j < waveConfig.enemyCounts[i]; j++)
            {
                HexTile spawnPoint = _allPossibleSpawnPoints[Random.Range(0, _allPossibleSpawnPoints.Count)];
                GameObject enemy = SpawnEnemy(spawnPoint, waveConfig.enemyPrefabs[i]);

                _activeEnemies.Add(enemy); // Track the spawned enemy

                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }
    }

    private GameObject SpawnEnemy(HexTile spawnTile, GameObject enemyPrefab)
    {
        // Get a pooled enemy or instantiate a new one
        GameObject enemy = EnemyObjectPool.Instance.GetEnemyObject(enemyPrefab);

        // Set the prefab reference in the EnemyController to the original prefab, not the instantiated object
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.Prefab = enemyPrefab; // Set the original prefab, not the instantiated clone

        // Position and initialize the enemy
        enemy.transform.position = spawnTile.TileObject.transform.position;
        enemy.transform.rotation = Quaternion.identity;
        enemyController.SetupEnemy(spawnTile);

        return enemy;
    }

    private void HandleEnemyDestroyed(GameObject enemy)
    {
        // Assuming EnemyController knows its prefab type, otherwise pass the prefab type to the event.
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            EnemyObjectPool.Instance.ReturnEnemyObject(enemyController.Prefab, enemy);
        }

        _activeEnemies.Remove(enemy);
    }
}
