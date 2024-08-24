using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/Weight Event Channel")]
    public class WeightEventChannel : EventChannelObject
    {
        public UnityAction<float, HexTile> OnWeightAdded;
        public UnityAction<float, HexTile> OnWeightRemoved;

        public void RaiseWeightAdded(float weight, HexTile hexTile)
        {
            OnWeightAdded?.Invoke(weight, hexTile);
        }
    
        public void RaiseWeightRemoved(float weight, HexTile hexTil)
        {
            OnWeightRemoved?.Invoke(weight, hexTil);
        }
    }
}
