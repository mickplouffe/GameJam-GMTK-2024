using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/Tower Event Channel")]
    public class TowerEventChannel : EventChannelObject
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
}
