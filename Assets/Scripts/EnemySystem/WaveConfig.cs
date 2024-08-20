using UnityEngine;

[CreateAssetMenu(menuName = "TowerDefense/WaveConfig")]
public class WaveConfig : ScriptableObject
{
    public GameObject[] enemyPrefabs;
    public int[] enemyCounts;
    public float spawnInterval;
    public float waveDelay;
    public int numberOfSpawnPoints;
    public GameObject dialogue;
}