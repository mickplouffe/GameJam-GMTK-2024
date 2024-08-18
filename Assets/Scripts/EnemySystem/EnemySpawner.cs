using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] private List<HexTile> allPossibleSpawnPoints;

    [SerializeField] private EnemyEventChennl enemyEventChannel;
    
    private int _currentWaveIndex;
    private List<GameObject> _activeEnemies = new();

    private void OnEnable()
    {
        enemyEventChannel.OnEnemyKilled += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        enemyEventChannel.OnEnemyKilled -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (_currentWaveIndex < waves.Length)
        {
            var currentWave = waves[_currentWaveIndex];
            allPossibleSpawnPoints = HexGridManager.Instance.GetRandomEdgeTiles(currentWave.numberOfSpawnPoints);
            foreach (var spawnPoint in allPossibleSpawnPoints)
            {
                enemyEventChannel.RaiseWaveStart(spawnPoint.TileObject.GetComponent<HexTileController>(), currentWave.waveDelay);
            }
            yield return new WaitForSeconds(currentWave.waveDelay);
            
            yield return StartCoroutine(SpawnEnemiesInWave(currentWave));

            // Wait until all enemies from the current wave are destroyed
            yield return new WaitUntil(() => _activeEnemies.Count == 0);

            _currentWaveIndex++;
        }
    }

    private IEnumerator SpawnEnemiesInWave(WaveConfig waveConfig)
    {
        for (int i = 0; i < waveConfig.enemyPrefabs.Length; i++)
        {
            for (int j = 0; j < waveConfig.enemyCounts[i]; j++)
            {
                HexTile spawnPoint = allPossibleSpawnPoints[Random.Range(0, allPossibleSpawnPoints.Count)];
                GameObject enemy = SpawnEnemy(spawnPoint);

                _activeEnemies.Add(enemy); // Track the spawned enemy

                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }
    }

    private GameObject SpawnEnemy(HexTile spawnTile)
    {
        GameObject enemy = EnemyObjectPool.Instance.GetEnemyObject();
        
        enemy.transform.position = spawnTile.TileObject.transform.position;
        enemy.transform.rotation = Quaternion.identity;
        enemy.GetComponent<EnemyController>().SetupEnemy(spawnTile);

        return enemy;
    }

    private void HandleEnemyDestroyed(GameObject enemy)
    {
        _activeEnemies.Remove(enemy);
        EnemyObjectPool.Instance.ReturnEnemyObject(enemy);
    }
}
