using UnityEngine;
using UnityEngine.Events;

namespace CristiEventSystem.EventChannels
{
    [CreateAssetMenu(menuName = "EventChannels/Coins Event Channel")]
    public class CoinsEventChannel : EventChannelObject
    {
        public UnityAction<int> OnModifyCoins;

        public void RaiseModifyCoins(int cost)
        {
            OnModifyCoins?.Invoke(cost);
        }
    }
}
