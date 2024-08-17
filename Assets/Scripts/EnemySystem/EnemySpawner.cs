using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] private Transform[] allPossibleSpawnPoints;

    private int _currentWaveIndex;
        

    private void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (_currentWaveIndex < waves.Length)
        {
            var currentWave = waves[_currentWaveIndex];
            yield return new WaitForSeconds(currentWave.waveDelay);

            if (currentWave.randomizeSpawnPoints)
                RandomizeSpawnPoints(currentWave);

            StartCoroutine(SpawnEnemiesInWave(currentWave));
            _currentWaveIndex++;
        }
    }

    private void RandomizeSpawnPoints(WaveConfig waveConfig)
    {
        // You can implement logic to randomize spawn points or select tiles
        waveConfig.spawnPoints =  allPossibleSpawnPoints;
    }

    private IEnumerator SpawnEnemiesInWave(WaveConfig waveConfig)
    {
        for (int i = 0; i < waveConfig.enemyPrefabs.Length; i++)
        {
            for (int j = 0; j < waveConfig.enemyCounts[i]; j++)
            {
                Transform spawnPoint = waveConfig.spawnPoints[Random.Range(0, waveConfig.spawnPoints.Length)];
                SpawnEnemy(waveConfig.enemyPrefabs[i], spawnPoint);
                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        GameObject enemy = EnemyObjectPool.Instance.GetEnemyObject();
        enemy.transform.position = spawnPoint.position;
        enemy.transform.rotation = Quaternion.identity;
    }
}
