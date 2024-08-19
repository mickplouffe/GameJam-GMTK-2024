using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "EventChannels/Coins Event Channel")]
public class CoinsEventChannel : ScriptableObject
{
    public UnityAction<int> OnModifyCoins;

    public void RaiseModifyCoins(int cost)
    {
        OnModifyCoins?.Invoke(cost);
    }
}
