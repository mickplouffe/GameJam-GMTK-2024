using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinsManager : MonoBehaviourSingleton<CoinsManager>
{
    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins = 100;
    [SerializeField] private int startCoins = 10;

    [SerializeField] private CoinsEventChannel coinsEventChannel;
    [SerializeField] private UIEventChannel uiEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;

    [SerializeField] private AK.Wwise.Event goldGainSFX;
    [SerializeField] private AK.Wwise.Event goldLossSFX;

    public int Coins { get; set; }

    private void OnEnable()
    {
        coinsEventChannel.OnModifyCoins += HandleModifyCoins;
        gameManagerEventChannel.OnGameRestart += () => HandleModifyCoins(startCoins);
    }

    private void OnDisable()
    {
        coinsEventChannel.OnModifyCoins -= HandleModifyCoins;
        gameManagerEventChannel.OnGameRestart -= () => HandleModifyCoins(startCoins);
    }

    public void Start()
    {
        HandleModifyCoins(startCoins);
    }

    private void HandleModifyCoins(int cost)
    {
        switch (Mathf.Sign(cost))
        {
            case < 0:
                goldLossSFX.Post(gameObject);
                break;
            case > 0:
                goldGainSFX.Post(gameObject);
                break;
        }
        
        Coins = Math.Clamp(Coins + cost, minCoins, maxCoins);
        uiEventChannel.RaiseCoinsValueChanged(Coins);
    }
    
    public bool CanBuy(int cost)
    {
        return cost <= Coins;
    }
}
