using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/Tower Event Channel")]
public class TowerEventChannel : ScriptableObject
{
    public UnityAction<Transform> OnTowerDestroyed;
    public UnityAction<Transform, HexTile> OnSnapToNewTile;
    

    public void RaiseTowerDestroyed(Transform tower)
    {
        OnTowerDestroyed?.Invoke(tower);
    }
    
    public void RaiseSnapToNewTile(Transform tower, HexTile tile)
    {
        OnSnapToNewTile?.Invoke(tower, tile);
    }
}
