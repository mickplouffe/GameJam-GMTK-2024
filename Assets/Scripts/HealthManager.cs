using System;
using System.Collections;
using System.Collections.Generic;
using CristiEventSystem.EventChannels;
using EventTriggerSystem;
using EventTriggerSystem.EventActions;
using NaughtyAttributes;
using UnityEngine;

public class HealthManager : MonoBehaviourSingleton<HealthManager>
{
    [SerializeField] private UiEventChannel uiEventChannel;
    [SerializeField] private EnemyEventChannel enemyEventChannel;
    [SerializeField] private GameManagerEventChannel gameManagerEventChannel;

    [SerializeField] private bool isInvincible;
    
    [SerializeField] private float maxHealth = 100;
    private float _currentHealth;
    
    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        enemyEventChannel.OnEnemyAttack += HandleEnemyAttack;
    }
    
    private void OnDisable()
    {
        enemyEventChannel.OnEnemyAttack -= HandleEnemyAttack;
    }

    private void HandleEnemyAttack(int damage)
    {
        if(isInvincible)
            return;
        
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        
        uiEventChannel.RaiseHealthChanged(_currentHealth, maxHealth);
        if (_currentHealth <= 0)
            gameManagerEventChannel.RaiseGameOver();
    }
}
