using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/Enemy Event Channel")]
public class EnemyEventChennl : ScriptableObject
{
    public UnityAction<GameObject> OnEnemyKilled;
    public UnityAction OnWaveCompleted;
    public UnityAction OnAllWaveCompleted;
    public UnityAction<HexTileController, float> OnTileFlashing;

    public void RaiseEnemyKilled(GameObject enemy)
    {
        OnEnemyKilled?.Invoke(enemy);
    }

    public void RaiseWaveCompleted()
    {
        OnWaveCompleted?.Invoke();
    }

    public void RaiseAllWaveCompleted()
    {
        OnAllWaveCompleted?.Invoke();
    }

    public void RaiseTileFlashing(HexTileController tile, float flashDuration)
    {
        OnTileFlashing?.Invoke(tile, flashDuration);
    }
}
