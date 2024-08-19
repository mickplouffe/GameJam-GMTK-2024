using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviourSingletonPersistent<CoinsManager>
{
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins = 100;
    [SerializeField] private int startCoins = 10;

    [SerializeField] private CoinsEventChannel coinsEventChannel;
    [SerializeField] private UIEventChannel uiEventChannel;
    public int Coins { get; set; }

    private void OnEnable()
    {
        coinsEventChannel.OnModifyCoins += HandleModifyCoins;
    }

    private void OnDisable()
    {
        coinsEventChannel.OnModifyCoins -= HandleModifyCoins;
    }

    public void Start()
    {
        HandleModifyCoins(startCoins);
    }

    private void HandleModifyCoins(int cost)
    {
        Coins = Math.Clamp(Coins + cost, minCoins, maxCoins);
        uiEventChannel.RaiseCoinsValueChanged(Coins);
    }
    
    public bool CanBuy(int cost)
    {
        return cost <= Coins;
    }
}
